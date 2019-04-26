// <copyright file="FunctionCallInfoTests.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace Fraunhofer.IPA.MSB.Client.API.Tests.Unit.Model
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;
    using Fraunhofer.IPA.MSB.Client.API.Attributes;
    using Fraunhofer.IPA.MSB.Client.API.Model;
    using Xunit;

    public class FunctionCallInfoTests
    {
        public FunctionCallInfoTests()
        {
        }

        public class Constructors : FunctionCallInfoTests
        {
            private FunctionCallInfo testFunctionCallInfo;

            private AbstractMsbClient expectedMsbClient = new MsbClientForTest();

            private string expectedCorrelationId = "3b0d9fb6-9e1b-4e6e-b671-a03e79908177";

            private Service expectedService = new ServiceForTest("uuid", "name", "description", "token");

            private Function expectedFunction;

            private Dictionary<string, Event> expectedResponseEvents = new Dictionary<string, Event>();

            public Constructors()
            {
                var methodInfo = this.GetType().GetRuntimeMethod("MsbFunction", new Type[] { typeof(FunctionCallInfo) });
                this.expectedFunction = new Function("id", "name", "description", methodInfo, this);
                this.testFunctionCallInfo = new FunctionCallInfo(
                    this.expectedMsbClient,
                    this.expectedCorrelationId,
                    this.expectedService,
                    this.expectedFunction,
                    this.expectedResponseEvents);
            }

            [Fact]
            public void CheckMsbClient()
            {
                Assert.Equal(this.expectedMsbClient, this.testFunctionCallInfo.MsbClient);
            }

            [Fact]
            public void CheckCorrelationId()
            {
                Assert.Equal(this.expectedCorrelationId, this.testFunctionCallInfo.CorrelationId);
            }

            [Fact]
            public void CheckService()
            {
                Assert.Equal(this.expectedService, this.testFunctionCallInfo.Service);
            }

            [Fact]
            public void CheckFunction()
            {
                Assert.Equal(this.expectedFunction, this.testFunctionCallInfo.Function);
            }

            [Fact]
            public void CheckResponseEvents()
            {
                Assert.Equal(this.expectedResponseEvents, this.testFunctionCallInfo.ResponseEvents);
            }

            #pragma warning disable xUnit1013 // Public method should be marked as test
            [MsbFunction]
            public void MsbFunction(FunctionCallInfo functionCallInfo)
            {
            }
            #pragma warning restore xUnit1013 // Public method should be marked as test

            private class MsbClientForTest : AbstractMsbClient
            {
                public override Task<bool> ConnectAsync()
                {
                    throw new NotImplementedException();
                }

                public override void Disconnect()
                {
                    throw new NotImplementedException();
                }

                public override bool IsConnected()
                {
                    throw new NotImplementedException();
                }

                public override Task<bool> PublishAsync(Service service, EventData eventData)
                {
                    throw new NotImplementedException();
                }

                public override Task<bool> RegisterAsync(Service serviceToRegister)
                {
                    throw new NotImplementedException();
                }
            }

            private class ServiceForTest : Service
            {
                public ServiceForTest(string uuid, string name, string description, string token)
                    : base(uuid, name, description, token)
                {
                }

                public override string Class => throw new NotImplementedException();
            }
        }
    }
}
