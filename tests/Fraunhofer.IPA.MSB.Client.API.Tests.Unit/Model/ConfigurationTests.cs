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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using FluentAssertions;
    using FluentAssertions.Json;
    using Fraunhofer.IPA.MSB.Client.API.Exceptions;
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
                Dictionary<string, ConfigurationParameterValue> configurationParameters = new Dictionary<string, ConfigurationParameterValue>
                {
                    { "parameter1", new ConfigurationParameterValue("TestValue") },
                    { "parameter2", new ConfigurationParameterValue(100) }
                };

                var configuration = new Configuration(configurationParameters);
                JObject actualJsonObject = JObject.Parse(JsonConvert.SerializeObject(configuration));

                actualJsonObject.Should().HaveElement("parameters");
                actualJsonObject.GetValue("parameters").Should().HaveElement("parameter1");
                actualJsonObject.GetValue("parameters").Should().HaveElement("parameter2");
            }

            public class SaveToFile : ConfigurationTests
            {
                [Fact]
                public void Save()
                {
                    Dictionary<string, ConfigurationParameterValue> configurationParameters = new Dictionary<string, ConfigurationParameterValue>
                    {
                        { "parameter1", new ConfigurationParameterValue("TestValue") },
                        { "parameter2", new ConfigurationParameterValue(100) }
                    };

                    var configuration = new Configuration(configurationParameters);
                    var targetFilePath = Path.GetTempFileName();
                    configuration.SaveToFile(targetFilePath);

                    var readFileContent = File.ReadAllText(targetFilePath);
                    JObject actualJsonObject = JObject.Parse(readFileContent);
                    actualJsonObject.Should().HaveElement("parameters");
                    actualJsonObject.GetValue("parameters").Should().HaveElement("parameter1");
                    actualJsonObject.GetValue("parameters").Should().HaveElement("parameter2");
                }

                [Fact]
                public void PathNotValid()
                {
                    var exception = Assert.Throws<ConfigurationPersistException>(() =>
                    {
                        var configurationFilePath = string.Empty;
                        var testConfiguration = new Configuration();
                        testConfiguration.SaveToFile(configurationFilePath);
                    });
                    Assert.Contains("Failed to save configuration to file", exception.Message);
                }
            }

            public class LoadFromFile : ConfigurationTests
            {
                [Fact]
                public void Load()
                {
                    Dictionary<string, ConfigurationParameterValue> configurationParameters = new Dictionary<string, ConfigurationParameterValue>
                    {
                        { "parameter1", new ConfigurationParameterValue("TestValue") },
                        { "parameter2", new ConfigurationParameterValue(100) }
                    };

                    var configuration = new Configuration(configurationParameters);
                    var serializedConfiguration = JsonConvert.SerializeObject(configuration);
                    var targetFilePath = Path.GetTempFileName();
                    File.WriteAllText(targetFilePath, serializedConfiguration);

                    var testConfiguration = new Configuration();
                    testConfiguration.LoadFromFile(targetFilePath);
                    testConfiguration.Parameters.Should().ContainKey("parameter1");
                    testConfiguration.Parameters.Should().ContainKey("parameter2");
                }

                [Fact]
                public void FileNotExistent()
                {
                    var exception = Assert.Throws<ConfigurationPersistException>(() =>
                    {
                        var configurationFilePath = string.Empty;
                        var testConfiguration = new Configuration();
                        testConfiguration.LoadFromFile(configurationFilePath);
                    });
                    Assert.Contains("Failed to load configuration from file", exception.Message);
                }
            }

            public class FromFile : ConfigurationTests
            {
                [Fact]
                public void CreateFromFile()
                {
                    Dictionary<string, ConfigurationParameterValue> configurationParameters = new Dictionary<string, ConfigurationParameterValue>
                    {
                        { "parameter1", new ConfigurationParameterValue("TestValue") },
                        { "parameter2", new ConfigurationParameterValue(100) }
                    };

                    var configuration = new Configuration(configurationParameters);
                    var serializedConfiguration = JsonConvert.SerializeObject(configuration);
                    var targetFilePath = Path.GetTempFileName();
                    File.WriteAllText(targetFilePath, serializedConfiguration);

                    var testConfiguration = Configuration.FromFile(targetFilePath);
                    testConfiguration.Parameters.Should().ContainKey("parameter1");
                    testConfiguration.Parameters.Should().ContainKey("parameter2");
                }

                [Fact]
                public void FileNotExistent()
                {
                    var exception = Assert.Throws<ConfigurationPersistException>(() =>
                    {
                        var configurationFilePath = string.Empty;
                        var testConfiguration = Configuration.FromFile(configurationFilePath);
                    });
                    Assert.Contains("Failed to create configuration from file", exception.Message);
                }
            }
        }
    }
}
