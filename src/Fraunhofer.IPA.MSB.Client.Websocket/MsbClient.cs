// <copyright file="MsbClient.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
// Copyright 2019 Fraunhofer Institute for Manufacturing Engineering and Automation IPA
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

namespace Fraunhofer.IPA.MSB.Client.Websocket
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Fraunhofer.IPA.MSB.Client.API;
    using Fraunhofer.IPA.MSB.Client.API.EventArgs;
    using Fraunhofer.IPA.MSB.Client.API.Exceptions;
    using Fraunhofer.IPA.MSB.Client.API.Model;
    using Fraunhofer.IPA.MSB.Client.API.OpenApi;
    using Fraunhofer.IPA.MSB.Client.Websocket.Logging;
    using Fraunhofer.IPA.MSB.Client.Websocket.Protocol;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Client to connect to MSB via Websocket.
    /// </summary>
    /// <seealso cref="Fraunhofer.IPA.MSB.Client.API.AbstractMsbClient" />
    public class MsbClient : AbstractMsbClient
    {
        /// <summary>The minimum automatic reconnect interval in milliseconds.</summary>
        public const int MinAutoReconnectIntervalInMilliseconds = 10000;
        private const System.Security.Authentication.SslProtocols SslProtocols = System.Security.Authentication.SslProtocols.Tls12;

        private static readonly ILog Log = LogProvider.For<MsbClient>();

        private int heartbeatIntervalInMilliseconds = 5000;
        private int autoReconnectIntervalInMilliseconds = MinAutoReconnectIntervalInMilliseconds;
        private bool receivedIOConnected = false;
        private bool receivedIORegistered = false;
        private bool receivedIOPublished = false;
        private bool reconnectInProgress = false;

        private WebSocket4Net.WebSocket webSocket;
        private Task reconnectTask;
        private CancellationToken reconnectTaskCancellationToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="MsbClient"/> class.
        /// </summary>
        /// <param name="msbWebsocketUrl">URL of MSB.</param>
        public MsbClient(string msbWebsocketUrl)
            : base(msbWebsocketUrl)
        {
            this.MsbUrl = this.MsbUrl.Replace("http://", "ws://");
            this.MsbUrl = this.MsbUrl.Replace("https://", "wss://");
            if (this.MsbUrl.ToLower().StartsWith("wss://"))
            {
                this.IsSslEnabled = true;

                // Remove wss:// and check if port is specified
                if (!this.MsbUrl.Remove(0, 6).Contains(":"))
                {
                    this.MsbUrl += ":443";
                }
            }
            else
            {
                // Remove ws:// and check if port is specified
                if (!this.MsbUrl.Remove(0, 5).Contains(":"))
                {
                    this.MsbUrl += ":80";
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MsbClient"/> class
        /// </summary>
        /// <param name="msbWebsocketAddress">Adress of MSB (Hostname or IP).</param>
        /// <param name="msbWebsocketPort">Port ob MSB Websocket interface.</param>
        /// <param name="useSSL">Definines if SSL should be used.</param>
        /// <param name="msbWebsocketPath">Path to MSB Websocket interface.</param>
        public MsbClient(string msbWebsocketAddress, uint msbWebsocketPort = 8085, bool useSSL = false, string msbWebsocketPath = "")
        {
            if (useSSL)
            {
                this.MsbUrl = $"wss://{msbWebsocketAddress}:{msbWebsocketPort}/{msbWebsocketPath}";
            }
            else
            {
                this.MsbUrl = $"ws://{msbWebsocketAddress}:{msbWebsocketPort}/{msbWebsocketPath}";
            }

            this.IsSslEnabled = useSSL;
        }

        /// <summary>Occurs when the client has connected.</summary>
        public event EventHandler<EventArgs> Connected;

        /// <summary>Occurs when the client has disconnected.</summary>
        public event EventHandler<EventArgs> Disconnected;

        /// <summary>Occurs when the client connection has closed.</summary>
        public event EventHandler<ConnectionClosedEventArgs> ConnectionClosed;

        /// <summary>Occurs when a client connection attempt failed.</summary>
        public event EventHandler<EventArgs> ConnectionFailed;

        /// <summary>Occurs when the client has registered a <see cref="Service"/>.</summary>
        public event EventHandler<EventArgs> Registered;

        /// <summary>Occurs when a registration attempt of the client for a <see cref="Service"/> failed.</summary>
        public event EventHandler<EventArgs> RegistrationFailed;

        /// <summary>Occurs when the client has published an event.</summary>
        public event EventHandler<EventArgs> EventPublished;

        /// <summary>Occurs when the client has cached an event.</summary>
        public event EventHandler<EventArgs> EventCached;

        /// <summary>Occurs when a publish attempt of the client for a <see cref="Event"/> failed.</summary>
        public event EventHandler<EventArgs> EventPublishingFailed;

        /// <summary>Occurs when the client has received new configuration parameters.</summary>
        public event EventHandler<ConfigurationParameterReceivedEventArgs> ConfigurationParameterReceived;

        /// <summary>
        /// Gets or sets a value indicating whether SockJS is used.
        /// </summary>
        public bool UseSockJs { get; set; } = true;

        /// <summary>
        /// Gets a value indicating whether SSL is enalbed for connecting to MSB.
        /// </summary>
        public bool IsSslEnabled { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the hostname of certificate is verified.
        /// </summary>
        public bool AllowSslCertificateNameMismatch { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether chain errors in certificate are allowed.
        /// </summary>
        public bool AllowSslCertificateChainErrors { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether untrusted certificates are allowed.
        /// </summary>
        public bool AllowSslUnstrustedCertificate { get; set; } = false;

        /// <summary>
        /// Gets or sets the interval for sending a websocket heartbeat.
        /// </summary>
        public int HeatbeatIntervalInMilliseconds
        {
            get
            {
                return this.heartbeatIntervalInMilliseconds;
            }

            set
            {
                Log.Debug($"Heartbeat Interval set to '{value}' ");
                this.heartbeatIntervalInMilliseconds = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the websocket connection reconnects automatically.
        /// </summary>
        public bool AutoReconnect { get; set; } = true;

        /// <summary>
        /// Gets or sets the interval to reconnect automatically to MSB if connection is lost.
        /// </summary>
        public int AutoReconnectIntervalInMilliseconds
        {
            get
            {
                return this.autoReconnectIntervalInMilliseconds;
            }

            set
            {
                if (value >= MinAutoReconnectIntervalInMilliseconds)
                {
                    Log.Debug($"Auto Reconnect Interval set to '{value}' ");
                    this.autoReconnectIntervalInMilliseconds = value;
                }
                else
                {
                    Log.Error($"Auto Reconnect Interval '{value}' lower then minimal Auto Reconnect Interval '{MinAutoReconnectIntervalInMilliseconds}'");
                }
            }
        }

        /// <summary>
        /// Gets the cached events of all services as Dictionary with <see cref="Service"/> as key and a List of cached <see cref="EventData"/> as value.
        /// </summary>
        public Dictionary<Service, List<EventData>> EventCache { get; private set; } = new Dictionary<Service, List<EventData>>();

        /// <summary>
        /// Gets a Dictionary of all registered <see cref="Service"/>s with <see cref="Service.Uuid"/> as key and the <see cref="Service"/> as value.
        /// </summary>
        public ConcurrentDictionary<string, Service> RegisteredServices { get; private set; } = new ConcurrentDictionary<string, Service>();

        /// <summary>
        /// Gets or sets the size of the <see cref="EventCache"/> per <see cref="Service"/>.
        /// </summary>
        public uint EventCacheSizePerService { get; set; } = 1000;

        /// <summary>
        /// Gets or sets the timeout to wait for connection response of MSB.
        /// </summary>
        public int WaitForConnectedInMilliseconds { get; set; } = 3000;

        /// <summary>
        /// Gets or sets the timeout to wait for registration response of MSB.
        /// </summary>
        public int WaitForRegistrationInMilliseconds { get; set; } = 10000;

        /// <summary>
        /// Gets or sets the timeout to wait for registration response of MSB.
        /// </summary>
        public int WaitForPublishInMilliseconds { get; set; } = 10000;

        /// <inheritdoc/>
        public override async Task<bool> ConnectAsync()
        {
            string msbWebsocketUrlWithPath = this.MsbUrl;
            if (this.UseSockJs)
            {
                if (!msbWebsocketUrlWithPath.EndsWith("/"))
                {
                    msbWebsocketUrlWithPath += "/";
                }

                msbWebsocketUrlWithPath += this.GenerateSockJSPath();
            }

            Log.Debug($"Connecting to address '{msbWebsocketUrlWithPath}'");

            this.webSocket = new WebSocket4Net.WebSocket(msbWebsocketUrlWithPath);

            // Security
            this.webSocket.Security.AllowCertificateChainErrors = this.AllowSslCertificateChainErrors;
            this.webSocket.Security.AllowUnstrustedCertificate = this.AllowSslUnstrustedCertificate;
            this.webSocket.Security.AllowNameMismatchCertificate = this.AllowSslCertificateNameMismatch;
            this.webSocket.Security.EnabledSslProtocols = SslProtocols;

            // Connection
            this.webSocket.AutoSendPingInterval = this.HeatbeatIntervalInMilliseconds;
            this.webSocket.EnableAutoSendPing = true;

            // Add listeners
            this.webSocket.MessageReceived += this.OnWebsocketMessageReceived;
            this.webSocket.Opened += this.OnWebsocketOpend;
            this.webSocket.Error += this.OnWebsocketError;
            this.webSocket.Closed += this.OnWebsocketClosed;

            var connectTask = Task.Run(() =>
            {
                this.webSocket.Open();
                CancellationTokenSource source = new CancellationTokenSource();
                while (!this.IsConnected() && !source.IsCancellationRequested)
                {
                    Thread.Sleep(10);
                }
            });

            if (await Task.WhenAny(connectTask, Task.Delay(this.WaitForConnectedInMilliseconds, default(CancellationToken))) == connectTask)
            {
                await connectTask;
                return true;
            }
            else
            {
                Log.Warn($"Connection try to '{this.MsbUrl}' timed out");
                this.ConnectionFailed?.Invoke(this, EventArgs.Empty);
                return false;
            }
        }

        /// <inheritdoc/>
        public override void Disconnect()
        {
            if (this.webSocket != null)
            {
                Log.Debug($"Disconnecting from {this.MsbUrl}");

                this.webSocket.Close("Normal");
                this.receivedIOConnected = false;
                this.webSocket = null;
            }

            this.Disconnected?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc/>
        public override bool IsConnected()
        {
            if (this.webSocket == null)
            {
                return false;
            }

            bool connected = this.webSocket.State == WebSocket4Net.WebSocketState.Open && this.receivedIOConnected;
            Log.Trace("IsConnected: {connected}", connected);
            return connected;
        }

        /// <inheritdoc/>
        public override async Task<bool> RegisterAsync(Service serviceToRegister)
        {
            string messageToSend = MessageGenerator.GenerateRegistrationMessage(serviceToRegister);
            this.RegisteredServices[serviceToRegister.Uuid] = serviceToRegister;

            if (this.UseSockJs)
            {
                messageToSend = messageToSend.Replace("\"", "\\\"");
                messageToSend = string.Format("[\"{0}\"]", messageToSend);
            }

            if (!this.IsConnected())
            {
                Log.Debug("Unable to register service, no connection or connection closed");
                return false;
            }
            else
            {
                this.receivedIORegistered = false;
                var registerTask = Task.Run(() =>
                {
                    this.webSocket.Send(messageToSend + "\n");
                    Log.Debug("Register service '{messageToSend}'", messageToSend);
                    CancellationTokenSource source = new CancellationTokenSource();
                    while (!this.receivedIORegistered && !source.IsCancellationRequested)
                    {
                        Thread.Sleep(10);
                    }
                });

                if (await Task.WhenAny(registerTask, Task.Delay(this.WaitForRegistrationInMilliseconds, default(CancellationToken))) == registerTask)
                {
                    await registerTask;
                    this.receivedIORegistered = false;
                    return true;
                }
                else
                {
                    Log.Warn($"Registration try for '{serviceToRegister.Uuid}' timed out");
                    return false;
                }
            }
        }

        /// <inheritdoc/>
        public override async Task<bool> PublishAsync(Service service, EventData eventData)
        {
            Log.Debug($"Publishing event '{eventData.Event.Id}' for service '{service.Uuid}'");

            if (!this.RegisteredServices.ContainsKey(service.Uuid))
            {
                throw new ServiceNotRegisteredException($"Service '{service.Uuid}' was not registered via this MSB client connected to '{this.MsbUrl}'");
            }

            if (!service.Events.Contains(eventData.Event))
            {
                throw new EventNotExistException($"Event '{eventData.Event.Id}' not added to service '{service.Uuid}'");
            }

            if (this.IsConnected())
            {
                this.receivedIOPublished = false;
                var publishTask = Task.Run(() =>
                {
                    string message = MessageGenerator.GenerateEventMessage(service, eventData, this.UseSockJs);
                    Log.Debug($"Event published '{message}' for service '{service.Uuid}'");
                    this.webSocket.Send(message);
                    CancellationTokenSource source = new CancellationTokenSource();
                    while (!this.receivedIOPublished && !source.IsCancellationRequested)
                    {
                        Thread.Sleep(10);
                    }
                });

                if (await Task.WhenAny(publishTask, Task.Delay(this.WaitForPublishInMilliseconds, default(CancellationToken))) == publishTask)
                {
                    await publishTask;
                    this.receivedIOPublished = false;
                    this.EventPublished?.Invoke(this, EventArgs.Empty);
                    return true;
                }
                else
                {
                    this.CacheEvent(service, eventData);
                    Log.Warn($"Publish try for event '{eventData.Event.Id}' of service ' {service.Uuid}' timed out. Event added to cache.");
                    return false;
                }
            }
            else if (eventData.ShouldBeCached)
            {
                this.CacheEvent(service, eventData);
                return false;
            }
            else
            {
                Log.Debug($"Caching disabled, event '{eventData.Event.Id}' was discarded");
                return false;
            }
        }

        private string GenerateSockJSPath()
        {
            Random genServerId = new Random();
            string server_id = genServerId.Next(100, 999).ToString();
            string transport_id = Guid.NewGuid().ToString().Replace("-", string.Empty);

            return "websocket/data/" + server_id + "/" + transport_id + "/websocket/";
        }

        private void CacheEvent(Service service, EventData eventData)
        {
            if (this.EventCache.ContainsKey(service))
            {
                if (this.EventCache[service].Count < this.EventCacheSizePerService)
                {
                    this.EventCache[service].Add(eventData);
                    Log.Debug("Client not connected, event has been cached");
                }
                else
                {
                    Log.Warn($"Cache for service '{service.Uuid}' full, event was discarded");
                }
            }
            else
            {
                List<EventData> cachedEvents = new List<EventData>();
                cachedEvents.Add(eventData);
                this.EventCache.Add(service, cachedEvents);
            }

            this.EventCached?.Invoke(this, EventArgs.Empty);
        }

        private void OnWebsocketMessageReceived(object sender, WebSocket4Net.MessageReceivedEventArgs e)
        {
            var receivedMessage = e.Message;
            string data;

            if (receivedMessage.Length < 8)
            {
                return;
            }
            else if (this.UseSockJs)
            {
                receivedMessage = receivedMessage.Substring(3, receivedMessage.Length - 5);
                data = receivedMessage.Substring(2);
                data = data.Replace("\\\"", "\"");
            }
            else
            {
                data = receivedMessage.Substring(2);
            }

            string messageType = MessageType.GetTypeOfMessage(receivedMessage);
            Log.Debug($"WebsocketMessage [{messageType}]: {receivedMessage}");
            switch (messageType)
            {
                case MessageType.PING:
                    this.webSocket.Send(MessageGenerator.GeneratePingAnswerMessage(this.UseSockJs));
                    break;

                case MessageType.IO_CONNECTED:
                    this.receivedIOConnected = true;
                    this.reconnectInProgress = false;

                    Task.Run(() =>
                    {
                        foreach (var registeredService in this.RegisteredServices.Values)
                        {
                            Log.Debug($"Re-registering service '{registeredService.Uuid}'");
                            this.RegisterAsync(registeredService).Wait();
                        }

                        foreach (var cacheEntry in this.EventCache)
                        {
                            foreach (var cachedEventData in cacheEntry.Value.ToArray())
                            {
                                if (this.PublishAsync(cacheEntry.Key, cachedEventData).Result)
                                {
                                    cacheEntry.Value.Remove(cachedEventData);
                                }
                            }
                        }
                    });

                    this.Connected?.Invoke(this, EventArgs.Empty);

                    break;

                case MessageType.NIO_ALREADY_CONNECTED:
                    this.ConnectionFailed?.Invoke(this, EventArgs.Empty);

                    break;

                case MessageType.NIO_UNAUTHORIZED_CONNECTION:
                    this.ConnectionFailed?.Invoke(this, EventArgs.Empty);

                    break;

                case MessageType.IO_REGISTERED:
                    this.receivedIORegistered = true;
                    this.Registered?.Invoke(this, EventArgs.Empty);

                    break;

                case MessageType.NIO_REGISTRATION_ERROR:
                    this.RegistrationFailed?.Invoke(this, EventArgs.Empty);

                    break;

                case MessageType.NIO_UNEXPECTED_REGISTRATION_ERROR:
                    this.RegistrationFailed?.Invoke(this, EventArgs.Empty);

                    break;

                case MessageType.IO_PUBLISHED:
                    this.receivedIOPublished = true;
                    this.EventPublished?.Invoke(this, EventArgs.Empty);

                    break;

                case MessageType.NIO_EVENT_FORWARDING_ERROR:
                    this.EventPublishingFailed?.Invoke(this, EventArgs.Empty);

                    break;

                case MessageType.NIO_UNEXPECTED_EVENT_FORWARDING_ERROR:
                    this.EventPublishingFailed?.Invoke(this, EventArgs.Empty);

                    break;

                case MessageType.FUNCTION_CALLBACK:
                    // Simple fix for https://cb.ipa.fraunhofer.de/cb/issue/164519 (Invalid escape of nested JSON trough websocket)
                    data = data.Replace("\\\\", "\\");

                    var functionCall = JsonConvert.DeserializeObject<FunctionCall>(data);
                    this.HandleFunctionCall(functionCall);

                    break;

                case MessageType.CONFIG:
                    var configMessage = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
                    string serviceUuid = configMessage["uuid"].ToString();
                    if (this.RegisteredServices.ContainsKey(serviceUuid))
                    {
                        var service = this.RegisteredServices[serviceUuid];
                        var configParameters = ((JObject)configMessage["params"]).ToObject<Dictionary<string, object>>();
                        ConfigurationParameterReceivedEventArgs eventArgs = new ConfigurationParameterReceivedEventArgs(service, configParameters);
                        this.ConfigurationParameterReceived?.Invoke(this, eventArgs);
                    }
                    else
                    {
                        Log.Error($"Service '{serviceUuid}' not registered via this MsbClient");
                    }

                    break;

                default:
                    Log.Error($"Unknown type of message: {receivedMessage}");
                    break;
            }
        }

        private void OnWebsocketError(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            Log.Debug("Websocket_Error: " + e.Exception.Message);
        }

        private void OnWebsocketOpend(object sender, EventArgs e)
        {
            Log.Trace("Websocket_Opened");
        }

        private void HandleFunctionCall(FunctionCall functionCall)
        {
            Log.Debug($"Callback for function '{functionCall.FunctionId}' of service '{functionCall.ServiceUuid}' received with parameters: {functionCall.FunctionParameters}");
            if (this.RegisteredServices.ContainsKey(functionCall.ServiceUuid))
            {
                var serviceOfFunctionCall = this.RegisteredServices[functionCall.ServiceUuid];
                if (serviceOfFunctionCall.Functions.Find(function => function.Id == functionCall.FunctionId) is Function functionOfService)
                {
                    var pointer = functionOfService.FunctionPointer;
                    var parameters = functionOfService.FunctionPointer.Method.GetParameters();

                    var parameterArrayForInvoke = new object[parameters.Length];
                    foreach (var functionCallParameter in functionCall.FunctionParameters)
                    {
                        var currentParameterCallIndex = AbstractFunctionHandler.GetParameterIndexFromName(functionOfService.FunctionPointer.Method, functionCallParameter.Key);
                        var currentParameterCallType = functionOfService.FunctionPointer.Method.GetParameters()[currentParameterCallIndex].ParameterType;

                        object deserializedParameter = null;
                        if (OpenApiMapper.IsPrimitiveDataType(currentParameterCallType))
                        {
                            deserializedParameter = Convert.ChangeType(functionCallParameter.Value, currentParameterCallType);
                        }
                        else
                        {
                            var valueAt = functionCallParameter.Value.ToString();
                            deserializedParameter = JsonConvert.DeserializeObject(valueAt, currentParameterCallType);
                        }

                        parameterArrayForInvoke[currentParameterCallIndex] = deserializedParameter;
                    }

                    var calledFunction = serviceOfFunctionCall.GetFunctionById(functionCall.FunctionId);
                    Dictionary<string, Event> responseEvents = new Dictionary<string, Event>();
                    foreach (var responseEventId in calledFunction.ResponseEventIds)
                    {
                        responseEvents.Add(responseEventId, serviceOfFunctionCall.GetEventById(responseEventId));
                    }

                    var functionCallInfo = new FunctionCallInfo(this, functionCall.CorrelationId, serviceOfFunctionCall, calledFunction, responseEvents);
                    parameterArrayForInvoke[parameterArrayForInvoke.Length - 1] = functionCallInfo;
                    try
                    {
                        var returnValue = functionOfService.FunctionPointer.DynamicInvoke(parameterArrayForInvoke);
                        if (returnValue != null)
                        {
                            EventData responseEventData = (EventData)returnValue;
                            if (responseEventData.Event.Id.Equals(EventData.NoResponseEvent.Event.Id))
                            {
                                Log.Info("No response event sent because result of function exuction was 'EventData.NoResponseEvent'");
                            }
                            else
                            {
                                if (calledFunction.ResponseEventIds.Contains(responseEventData.Event.Id))
                                {
                                    responseEventData.CorrelationId = functionCall.CorrelationId;
                                    #pragma warning disable CS4014 // Da dieser Aufruf nicht abgewartet wird, wird die Ausführung der aktuellen Methode fortgesetzt, bevor der Aufruf abgeschlossen ist
                                    this.PublishAsync(serviceOfFunctionCall, (EventData)responseEventData);
                                    #pragma warning restore CS4014 // Da dieser Aufruf nicht abgewartet wird, wird die Ausführung der aktuellen Methode fortgesetzt, bevor der Aufruf abgeschlossen ist
                                }
                                else
                                {
                                    Log.Error($"Response event not published, because event '{responseEventData.Event.Id}' wasn't defined as response event of function '{calledFunction.Id}'");
                                }
                            }
                        }
                    }
                    catch (ArgumentException e)
                    {
                        Log.Error($"Failed to invoke function with received parameters: {e}");
                    }
                }
                else
                {
                    Log.Warn($"Function call can't be processed, function '{functionCall.FunctionId}' not found in service '{functionCall.ServiceUuid}'");
                }
            }
            else
            {
                Log.Warn($"Function call can't be processed, service '{functionCall.ServiceUuid}' wasn't registered via this MsbClient");
            }
        }

        private void OnWebsocketClosed(object sender, EventArgs eventArgs)
        {
            ConnectionClosedEventArgs connectionClosedEventArg;
            try
            {
                WebSocket4Net.ClosedEventArgs closedArgs = (WebSocket4Net.ClosedEventArgs)eventArgs;
                connectionClosedEventArg = new ConnectionClosedEventArgs(closedArgs.Code, closedArgs.Reason);
                Log.Debug("WebSocket_Closed: Code: " + closedArgs.Code + ", Reason: " + closedArgs.Reason);
            }
            catch (System.InvalidCastException ex)
            {
                Log.Trace($"Can't cast ClosedEventArgs: '{ex}'");
                connectionClosedEventArg = ConnectionClosedEventArgs.Empty;
            }

            this.receivedIOConnected = false;
            this.ConnectionClosed?.Invoke(this, connectionClosedEventArg);

            if (this.AutoReconnect && !this.reconnectInProgress)
            {
                this.reconnectInProgress = true;
                CancellationTokenSource tokenSource = new CancellationTokenSource();
                this.reconnectTaskCancellationToken = tokenSource.Token;
                this.reconnectTask = Task.Run(
                    () =>
                    {
                        CancellationTokenSource source = new CancellationTokenSource();
                        while (!this.IsConnected() && !source.IsCancellationRequested)
                        {
                            Log.Info($"Trying to reconnect to '{this.MsbUrl}'");
                            bool connected = this.ConnectAsync().Result;
                            Thread.Sleep(this.AutoReconnectIntervalInMilliseconds);
                        }
                    }, this.reconnectTaskCancellationToken);
            }
        }
    }
}
