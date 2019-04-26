// <copyright file="MockWebsocketInterface.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using System.Net;
    using System.Net.Sockets;
    using Fleck;
    using Fraunhofer.IPA.MSB.Client.Websocket.Protocol;

    public class MockWebsocketInterface
    {
        private WebSocketServer server;
        private IWebSocketConnection socket;

        public MockWebsocketInterface()
        {
            this.Port = this.GetNextFreeTcpPort();
            this.URL = $"ws://localhost:{this.Port}";
            this.server = new WebSocketServer($"ws://0.0.0.0:{this.Port}");
        }

        public int Port { get; }

        public string URL { get; }

        public void Start()
        {
            this.server.Start(socket =>
            {
                this.socket = socket;
                socket.OnOpen = () =>
                {
                    socket.Send("a[\"IO_CONNECTED\"]");
                };
                socket.OnClose = () =>
                {
                    Console.WriteLine("Close!");
                };
                socket.OnMessage = message =>
                {
                    Console.WriteLine($"Received message: {message}");
                    string data = message.Substring(2, message.Length - 5);
                    data = data.Replace("\\\"", "\"");

                    switch (MessageType.GetTypeOfMessage(data))
                    {
                        case MessageType.REGISTRATION:
                            this.SendMessageOfType(MessageType.IO_REGISTERED);
                            break;

                        case MessageType.EVENT:
                            this.SendMessageOfType(MessageType.IO_PUBLISHED);
                            break;
                    }
                };
            });
        }

        public void Stop()
        {
            this.socket?.Close();
        }

        public void SendMessageOfType(string messageType)
        {
            this.socket.Send(this.AddSockJsFrame(messageType));
        }

        public void SendConfigurationParameter(string uuid, string parameterName, object parametervalue)
        {
            string message = this.AddSockJsFrame($"K {{\\\"uuid\\\":\\\"{uuid}\\\",\\\"params\\\":{{\\\"{parameterName}\\\":{parametervalue}}}}}");
            this.socket.Send(message);
        }

        private string AddSockJsFrame(string message)
        {
            return $"a[\"{message}\"]";
        }

        private int GetNextFreeTcpPort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }
    }
}
