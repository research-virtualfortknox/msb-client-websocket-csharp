// <copyright file="ServiceTests.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using System.Reflection;
    using FluentAssertions;
    using FluentAssertions.Json;
    using Fraunhofer.IPA.MSB.Client.API.Attributes;
    using Fraunhofer.IPA.MSB.Client.API.Exceptions;
    using Fraunhofer.IPA.MSB.Client.API.Model;
    using Fraunhofer.IPA.MSB.Client.Tests.Shared;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Xunit;
    using Xunit.Abstractions;

    public class ServiceTests : BaseTests
    {
        protected const string Uuid = "df00ce10-9443-416f-8a42-e388a7d76dbe";

        protected const string Name = "Test Application";

        protected const string Description = "Description of Test Application";

        protected const string Token = "31f7bcaff-2753-4cd6-811d-30397a45318f";

        public ServiceTests()
        {
        }

        public ServiceTests(ITestOutputHelper output)
            : base(output)
        {
        }

        protected TestImplementationOfServiceClass TestService { get; set; }

        public class TestImplementationOfServiceClass : Service
        {
            public TestImplementationOfServiceClass(string uuid, string name, string description, string token)
                : base(uuid, name, description, token)
            {
            }

            public override string Class => "TestImplementationOfServiceClass";
        }

        public class Constructor : ServiceTests
        {
            public Constructor()
            {
                this.TestService = new TestImplementationOfServiceClass(Uuid, Name, Description, Token);
            }

            [Fact]
            public void CheckUuid()
            {
                Assert.Equal(Uuid, this.TestService.Uuid);
            }

            [Fact]
            public void CheckName()
            {
                Assert.Equal(Name, this.TestService.Name);
            }

            [Fact]
            public void CheckDescription()
            {
                Assert.Equal(Description, this.TestService.Description);
            }

            [Fact]
            public void CheckToken()
            {
                Assert.Equal(Token, this.TestService.Token);
            }
        }

        public class ToJson : ServiceTests
        {
            public ToJson()
            {
                this.TestService = new TestImplementationOfServiceClass(Uuid, Name, Description, Token);
            }

            [Fact]
            public void CheckSerialization()
            {
                var serviceJson = @"{
                    '@class':'TestImplementationOfServiceClass',
                    'name':'Test Application',
                    'description':'Description of Test Application',
                    'token':'31f7bcaff-2753-4cd6-811d-30397a45318f',
                    'uuid':'df00ce10-9443-416f-8a42-e388a7d76dbe',
                    'configuration':{
                        'parameters':{}
                    },
                    'events':[],
                    'functions':[]
                }";
                JObject expectedJsonObject = JObject.Parse(serviceJson);
                JObject actualJsonObject = JObject.Parse(this.TestService.ToJson());
                actualJsonObject.Should().BeEquivalentTo(expectedJsonObject);
            }
        }

        public class AddEvent_RemoveEvent : ServiceTests
        {
            private Event testEvent = new Event("EventId", "EventName", "Description", typeof(int));

            public AddEvent_RemoveEvent()
            {
            }

            [Fact]
            public void AddEvent()
            {
                this.TestService = new TestImplementationOfServiceClass(Uuid, Name, Description, Token);
                Event testEvent = new Event("EventId", "EventName", "Description", typeof(int));
                this.TestService.AddEvent(testEvent);
                Assert.Contains(testEvent, this.TestService.Events);
            }

            [Fact]
            public void AddEventRaw()
            {
                this.TestService = new TestImplementationOfServiceClass(Uuid, Name, Description, Token);
                Event expectedEvent = new Event("EventId", "EventName", "Description", typeof(string));

                var eventAsJsonString = @"{
                    'eventId': 'EventId',
                    'name': 'EventName',
                    'description': 'Description',
                    'dataFormat': {
                        'dataObject': {
                          'type': 'string'
                        }
                    }
                }";
                this.TestService.AddEventRaw(eventAsJsonString);

                var actualEventString = JsonConvert.SerializeObject(this.TestService.Events[0]);
                var expectedEventString = JsonConvert.SerializeObject(expectedEvent);

                Assert.Equal(expectedEventString, actualEventString);
            }

            [Fact]
            public void RemoveEvent()
            {
                this.AddEvent();
                this.TestService.RemoveEvent(this.testEvent);
                Assert.DoesNotContain(this.testEvent, this.TestService.Events);
            }
        }

        public class AddFunction_RemoveFunction : ServiceTests
        {
            private Function testFunctionWithoutResponseEvent;
            private Function testFunctionWithResponseEvent;
            private Event responseEventOfFcuntion;

            public AddFunction_RemoveFunction()
            {
                this.TestService = new TestImplementationOfServiceClass(Uuid, Name, Description, Token);

                MethodInfo methodInfo = this.GetType().GetRuntimeMethod("MsbFunctionWithoutResponseEvent", new Type[] { typeof(FunctionCallInfo) });
                this.testFunctionWithoutResponseEvent = new Function("MsbFunctionWithoutResponseEvent", "MsbFunctionWithoutResponseEvent", "Description", methodInfo, this);

                MethodInfo methodInfo2 = this.GetType().GetRuntimeMethod("MsbFunctionWithResponseEvent", new Type[] { typeof(FunctionCallInfo) });
                this.testFunctionWithResponseEvent = new Function("MsbFunctionWithResponseEvent", "MsbFunctionWithResponseEvent", "Description", methodInfo2, this);

                this.responseEventOfFcuntion = new Event("ResponseEvent1", "Response Event 1", "Description", typeof(string));
            }

            [Fact]
            public void AddFunctionWithoutResponseEvent()
            {
                this.TestService.AddFunction(this.testFunctionWithoutResponseEvent);
                this.TestService.Functions.Should().Contain(this.testFunctionWithoutResponseEvent);
            }

            [Fact]
            public void AddFunctionWithResponseEvent()
            {
                this.TestService.AddEvent(this.responseEventOfFcuntion);
                this.TestService.AddFunction(this.testFunctionWithResponseEvent);
                this.TestService.Functions.Should().Contain(this.testFunctionWithResponseEvent);
            }

            [Fact]
            public void AddFunctionWithResponseEventButResponseEventNotAddedToService()
            {
                Exception ex = Assert.Throws<ResponseEventNotFoundException>(() => this.TestService.AddFunction(this.testFunctionWithResponseEvent));
            }

            [Fact]
            public void RemoveFunction()
            {
                this.AddFunctionWithoutResponseEvent();
                this.TestService.RemoveFunction(this.testFunctionWithoutResponseEvent);
                this.TestService.Functions.Should().NotContain(this.testFunctionWithoutResponseEvent);
            }

#pragma warning disable xUnit1013 // Public method should be marked as test
            [MsbFunction]
            public void MsbFunctionWithoutResponseEvent(FunctionCallInfo functionCallInfo)
            {
            }

            [MsbFunction(ResponseEvents = new string[] { "ResponseEvent1" })]
            public EventData MsbFunctionWithResponseEvent(FunctionCallInfo functionCallInfo)
            {
                return new EventDataBuilder(functionCallInfo.ResponseEvents["ResponseEvent1"]).Build();
            }
#pragma warning restore xUnit1013 // Public method should be marked as test
        }

        public class AddFunctionHandler_RemoveFunctionHandler : ServiceTests
        {
            private SimpleFunctionHandler simpleFunctionHandler = new SimpleFunctionHandler();

            public AddFunctionHandler_RemoveFunctionHandler()
            {
                this.TestService = new TestImplementationOfServiceClass(Uuid, Name, Description, Token);
            }

            [Fact]
            public void AddFunctionHandler()
            {
                this.TestService.AddFunctionHandler(this.simpleFunctionHandler);
                this.TestService.Functions.Should().HaveCount(2);
                Assert.NotNull(this.TestService.GetFunctionById("SimpleFunctionHandlerId/SimpleFunctionWithNoParameters"));
                Assert.NotNull(this.TestService.GetFunctionById("SimpleFunctionHandlerId/SimpleFunctionWithParameters"));
            }

            [Fact]
            public void AddFunctionHandlerWithoutDefinedId()
            {
                this.TestService.AddFunctionHandler(new SimpleFunctionHandlerWithoutId());
                this.TestService.Functions.Should().HaveCount(1);
                Assert.Equal("SimpleFunctionHandlerWithoutId/SimpleFunction", this.TestService.Functions[0].Id);
            }

            [Fact]
            public void AddFunctionHandlerWithoutAttribute()
            {
                this.TestService.AddFunctionHandler(new SimpleFunctionHandlerWithoutAttribute());
                this.TestService.Functions.Should().HaveCount(1);
                Assert.Equal("SimpleFunctionHandlerWithoutAttribute/SimpleFunction", this.TestService.Functions[0].Id);
            }

            [Fact]
            public void ReAddFunctionHanlder()
            {
                this.AddFunctionHandler();
                this.AddFunctionHandler();
            }

            [Fact]
            public void RemoveFunctionHandler()
            {
                this.AddFunctionHandler();
                this.TestService.RemoveFunctionHandler(this.simpleFunctionHandler);
                this.TestService.Functions.Should().HaveCount(0);
            }

            [Fact]
            public void RemoveNoneExistingFunctionHandler()
            {
                this.TestService.Functions.Should().HaveCount(0);
                this.TestService.RemoveFunctionHandler(this.simpleFunctionHandler);
                this.TestService.Functions.Should().HaveCount(0);
            }

            [MsbFunctionHandler(Id = "SimpleFunctionHandlerId")]
            private class SimpleFunctionHandler : AbstractFunctionHandler
            {
                [MsbFunction]
                public void SimpleFunctionWithNoParameters(FunctionCallInfo functionCallInfo)
                {
                }

                [MsbFunction]
                public void SimpleFunctionWithParameters(
                    [MsbFunctionParameter(Name = "param1")] string testParameter1,
                    [MsbFunctionParameter(Name = "param2")] int testParameter2,
                    [MsbFunctionParameter(Name = "param3")] DateTime testParameter3,
                    FunctionCallInfo functionCallInfo)
                {
                }
            }

            [MsbFunctionHandler]
            private class SimpleFunctionHandlerWithoutId : AbstractFunctionHandler
            {
                [MsbFunction]
                public void SimpleFunction(FunctionCallInfo functionCallInfo)
                {
                }
            }

            private class SimpleFunctionHandlerWithoutAttribute : AbstractFunctionHandler
            {
                [MsbFunction]
                public void SimpleFunction(FunctionCallInfo functionCallInfo)
                {
                }
            }
        }

        public class AddConfigurationParaemter_RemoveConfigurationParameter : ServiceTests
        {
            private readonly string expectedParameterName = "testParameter";
            private readonly ConfigurationParameterValue expectedConfigurationParameterValue = new ConfigurationParameterValue("testValue");

            public AddConfigurationParaemter_RemoveConfigurationParameter()
            {
                this.TestService = new TestImplementationOfServiceClass(Uuid, Name, Description, Token);
            }

            [Fact]
            public void AddConfigurationParameter()
            {
                this.TestService.AddConfigurationParameter(this.expectedParameterName, this.expectedConfigurationParameterValue);
                this.TestService.Configuration.Parameters.Should().Contain(new System.Collections.Generic.KeyValuePair<string, ConfigurationParameterValue>(this.expectedParameterName, this.expectedConfigurationParameterValue));
            }

            [Fact]
            public void RemoveConfigurationParameter()
            {
                this.AddConfigurationParameter();
                this.TestService.RemoveConfigurationParameter(this.expectedParameterName);
                this.TestService.Configuration.Parameters.Should().NotContainKey(this.expectedParameterName);
            }
        }
    }
}
