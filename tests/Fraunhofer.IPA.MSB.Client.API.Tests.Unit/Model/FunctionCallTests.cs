// <copyright file="FunctionCallTests.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using Fraunhofer.IPA.MSB.Client.API.Model;
    using Newtonsoft.Json;
    using Xunit;

    public class FunctionCallTests
    {
        public FunctionCallTests()
        {
        }

        public class Deserialization : FunctionCallTests
        {
            private const string ExpectedServiceUuid = "76010f31-bb6b-47b0-bcde-1e7b42dd3d53";

            private const string ExpectedFunctionId = "SampleFunctionId";

            private const string ExpectedCorrelationId = "9d6d202f-df41-4ffb-9a51-c03a5eb4f512";

            private readonly Dictionary<string, object> expectedFunctionParameters = new Dictionary<string, object>()
                {
                    { "parameter1", "parameterValue1" },
                    { "parameter2", "parameterValue2" },
                };

            private readonly string serializedFunctionCall = $"{{ \"uuid\" : \"{ExpectedServiceUuid}\", \"functionId\" : \"{ExpectedFunctionId}\", \"correlationId\" : \"{ExpectedCorrelationId}\", \"functionParameters\" : {{ \"parameter1\" : \"parameterValue1\", \"parameter2\" : \"parameterValue2\" }} }}";

            private FunctionCall deserializedFunctionCall;

            public Deserialization()
            {
                this.deserializedFunctionCall = JsonConvert.DeserializeObject<FunctionCall>(this.serializedFunctionCall);
            }

            [Fact]
            public void CheckServiceUuid()
            {
                Assert.Equal(ExpectedServiceUuid, this.deserializedFunctionCall.ServiceUuid);
            }

            [Fact]
            public void CheckFunctionid()
            {
                Assert.Equal(ExpectedFunctionId, this.deserializedFunctionCall.FunctionId);
            }

            [Fact]
            public void CheckCorrelationId()
            {
                Assert.Equal(ExpectedCorrelationId, this.deserializedFunctionCall.CorrelationId);
            }

            [Fact]
            public void CheckFunctionParameter()
            {
                this.deserializedFunctionCall.FunctionParameters.Should().Equal(this.expectedFunctionParameters);
            }
        }
    }
}
