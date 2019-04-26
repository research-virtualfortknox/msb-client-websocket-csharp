// <copyright file="ConfigurationTests.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Xunit;

    public class ConfigurationTests
    {
        public class Constructors : ConfigurationTests
        {
            [Fact]
            public void CheckCorrectSerialization()
            {
                Dictionary<string, ConfigurationParameterValue> configurationParameters = new Dictionary<string, ConfigurationParameterValue>();
                configurationParameters.Add("parameter1", new ConfigurationParameterValue("TestValue"));
                configurationParameters.Add("parameter2", new ConfigurationParameterValue(100));

                var configuration = new Configuration(configurationParameters);
                JObject actualJsonObject = JObject.Parse(JsonConvert.SerializeObject(configuration));

                actualJsonObject.Should().HaveElement("parameters");
                actualJsonObject.GetValue("parameters").Should().HaveElement("parameter1");
                actualJsonObject.GetValue("parameters").Should().HaveElement("parameter2");
            }
        }
    }
}
