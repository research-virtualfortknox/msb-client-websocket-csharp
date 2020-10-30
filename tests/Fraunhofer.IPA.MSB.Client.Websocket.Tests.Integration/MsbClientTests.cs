// <copyright file="MsbClientTests.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace Fraunhofer.IPA.MSB.Client.Websocket.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Fraunhofer.IPA.MSB.Client.API.Attributes;
    using Fraunhofer.IPA.MSB.Client.API.EventArgs;
    using Fraunhofer.IPA.MSB.Client.API.Exceptions;
    using Fraunhofer.IPA.MSB.Client.API.Model;
    using Fraunhofer.IPA.MSB.Client.Tests.Shared;
    using Fraunhofer.IPA.MSB.Client.Tests.Shared.Functions;
    using Fraunhofer.IPA.MSB.Client.Websocket.IntegrationTest.Events;
    using Fraunhofer.IPA.MSB.Client.Websocket.Model;
    using Fraunhofer.IPA.MSB.Client.Websocket.Protocol;
    using Newtonsoft.Json;
    using Serilog;
    using Xunit;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    /// <summary>
    /// Tests <see cref="MsbClient"/>.
    /// </summary>
    /// <seealso cref="BaseTests" />
    public class MsbClientTests : BaseTests
    {
        /// <summary>
        /// The owner UUID to verify <see cref="Service"/>s at MSB.
        /// </summary>
        protected const string OwnerUuid = "7c328ad1-cea5-410e-8dd8-6c7ca5a2e4f5";

        protected static readonly string ResourcesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
        protected static readonly string IntegrationFlowsDirectory = Path.Combine(ResourcesDirectory, "IntegrationFlows");

        public MsbClientTests(ITestOutputHelper output)
            : base(output)
        {
            string uuid1 = Guid.NewGuid().ToString();
            string uuid2 = Guid.NewGuid().ToString();
            string uuidGateway = Guid.NewGuid().ToString();
            string uuidGatewayService1 = Guid.NewGuid().ToString();
            string uuidGatewayService2 = Guid.NewGuid().ToString();
            string token1 = uuid1.Split('-').Last();
            string token2 = uuid2.Split('-').Last();
            string tokenGateway = uuidGateway.Split('-').Last();
            string tokenGatewayService1 = uuidGatewayService1.Split('-').Last();
            string tokenGatewayService2 = uuidGatewayService2.Split('-').Last();

            this.MySmartObject = new SmartObject(uuid1, "C# Unit Test SmartObject", "Test SmartObject for Integration Tests of C# Websocket Client", token1);
            this.MyApplication = new Application(uuid2, "C# Unit Test Application", "Test Application for Integration Tests of C# Websocket Client", token2);
            this.MyGateway = new Gateway(uuidGateway, "C# Unit Test Gateway", "Test Gateway for Integration Tests of C# Websocket Client", tokenGateway);
            this.MyGatewaySubService1 = new Application(uuidGatewayService1, "C# Unit Test Gateway SubService 1", "Test SubService 1 of Gateway for Integration Tests of C# Websocket Client", tokenGatewayService1);
            this.MyGatewaySubService2 = new SmartObject(uuidGatewayService2, "C# Unit Test Gateway SubService 2", "Test SubService 2 of Gateway for Integration Tests of C# Websocket Client", tokenGatewayService2);
            this.MyGateway.AddService(this.MyGatewaySubService1);
            this.MyGateway.AddService(this.MyGatewaySubService2);

            this.MsbClient = new MsbClient(TestConfiguration.MsbWebsocketInterfaceUrl);

            HttpClient httpClientSmartObjectMgmt = new HttpClient();
            this.SmartObjectMgmtClient = new Client.Tests.Shared.SmartObjectMgmt.SmartObjectMgmtClient(httpClientSmartObjectMgmt);
            this.SmartObjectMgmtClient.BaseUrl = TestConfiguration.MsbSmartObjectMgmtUrl;

            HttpClient httpClientIntegrationDesignMgmt = new HttpClient();
            this.IntegrationDesignMgmtClient = new Client.Tests.Shared.IntegrationDesignMgmt.IntegrationDesignMgmtClient(httpClientIntegrationDesignMgmt);
            this.IntegrationDesignMgmtClient.BaseUrl = TestConfiguration.MsbIntegrationDesignMgmtUrl;
        }

        /// <summary>Gets the client to communicate with SmartObjectMgmt via REST.</summary>
        protected Client.Tests.Shared.SmartObjectMgmt.SmartObjectMgmtClient SmartObjectMgmtClient { get; }

        /// <summary>Gets the client to communicate with IntegrationDesignMgmt via REST.</summary>
        protected Client.Tests.Shared.IntegrationDesignMgmt.IntegrationDesignMgmtClient IntegrationDesignMgmtClient { get; }

        protected MsbClient MsbClient { get; }

        protected SmartObject MySmartObject { get; }

        protected Application MyApplication { get; }

        protected Gateway MyGateway { get; }

        protected Application MyGatewaySubService1 { get; }

        protected SmartObject MyGatewaySubService2 { get; }

        /// <summary>
        /// Tests <see cref="MsbClient.ConnectAsync"/>.
        /// </summary>
        public class Connect : MsbClientTests
        {
            public Connect(ITestOutputHelper output)
                : base(output)
            {
            }

            [Fact]
            public void ConnectToMsb()
            {
                Assert.True(this.MsbClient.ConnectAsync().Result);

                this.MsbClient.Disconnect();
            }

            [Fact]
            public void ConnectToNowhere()
            {
                MsbClient testMsbClient = new MsbClient("ws://localhost:50505");
                Assert.False(testMsbClient.ConnectAsync().Result);
            }
        }

        /// <summary>
        /// Tests <see cref="MsbClient.Disconnect(bool)"/>.
        /// </summary>
        public class Disconnect : MsbClientTests
        {
            public Disconnect(ITestOutputHelper output)
                : base(output)
            {
            }

            [Fact]
            public void DisconnectFromMsbWhenConnected()
            {
                Assert.True(this.MsbClient.ConnectAsync().Result);
                this.MsbClient.Disconnect();
                Assert.False(this.MsbClient.IsConnected());
            }

            [Fact]
            public void DisconnectFromMsbWhenNotConnected()
            {
                this.MsbClient.Disconnect();
                Assert.False(this.MsbClient.IsConnected());
            }
        }

        /// <summary>
        /// Tests <see cref="MsbClient.RegisterAsync(Service)"/>.
        /// </summary>
        public class Register : MsbClientTests
        {
            public Register(ITestOutputHelper output)
                : base(output)
            {
            }

            [Fact]
            public void RegisterWithoutConnected()
            {
                Assert.False(this.MsbClient.RegisterAsync(this.MySmartObject).Result);
            }

            [Fact]
            public void RegisterEmptyService()
            {
                this.RegisterService(this.MySmartObject);

                this.MsbClient.Disconnect();
            }

            [Fact]
            public void RegisterServiceWithEvents()
            {
                this.MySmartObject.AddEvent(new Event("SimpleEvent", "Simple Event", "Description of SimpleEvent", typeof(SimpleEvent)));
                this.MySmartObject.AddEvent(new Event("AllDataTypesEvent", "All Data Types Event", "Description of AllDataTypesEvent", typeof(AllDataTypesEvent)));
                this.MySmartObject.AddEvent(new Event("ComplexEvent", "Complex Event", "Description of ComplexEvent", typeof(ComplexEvent)));
                this.RegisterService(this.MySmartObject);

                this.MsbClient.Disconnect();
            }

            [Fact]
            public void RegisterServiceWithFunctions()
            {
                this.MySmartObject.AddFunctionHandler(new SimpleFunctionHandler());
                this.RegisterService(this.MySmartObject);

                this.MsbClient.Disconnect();
            }

            [Fact]
            public void RegisterServiceWithEventsAndFunctions()
            {
                this.MySmartObject.AddEvent(new Event("SimpleEvent", "Simple Event", "Description of SimpleEvent", typeof(SimpleEvent)));
                this.MySmartObject.AddEvent(new Event("AllDataTypesEvent", "All Data Types Event", "Description of AllDataTypesEvent", typeof(AllDataTypesEvent)));
                this.MySmartObject.AddEvent(new Event("ComplexEvent", "Complex Event", "Description of ComplexEvent", typeof(ComplexEvent)));
                this.MySmartObject.AddFunctionHandler(new SimpleFunctionHandler());
                this.RegisterService(this.MySmartObject);

                this.MsbClient.Disconnect();
            }

            [Fact]
            public void RegisterTwoServices()
            {
                Assert.True(this.MsbClient.ConnectAsync().Result);
                Assert.True(this.MsbClient.RegisterAsync(this.MyApplication).Result);
                Assert.True(this.MsbClient.RegisterAsync(this.MySmartObject).Result);

                this.MsbClient.Disconnect();
            }

            [Fact]
            public void RegisterAndVerify()
            {
                this.RegisterServiceWithEventsAndFunctions();

                var responseVerify = this.SmartObjectMgmtClient.SmartobjectVerifyAsync(OwnerUuid, new MemoryStream(Encoding.ASCII.GetBytes(this.MySmartObject.Token))).Result;
                Assert.Equal(201, responseVerify.StatusCode);

                this.MsbClient.Disconnect();
                var responseDelete = this.SmartObjectMgmtClient.SmartobjectDeleteAsync(this.MySmartObject.Uuid).Result;
                Assert.Equal(201, responseDelete.StatusCode);
            }

            [Fact]
            private void RegisterGatewayWithoutServices()
            {
                this.MyGateway.RemoveService(this.MyGatewaySubService1);
                this.MyGateway.RemoveService(this.MyGatewaySubService2);

                Assert.True(this.MsbClient.ConnectAsync().Result);
                Assert.True(this.MsbClient.RegisterAsync(this.MyGateway).Result);

                this.MsbClient.Disconnect();
            }

            [Fact]
            private void RegisterGatewayWithTwoServices()
            {
                Assert.True(this.MsbClient.ConnectAsync().Result);
                Assert.True(this.MsbClient.RegisterAsync(this.MyGateway).Result);

                this.MsbClient.Disconnect();
            }

            private void RegisterService(Service serviceToRegister)
            {
                Assert.True(this.MsbClient.ConnectAsync().Result);
                Assert.True(this.MsbClient.RegisterAsync(serviceToRegister).Result);
            }
        }

        /// <summary>
        /// Tests <see cref="MsbClient.PublishAsync(Service, EventData)"/>.
        /// </summary>
        public class Publish : MsbClientTests
        {
            private Event testEvent;
            private Event testResponseEvent;
            private Function testReceiveFunction;
            private Function testReceiveFunctionWithResponseEvent;
            private EventData testEventData;
            private bool receivedFunctionCallViaMsb = false;
            private string receivedStringViaMsb = string.Empty;

            public Publish(ITestOutputHelper output)
                : base(output)
            {
                this.testEvent = new Event("TestEvent", "Test Event", "Description of test event", typeof(string));
                this.testResponseEvent = new Event("TestResponseEvent", "Test Response Event", "Description of test response event", typeof(string));
                this.testReceiveFunction = new Function(this.GetType().GetRuntimeMethod("ReceivedPublishedEventViaMsb", new Type[] { typeof(string), typeof(FunctionCallInfo) }), this);
                this.testReceiveFunctionWithResponseEvent = new Function(this.GetType().GetRuntimeMethod("ReceivedPublishedEventViaMsbAndSendResponseEvent", new Type[] { typeof(string), typeof(FunctionCallInfo) }), this);
                this.testEventData = new EventDataBuilder(this.testEvent)
                    .SetValue("TestValue")
                    .Build();
            }

            [Fact]
            public void PublishEvent()
            {
                this.MySmartObject.AddEvent(this.testEvent);

                Assert.True(this.MsbClient.ConnectAsync().Result);
                Assert.True(this.MsbClient.RegisterAsync(this.MySmartObject).Result);
                Assert.True(this.MsbClient.PublishAsync(this.MySmartObject, this.testEventData).Result);

                this.MsbClient.Disconnect();
            }

            [Fact]
            public void PublishEventAndReceiveViaSecondClient()
            {
                string integrationFlowJson = File.ReadAllText($@"{IntegrationFlowsDirectory}/IntegrationFlow_PublishReceive.json");
                integrationFlowJson = integrationFlowJson.Replace("%%%FlowName%%%", "PublishEventAndReceiveViaSecondClient");
                integrationFlowJson = integrationFlowJson.Replace("%%%OwnerUUID%%%", OwnerUuid);
                integrationFlowJson = integrationFlowJson.Replace("%%%Service1UUID%%%", this.MySmartObject.Uuid);
                integrationFlowJson = integrationFlowJson.Replace("%%%Service2UUID%%%", this.MyApplication.Uuid);
                this.MySmartObject.AddEvent(this.testEvent);
                this.MyApplication.AddFunction(this.testReceiveFunction);

                Assert.True(this.MsbClient.ConnectAsync().Result);
                Assert.True(this.MsbClient.RegisterAsync(this.MySmartObject).Result);
                var responseSmartObjectVerification = this.SmartObjectMgmtClient.SmartobjectVerifyAsync(OwnerUuid, new MemoryStream(Encoding.ASCII.GetBytes(this.MySmartObject.Token))).Result;
                Assert.Equal(201, responseSmartObjectVerification.StatusCode);
                Assert.True(this.MsbClient.RegisterAsync(this.MyApplication).Result);
                var responseApplicationVerification = this.SmartObjectMgmtClient.ApplicationVerifyAsync(OwnerUuid, new MemoryStream(Encoding.ASCII.GetBytes(this.MyApplication.Token))).Result;
                Assert.Equal(201, responseApplicationVerification.StatusCode);

                var integrationFlowId = this.IntegrationDesignMgmtClient.CreateAndDeployUsingPOSTAsync(integrationFlowJson).Result;

                Assert.True(this.MsbClient.PublishAsync(this.MySmartObject, this.testEventData).Result);
                bool spinUntil = System.Threading.SpinWait.SpinUntil(() => this.receivedFunctionCallViaMsb == true, TimeSpan.FromSeconds(5));
                Assert.True(this.receivedFunctionCallViaMsb);
                Assert.Equal("TestValue", this.receivedStringViaMsb);

                this.MsbClient.Disconnect();
                var responseDeleteSmartObject = this.SmartObjectMgmtClient.SmartobjectDeleteAsync(this.MySmartObject.Uuid).Result;
                Assert.Equal(201, responseDeleteSmartObject.StatusCode);
                var responseDeleteApplication = this.SmartObjectMgmtClient.ApplicationDeleteAsync(this.MyApplication.Uuid).Result;
                Assert.Equal(201, responseDeleteApplication.StatusCode);
                var responseDeleteIntegrationFlow = this.IntegrationDesignMgmtClient.DeleteUsingDELETEAsync(integrationFlowId).Result;
                Assert.Equal(201, responseDeleteIntegrationFlow.StatusCode);
            }

            #pragma warning disable xUnit1013 // Public method should be marked as test
            [MsbFunction(
                Id = "ReceiveString",
                Name = "Receive String")]
            public void ReceivedPublishedEventViaMsb(
                [MsbFunctionParameter(Name = "message")] string stringParameter,
                FunctionCallInfo functionCallInfo)
            {
                this.receivedStringViaMsb = stringParameter;
                this.receivedFunctionCallViaMsb = true;
            }
            #pragma warning restore xUnit1013 // Public method should be marked as test

            [Fact]
            public void PublishEventAndReceiveWithFunctionWithResponseEvent()
            {
                string integrationFlowJson = File.ReadAllText($@"{IntegrationFlowsDirectory}/IntegrationFlow_PublishReceiveWithResponseEvent.json");
                integrationFlowJson = integrationFlowJson.Replace("%%%FlowName%%%", "PublishEventAndReceiveWithFunctionWithResponseEvent");
                integrationFlowJson = integrationFlowJson.Replace("%%%OwnerUUID%%%", OwnerUuid);
                integrationFlowJson = integrationFlowJson.Replace("%%%Service1UUID%%%", this.MySmartObject.Uuid);
                integrationFlowJson = integrationFlowJson.Replace("%%%Service2UUID%%%", this.MyApplication.Uuid);
                this.MySmartObject.AddEvent(this.testEvent);
                this.MyApplication.AddEvent(this.testResponseEvent);
                this.MyApplication.AddFunction(this.testReceiveFunction);
                this.MyApplication.AddFunction(this.testReceiveFunctionWithResponseEvent);

                Assert.True(this.MsbClient.ConnectAsync().Result);
                Assert.True(this.MsbClient.RegisterAsync(this.MySmartObject).Result);
                var responseSmartObjectVerification = this.SmartObjectMgmtClient.SmartobjectVerifyAsync(OwnerUuid, new MemoryStream(Encoding.ASCII.GetBytes(this.MySmartObject.Token))).Result;
                Assert.Equal(201, responseSmartObjectVerification.StatusCode);
                Assert.True(this.MsbClient.RegisterAsync(this.MyApplication).Result);
                var responseApplicationVerification = this.SmartObjectMgmtClient.ApplicationVerifyAsync(OwnerUuid, new MemoryStream(Encoding.ASCII.GetBytes(this.MyApplication.Token))).Result;
                Assert.Equal(201, responseApplicationVerification.StatusCode);

                var integrationFlowId = this.IntegrationDesignMgmtClient.CreateAndDeployUsingPOSTAsync(integrationFlowJson).Result;

                Assert.True(this.MsbClient.PublishAsync(this.MySmartObject, this.testEventData).Result);
                bool spinUntil = System.Threading.SpinWait.SpinUntil(() => this.receivedFunctionCallViaMsb == true, TimeSpan.FromSeconds(10));
                Assert.True(this.receivedFunctionCallViaMsb);
                Assert.Equal("TestValue", this.receivedStringViaMsb);

                this.MsbClient.Disconnect();
                var responseDeleteSmartObject = this.SmartObjectMgmtClient.SmartobjectDeleteAsync(this.MySmartObject.Uuid).Result;
                Assert.Equal(201, responseDeleteSmartObject.StatusCode);
                var responseDeleteApplication = this.SmartObjectMgmtClient.ApplicationDeleteAsync(this.MyApplication.Uuid).Result;
                Assert.Equal(201, responseDeleteApplication.StatusCode);
                var responseDeleteIntegrationFlow = this.IntegrationDesignMgmtClient.DeleteUsingDELETEAsync(integrationFlowId).Result;
                Assert.Equal(201, responseDeleteIntegrationFlow.StatusCode);
            }

            #pragma warning disable xUnit1013 // Public method should be marked as test
            [MsbFunction(
                Id = "ReceiveStringAndPublishItAsResponseEvent",
                Name = "Receive String And Publish It as Response Event",
                ResponseEvents = new string[] { "TestResponseEvent" })]
            public EventData ReceivedPublishedEventViaMsbAndSendResponseEvent(
                [MsbFunctionParameter(Name = "message")] string stringParameter,
                FunctionCallInfo functionCallInfo)
            {
                return new EventDataBuilder(functionCallInfo.ResponseEvents["TestResponseEvent"]).SetValue(stringParameter).Build();
            }
            #pragma warning restore xUnit1013 // Public method should be marked as test

            [Fact]
            public void PublishWithoutRegistration()
            {
                var exception = Assert.ThrowsAsync<ServiceNotRegisteredException>(async () => await this.MsbClient.PublishAsync(this.MySmartObject, this.testEventData)).Result;
                Assert.Contains("was not registered via this MSB client", exception.Message);
            }

            [Fact]
            public void PublishWithoutAddedEvent()
            {
                Assert.True(this.MsbClient.ConnectAsync().Result);
                Assert.True(this.MsbClient.RegisterAsync(this.MySmartObject).Result);

                var exception = Assert.ThrowsAsync<EventNotExistException>(async () => await this.MsbClient.PublishAsync(this.MySmartObject, this.testEventData)).Result;
                Assert.Contains("not added to service", exception.Message);

                this.MsbClient.Disconnect();
            }

            [Fact]
            public void PublishWithoutConnectionAndCacheEnabled()
            {
                this.MySmartObject.AddEvent(this.testEvent);
                Assert.True(this.MsbClient.ConnectAsync().Result);
                Assert.True(this.MsbClient.RegisterAsync(this.MySmartObject).Result);
                this.MsbClient.AutoReconnect = false;
                this.MsbClient.Disconnect();

                Assert.False(this.MsbClient.PublishAsync(this.MySmartObject, this.testEventData).Result);
                Log.Information($"Cached events: {JsonConvert.SerializeObject(this.MsbClient.EventCache)}");
                this.MsbClient.EventCache.Should().ContainKey(this.MySmartObject);
                this.MsbClient.EventCache[this.MySmartObject].Should().Contain(this.testEventData);

                Assert.False(this.MsbClient.PublishAsync(this.MySmartObject, this.testEventData).Result);
                Log.Information($"Cached events: {JsonConvert.SerializeObject(this.MsbClient.EventCache)}");
                this.MsbClient.EventCache.Should().ContainKey(this.MySmartObject);
                this.MsbClient.EventCache[this.MySmartObject].Should().HaveCount(2);

                this.MsbClient.Disconnect();
            }

            [Fact]
            public void PublishWithoutConnectionAndCacheDisabled()
            {
                this.testEventData.ShouldBeCached = false;
                this.MySmartObject.AddEvent(this.testEvent);
                Assert.True(this.MsbClient.ConnectAsync().Result);
                Assert.True(this.MsbClient.RegisterAsync(this.MySmartObject).Result);
                this.MsbClient.AutoReconnect = false;
                this.MsbClient.Disconnect();

                Assert.False(this.MsbClient.PublishAsync(this.MySmartObject, this.testEventData).Result);
                this.MsbClient.EventCache.Should().NotContainKey(this.MySmartObject);

                this.MsbClient.Disconnect();
            }
        }

        public class EventHandling : MsbClientTests
        {
            private MsbClient testMsbClient;

            private MockWebsocketInterface mockWebsocketInterface;

            public EventHandling(ITestOutputHelper output)
                 : base(output)
            {
                this.mockWebsocketInterface = new MockWebsocketInterface();
                this.mockWebsocketInterface.Start();
                this.testMsbClient = new MsbClient(this.mockWebsocketInterface.URL);
            }

            [Fact]
            public void Connected()
            {
                var raisedEvent = Assert.RaisesAnyAsync<EventArgs>(
                    h => this.testMsbClient.Connected += h,
                    h => this.testMsbClient.Connected -= h,
                    () => Task.Run(() =>
                    {
                        Assert.True(this.testMsbClient.ConnectAsync().Result);
                        Thread.Sleep(100);
                    })).Result;

                Assert.IsType<EventArgs>(raisedEvent.Arguments);
            }

            [Fact]
            public void Disconnected()
            {
                var raisedEvent = Assert.RaisesAnyAsync<EventArgs>(
                    h => this.testMsbClient.Disconnected += h,
                    h => this.testMsbClient.Disconnected -= h,
                    () => Task.Run(() =>
                    {
                        this.testMsbClient.Disconnect();
                        Thread.Sleep(100);
                    })).Result;

                Assert.IsType<EventArgs>(raisedEvent.Arguments);
            }

            [Fact]
            public void ConnectionClosed()
            {
                Assert.True(this.testMsbClient.ConnectAsync().Result);
                var raisedEvent = Assert.RaisesAnyAsync<ConnectionClosedEventArgs>(
                    h => this.testMsbClient.ConnectionClosed += h,
                    h => this.testMsbClient.ConnectionClosed -= h,
                    () => Task.Run(() =>
                    {
                        this.mockWebsocketInterface.Stop();
                        Thread.Sleep(100);
                    })).Result;

                Assert.IsType<ConnectionClosedEventArgs>(raisedEvent.Arguments);
                Assert.Equal(1000, raisedEvent.Arguments.Code);
                Assert.Equal(string.Empty, raisedEvent.Arguments.Reason);
            }

            [Fact]
            public void ConnectionFailed_HostNotAvailable()
            {
                MsbClient clientToNowhere = new MsbClient("ws://NotExistingHost:12345");
                var raisedEvent = Assert.RaisesAnyAsync<EventArgs>(
                    h => clientToNowhere.ConnectionFailed += h,
                    h => clientToNowhere.ConnectionFailed -= h,
                    () => Task.Run(() =>
                    {
                        Assert.False(clientToNowhere.ConnectAsync().Result);
                        Thread.Sleep(100);
                    })).Result;

                Assert.IsType<EventArgs>(raisedEvent.Arguments);
            }

            [Fact]
            public void ConnectionFailed_NIO_UNAUTHORIZED_CONNECTION()
            {
                Assert.True(this.testMsbClient.ConnectAsync().Result);
                var raisedEvent = Assert.RaisesAnyAsync<EventArgs>(
                    h => this.testMsbClient.ConnectionFailed += h,
                    h => this.testMsbClient.ConnectionFailed -= h,
                    () => Task.Run(() =>
                    {
                        this.mockWebsocketInterface.SendMessageOfType(MessageType.NIO_UNAUTHORIZED_CONNECTION);
                        Thread.Sleep(100);
                    })).Result;

                Assert.IsType<EventArgs>(raisedEvent.Arguments);
            }

            [Fact]
            public void ConnectionFailed_NIO_ALREADY_CONNECTED()
            {
                Assert.True(this.testMsbClient.ConnectAsync().Result);
                var raisedEvent = Assert.RaisesAnyAsync<EventArgs>(
                    h => this.testMsbClient.ConnectionFailed += h,
                    h => this.testMsbClient.ConnectionFailed -= h,
                    () => Task.Run(() =>
                    {
                        this.mockWebsocketInterface.SendMessageOfType(MessageType.NIO_ALREADY_CONNECTED);
                        Thread.Sleep(100);
                    })).Result;

                Assert.IsType<EventArgs>(raisedEvent.Arguments);
            }

            [Fact]
            public void Registered()
            {
                SmartObject testSmartObject = new SmartObject("Id", "Name", "Description", "Token");

                Assert.True(this.testMsbClient.ConnectAsync().Result);
                var raisedEvent = Assert.RaisesAnyAsync<EventArgs>(
                    h => this.testMsbClient.Registered += h,
                    h => this.testMsbClient.Registered -= h,
                    () => Task.Run(() =>
                    {
                        Assert.True(this.testMsbClient.RegisterAsync(testSmartObject).Result);
                        Thread.Sleep(100);
                    })).Result;

                Assert.IsType<EventArgs>(raisedEvent.Arguments);
            }

            [Fact]
            public void RegistrationFailed_NIO_REGISTRAION_ERROR()
            {
                Assert.True(this.testMsbClient.ConnectAsync().Result);
                var raisedEvent = Assert.RaisesAnyAsync<EventArgs>(
                    h => this.testMsbClient.RegistrationFailed += h,
                    h => this.testMsbClient.RegistrationFailed -= h,
                    () => Task.Run(() =>
                    {
                        this.mockWebsocketInterface.SendMessageOfType(MessageType.NIO_REGISTRATION_ERROR);
                        Thread.Sleep(100);
                    })).Result;
                Assert.IsType<EventArgs>(raisedEvent.Arguments);
            }

            [Fact]
            public void RegistrationFailed_NIO_UNEXPECTED_REGISTRATION_ERROR()
            {
                Assert.True(this.testMsbClient.ConnectAsync().Result);
                var raisedEvent = Assert.RaisesAnyAsync<EventArgs>(
                    h => this.testMsbClient.RegistrationFailed += h,
                    h => this.testMsbClient.RegistrationFailed -= h,
                    () => Task.Run(() =>
                    {
                        this.mockWebsocketInterface.SendMessageOfType(MessageType.NIO_UNEXPECTED_REGISTRATION_ERROR);
                        Thread.Sleep(100);
                    })).Result;
                Assert.IsType<EventArgs>(raisedEvent.Arguments);
            }

            [Fact]
            public void EventPublished()
            {
                Event testEvent = new Event("Id", "Name", "Description", typeof(string));
                EventData testEventData = new EventDataBuilder(testEvent).Build();
                SmartObject testSmartobject = new SmartObject("Id", "Name", "Description", "Token");
                testSmartobject.AddEvent(testEvent);
                Assert.True(this.testMsbClient.ConnectAsync().Result);
                Assert.True(this.testMsbClient.RegisterAsync(testSmartobject).Result);

                var raisedEvent = Assert.RaisesAnyAsync<EventArgs>(
                    h => this.testMsbClient.EventPublished += h,
                    h => this.testMsbClient.EventPublished -= h,
                    () => Task.Run(() =>
                    {
                        Assert.True(this.testMsbClient.PublishAsync(testSmartobject, testEventData).Result);
                        Thread.Sleep(100);
                    })).Result;
                Assert.IsType<EventArgs>(raisedEvent.Arguments);
            }

            [Fact]
            public void EventCached()
            {
                Event testEvent = new Event("Id", "Name", "Description", typeof(string));
                EventData testEventData = new EventDataBuilder(testEvent).Build();
                SmartObject testSmartobject = new SmartObject("Id", "Name", "Description", "Token");
                testSmartobject.AddEvent(testEvent);
                Assert.True(this.testMsbClient.ConnectAsync().Result);
                Assert.True(this.testMsbClient.RegisterAsync(testSmartobject).Result);
                this.MsbClient.AutoReconnect = false;
                this.testMsbClient.Disconnect();

                var raisedEvent = Assert.RaisesAnyAsync<EventArgs>(
                    h => this.testMsbClient.EventCached += h,
                    h => this.testMsbClient.EventCached -= h,
                    () => Task.Run(() =>
                    {
                        Assert.False(this.testMsbClient.PublishAsync(testSmartobject, testEventData).Result);
                        Thread.Sleep(100);
                    })).Result;
                Assert.IsType<EventArgs>(raisedEvent.Arguments);
            }

            [Fact]
            public void EventPublishingFailed_NIO_EVENT_FORWARDING_ERROR()
            {
                Assert.True(this.testMsbClient.ConnectAsync().Result);
                var raisedEvent = Assert.RaisesAnyAsync<EventArgs>(
                    h => this.testMsbClient.EventPublishingFailed += h,
                    h => this.testMsbClient.EventPublishingFailed -= h,
                    () => Task.Run(() =>
                    {
                        this.mockWebsocketInterface.SendMessageOfType(MessageType.NIO_EVENT_FORWARDING_ERROR);
                        Thread.Sleep(100);
                    })).Result;
                Assert.IsType<EventArgs>(raisedEvent.Arguments);
            }

            [Fact]
            public void EventPublishingFailed_NIO_UNEXPECTED_EVENT_FORWARDING_ERROR()
            {
                Assert.True(this.testMsbClient.ConnectAsync().Result);
                var raisedEvent = Assert.RaisesAnyAsync<EventArgs>(
                    h => this.testMsbClient.EventPublishingFailed += h,
                    h => this.testMsbClient.EventPublishingFailed -= h,
                    () => Task.Run(() =>
                    {
                        this.mockWebsocketInterface.SendMessageOfType(MessageType.NIO_UNEXPECTED_EVENT_FORWARDING_ERROR);
                        Thread.Sleep(100);
                    })).Result;
                Assert.IsType<EventArgs>(raisedEvent.Arguments);
            }

            [Fact]
            public void ConfigurationParameterReceived()
            {
                SmartObject testSmartObject = new SmartObject("6e93a348-35cc-4fe1-b6fa-7015787dec61", "Name", "Description", "Token");
                string serviceUuid = testSmartObject.Uuid;
                string parameterName = "testParameter";
                object parameterValue = 43536;

                Assert.True(this.testMsbClient.ConnectAsync().Result);
                Assert.True(this.testMsbClient.RegisterAsync(testSmartObject).Result);
                var raisedEvent = Assert.RaisesAnyAsync<ConfigurationParameterReceivedEventArgs>(
                    h => this.testMsbClient.ConfigurationParameterReceived += h,
                    h => this.testMsbClient.ConfigurationParameterReceived -= h,
                    () => Task.Run(() =>
                    {
                        this.mockWebsocketInterface.SendConfigurationParameter(serviceUuid, parameterName, parameterValue);
                        Thread.Sleep(100);
                    })).Result;
                Assert.IsType<ConfigurationParameterReceivedEventArgs>(raisedEvent.Arguments);
                Assert.Equal(testSmartObject, raisedEvent.Arguments.Service);
                raisedEvent.Arguments.Value.Should().Contain(new KeyValuePair<string, object>(parameterName, parameterValue));
            }

            [Fact]
            public void ConfigurationParameterReceivedForNotRegisteredService()
            {
                SmartObject testSmartObject = new SmartObject("6e93a348-35cc-4fe1-b6fa-7015787dec61", "Name", "Description", "Token");
                string serviceUuid = testSmartObject.Uuid;
                string parameterName = "testParameter";
                object parameterValue = 43536;

                Assert.True(this.testMsbClient.ConnectAsync().Result);
                var exception = Assert.ThrowsAsync<RaisesException>(
                    async () => await Assert.RaisesAnyAsync<ConfigurationParameterReceivedEventArgs>(
                    h => this.testMsbClient.ConfigurationParameterReceived += h,
                    h => this.testMsbClient.ConfigurationParameterReceived -= h,
                    () => Task.Run(() =>
                    {
                        this.mockWebsocketInterface.SendConfigurationParameter(serviceUuid, parameterName, parameterValue);
                        Thread.Sleep(100);
                    }))).Result;

                Assert.Contains("No event was raised", exception.Message);
            }
        }

        public class OnWebsocketMessageReceived : MsbClientTests
        {
            private MsbClient firstMsbClient;

            private MsbClient secondMsbClient;

            private SmartObject testSmartObject;

            public OnWebsocketMessageReceived(ITestOutputHelper output)
                 : base(output)
            {
                this.firstMsbClient = new MsbClient(TestConfiguration.MsbWebsocketInterfaceUrl);
                this.secondMsbClient = new MsbClient(TestConfiguration.MsbWebsocketInterfaceUrl);
                this.testSmartObject = new SmartObject("55d9de0f-618e-4051-99c1-76d7f8d92876", "Name", "Description", "0cd8e806-aaf8-4d97-b399-baaa1535c1ae");
            }

            public class PingPong : OnWebsocketMessageReceived
            {
                public PingPong(ITestOutputHelper output)
                     : base(output)
                {
                }

                [Fact]
                public void TestPingPong()
                {
                    Assert.True(this.firstMsbClient.ConnectAsync().Result);
                    Assert.True(this.firstMsbClient.RegisterAsync(this.testSmartObject).Result);
                    Assert.True(this.secondMsbClient.ConnectAsync().Result);
                    var raisedEvent = Assert.RaisesAnyAsync<EventArgs>(
                        h => this.secondMsbClient.ConnectionFailed += h,
                        h => this.secondMsbClient.ConnectionFailed -= h,
                        () => Task.Run(() =>
                        {
                            Assert.False(this.secondMsbClient.RegisterAsync(this.testSmartObject).Result);
                            Thread.Sleep(100);
                        })).Result;
                    Assert.IsType<EventArgs>(raisedEvent.Arguments);
                }
            }

            public class FunctionCall : OnWebsocketMessageReceived
            {
                private bool testFunctionCallReceived = false;

                public FunctionCall(ITestOutputHelper output)
                     : base(output)
                {
                }

                [Fact]
                public void PrimtiveFunctionParameterConversion_Integers()
                {
                    var mockWebsocketInterface = new MockWebsocketInterface();
                    mockWebsocketInterface.Start();
                    var testMsbClient = new MsbClient(mockWebsocketInterface.URL);
                    var testSmartObject = new SmartObject(Guid.NewGuid().ToString(), "Name", "Description", Guid.NewGuid().ToString());
                    var testFunction = new Function(this.GetType().GetRuntimeMethod("TestFunction", new Type[] { typeof(int), typeof(FunctionCallInfo) }), this);
                    testSmartObject.AddFunction(testFunction);

                    Assert.True(testMsbClient.ConnectAsync().Result);
                    Assert.True(testMsbClient.RegisterAsync(testSmartObject).Result);
                    string functionCallJson = $@"{MessageType.FUNCTION_CALLBACK} {{
                        ""uuid"" : ""{testSmartObject.Uuid}"",
                        ""correlationId"" : ""{Guid.NewGuid().ToString()}"",
                        ""functionId"" : ""TestFunction"",
                        ""functionParameters"" : {{
                            ""testParameter"" : 1234
                        }}
                    }}";

                    mockWebsocketInterface.SendMessageOfType(functionCallJson);
                    var maxPollTries = 10;
                    for (int i = 0; i < maxPollTries; i++)
                    {
                        if (this.testFunctionCallReceived)
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(50);
                        }
                    }

                    Assert.True(this.testFunctionCallReceived);
                }

                #pragma warning disable xUnit1013 // Public method should be marked as test
                [MsbFunction(
                    Id = "TestFunction",
                    Name = "Test Function")]
                public void TestFunction(
                    [MsbFunctionParameter(Name = "testParameter")] int testParameter,
                    FunctionCallInfo functionCallInfo)
                {
                    if (testParameter.GetType().Equals(typeof(int)))
                    {
                        this.testFunctionCallReceived = true;
                    }
                    else
                    {
                        Log.Error($"Expected parameter type '{typeof(int)}' but was '{testParameter.GetType()}'");
                    }
                }
                #pragma warning restore xUnit1013 // Public method should be marked as test

                [Fact]
                public void NoResponseEventShouldBeSendForFunctionCall()
                {
                    var mockWebsocketInterface = new MockWebsocketInterface();
                    mockWebsocketInterface.Start();
                    var testMsbClient = new MsbClient(mockWebsocketInterface.URL);
                    var testSmartObject = new SmartObject(Guid.NewGuid().ToString(), "Name", "Description", Guid.NewGuid().ToString());
                    var responseEventWhichShouldNotBeSend = new Event("ResponseEventWhichShouldNotBeSend", string.Empty, string.Empty, new DataFormat());
                    testSmartObject.AddEvent(responseEventWhichShouldNotBeSend);
                    var testFunction = new Function(this.GetType().GetRuntimeMethod("NoResponseEventShouldBeSendForFunctionCallMsbFunction", new Type[] { typeof(FunctionCallInfo) }), this);
                    testSmartObject.AddFunction(testFunction);

                    Assert.True(testMsbClient.ConnectAsync().Result);
                    Assert.True(testMsbClient.RegisterAsync(testSmartObject).Result);
                    string functionCallJson = $@"{MessageType.FUNCTION_CALLBACK} {{
                        ""uuid"" : ""{testSmartObject.Uuid}"",
                        ""correlationId"" : ""{Guid.NewGuid().ToString()}"",
                        ""functionId"" : ""NoResponseEventShouldBeSendForFunctionCallMsbFunction"",
                        ""functionParameters"" : {{ }}
                    }}";

                    try
                    {
                        var raisedEvent = Assert.RaisesAnyAsync<EventArgs>(
                        h => testMsbClient.EventPublished += h,
                        h => testMsbClient.EventPublished -= h,
                        () => Task.Run(() =>
                        {
                            mockWebsocketInterface.SendMessageOfType(functionCallJson);
                            Thread.Sleep(100);
                        })).Result;
                    }
                    catch (AggregateException e)
                    {
                        Assert.Contains("No event was raised", e.InnerException.Message);
                    }
                }

                #pragma warning disable xUnit1013 // Public method should be marked as test
                [MsbFunction(
                    Id = "NoResponseEventShouldBeSendForFunctionCallMsbFunction",
                    Name = "NoResponseEventShouldBeSendForFunctionCallMsbFunction",
                    ResponseEvents = new string[] { "ResponseEventWhichShouldNotBeSend" })]
                public EventData NoResponseEventShouldBeSendForFunctionCallMsbFunction(
                    FunctionCallInfo functionCallInfo)
                {
                    return EventData.NoResponseEvent;
                }
                #pragma warning restore xUnit1013 // Public method should be marked as test
            }
        }
    }
}
