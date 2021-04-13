// <copyright file="Configuration.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace Fraunhofer.IPA.MSB.Client.API.Model
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Fraunhofer.IPA.MSB.Client.API.Exceptions;
    using Fraunhofer.IPA.MSB.Client.API.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a configuration of a MSB service.
    /// </summary>
    public class Configuration
    {
        private static readonly ILog Log = LogProvider.For<Configuration>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        public Configuration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        /// <param name="parameters">The parameters of the <see cref="Configuration"/>.</param>
        public Configuration(Dictionary<string, ConfigurationParameterValue> parameters)
            : this()
        {
            this.Parameters = parameters;
        }

        /// <summary>Gets or sets the parameters of the configuration.</summary>
        [JsonProperty("parameters")]
        public Dictionary<string, ConfigurationParameterValue> Parameters { get; set; } = new Dictionary<string, ConfigurationParameterValue>();

        /// <summary>
        /// Returns a new instance of the <see cref="Configuration"/> parsing a file.
        /// </summary>
        /// <param name="path">The file to parse.</param>
        /// <returns>The parsed configuration.</returns>
        public static Configuration FromFile(string path)
        {
            try
            {
                var configurationFile = File.ReadAllText(path);
                var loadedConfiguration = JsonConvert.DeserializeObject<Configuration>(configurationFile);

                return loadedConfiguration;
            }
            catch (Exception e)
            {
                Log.Error($"Failed to create configuration from file: {e.Message}");
                throw new ConfigurationPersistException($"Failed to create configuration from file: {e.Message}");
            }
        }

        /// <summary>
        /// Loads the <see cref="Configuration.Parameters"/> parsing a file.
        /// </summary>
        /// <param name="path">The file to parse.</param>
        public void LoadFromFile(string path)
        {
            try
            {
                var configurationFile = File.ReadAllText(path);
                var loadedConfiguration = Newtonsoft.Json.Linq.JToken.Parse(configurationFile);

                this.Parameters = loadedConfiguration["parameters"].ToObject<Dictionary<string, ConfigurationParameterValue>>();
            }
            catch (Exception e)
            {
                Log.Error($"Failed to load configuration from file: {e.Message}");
                throw new ConfigurationPersistException($"Failed to load configuration from file: {e.Message}");
            }
        }

        /// <summary>
        /// Writes the <see cref="Configuration"/> serialized into a file.
        /// </summary>
        /// <param name="path">The target path.</param>
        public void SaveToFile(string path)
        {
            var configurationAsJsonString = JsonConvert.SerializeObject(this);

            try
            {
                var configFileDirectory = Path.GetDirectoryName(path);
                Directory.CreateDirectory(configFileDirectory);
                File.WriteAllText(path, configurationAsJsonString);
            }
            catch (Exception e)
            {
                Log.Error($"Failed to save configuration to file: {e.Message}");
                throw new ConfigurationPersistException($"Failed to save configuration to file: {e.Message}");
            }
        }
    }
}
