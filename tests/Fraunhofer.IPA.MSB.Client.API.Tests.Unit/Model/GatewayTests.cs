// <copyright file="GatewayTests.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using System.Collections.Generic;
    using FluentAssertions;
    using FluentAssertions.Json;
    using Fraunhofer.IPA.MSB.Client.API.Model;
    using Newtonsoft.Json.Linq;
    using Xunit;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:Elements should be documented", Justification = "<Ausstehend>")]
    public class GatewayTests
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "<Ausstehend>")]
        protected Application testApplicationOfGateway = new Application("5c9d7ff2-8fb3-4f5f-af21-f024e96540e4", "ApplicationOfGateway", "Gateway Application Description", "9d1b9de8-18bd-4c49-ae84-2f83a61e9592");

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "<Ausstehend>")]
        protected SmartObject testSmartObjectOfGateway = new SmartObject("afeff37c-1abb-11eb-adc1-0242ac120002", "SmartObjectOfGateway", "Gateway SmartObject Description", "b2d04704-1abb-11eb-adc1-0242ac120002");

        public GatewayTests()
        {
            var gatewayServices = new List<Service>();
            gatewayServices.Add(this.testApplicationOfGateway);
            gatewayServices.Add(this.testSmartObjectOfGateway);

            this.TestGateway = new Gateway(this.ExpectedUuid, this.ExpectedName, this.ExpectedDescription, this.ExpectedToken, gatewayServices);
        }

        protected Gateway TestGateway { get; }

        protected string ExpectedUuid { get; } = "afdb80ac-1abd-11eb-adc1-0242ac120002";

        protected string ExpectedName { get; } = "Test Gateway";

        protected string ExpectedDescription { get; } = "Description of Test Gateway";

        protected string ExpectedToken { get; } = "b6f23200-1abd-11eb-adc1-0242ac120002";

        public class Constructor : GatewayTests
        {
            [Fact]
            public void CheckClass()
            {
                string expectedClass = "Gateway";
                Assert.Equal(expectedClass, this.TestGateway.Class);
            }

            [Fact]
            public void CheckUuid()
            {
                Assert.Equal(this.ExpectedUuid, this.TestGateway.Uuid);
            }

            [Fact]
            public void CheckName()
            {
                Assert.Equal(this.ExpectedName, this.TestGateway.Name);
            }

            [Fact]
            public void CheckDescription()
            {
                Assert.Equal(this.ExpectedDescription, this.TestGateway.Description);
            }

            [Fact]
            public void CheckToken()
            {
                Assert.Equal(this.ExpectedToken, this.TestGateway.Token);
            }
        }

        public class ToJson : GatewayTests
        {
            [Fact]
            public void CheckSerialization()
            {
                var gatewayJson = @"{
                    '@class':'Gateway',
                    'uuid':'afdb80ac-1abd-11eb-adc1-0242ac120002',
                    'name':'Test Gateway',
                    'description':'Description of Test Gateway',
                    'token':'b6f23200-1abd-11eb-adc1-0242ac120002',
                    'configuration':{
                        'parameters':{}
                    },
                    'events':[],
                    'functions':[],
                    'services':[
                        {
                            '@class':'Application',
                            'uuid':'5c9d7ff2-8fb3-4f5f-af21-f024e96540e4',
                            'name':'ApplicationOfGateway',
                            'description':'Gateway Application Description',
                            'token':'9d1b9de8-18bd-4c49-ae84-2f83a61e9592',
                            'configuration':{
                                'parameters':{}
                            },
                            'events':[],
                            'functions':[]
                        },
                        {
                            '@class':'SmartObject',
                            'uuid':'afeff37c-1abb-11eb-adc1-0242ac120002',
                            'name':'SmartObjectOfGateway',
                            'description':'Gateway SmartObject Description',
                            'token':'b2d04704-1abb-11eb-adc1-0242ac120002',
                            'configuration':{
                                'parameters':{}
                            },
                            'events':[],
                            'functions':[]
                        }
                    ]
                }";
                JObject expectedJsonObject = JObject.Parse(gatewayJson);
                JObject actualJsonObject = JObject.Parse(this.TestGateway.ToJson());
                actualJsonObject.Should().BeEquivalentTo(expectedJsonObject);
            }
        }

        public class Add_Remove_Service : GatewayTests
        {
            [Fact]
            public void AddService()
            {
                var newTestApplication = new Application("e15a20b0-1abe-11eb-adc1-0242ac120002", "Test Application", "Description", "e8a66504-1abe-11eb-adc1-0242ac120002");
                this.TestGateway.AddService(newTestApplication);

                this.TestGateway.Services.Should().Contain(newTestApplication);
            }

            [Fact]
            public void AddAlreadyAddedService()
            {
                this.TestGateway.AddService(this.testApplicationOfGateway);

                this.TestGateway.Services.Should().Contain(this.testApplicationOfGateway);
            }

            [Fact]
            public void RemoveService()
            {
                this.TestGateway.RemoveService(this.testApplicationOfGateway);

                this.TestGateway.Services.Should().NotContain(this.testApplicationOfGateway);
            }

            [Fact]
            public void RemoveNotExistingService()
            {
                var newTestApplication = new Application("e15a20b0-1abe-11eb-adc1-0242ac120002", "Test Application", "Description", "e8a66504-1abe-11eb-adc1-0242ac120002");
                this.TestGateway.RemoveService(newTestApplication);

                this.TestGateway.Services.Should().NotContain(newTestApplication);
            }
        }
    }
}
