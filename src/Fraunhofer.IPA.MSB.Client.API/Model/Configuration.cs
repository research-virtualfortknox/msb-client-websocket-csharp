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
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a configuration of a MSB service.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        public Configuration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        /// <param name="parameters">The parameters of the <see cref="Configuration"/></param>
        public Configuration(Dictionary<string, ConfigurationParameterValue> parameters)
        {
            this.Parameters = parameters;
        }

        /*/// <summary>
        /// Returns a new instance of the <see cref="Configuration"/> class based on parsing a file.
        /// </summary>
        /// <param name="path">The file to parse.</param>
        /// <returns>The parsed configuration.</returns>
        public static Configuration FromFile(string path)
        {
            Configuration r = null;

            try
            {
                var file = System.IO.File.ReadAllText(path);
                r = JsonConvert.DeserializeObject<Configuration>(file);
            }
            catch
            {
            }

            return r;
        }*/

        /// <summary>
        /// Returns a new instance of the <see cref="Configuration"/> class based on parsing a file.
        /// </summary>
        /// <param name="path">The file to parse.</param>
        public void FromFile(string path)
        {
            try
            {
                var file = System.IO.File.ReadAllText(path);
                var r = Newtonsoft.Json.Linq.JToken.Parse(file);

                this.Parameters = r["parameters"].ToObject<Dictionary<string, ConfigurationParameterValue>>();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Writes a serialised description of the <see cref="Configuration"/> object into a file.
        /// </summary>
        /// <param name="path">The target path.</param>
        public void ToFile(string path)
        {
            var s = JsonConvert.SerializeObject(this);

            try
            {
                System.IO.File.WriteAllText(path, s);
            }
            catch
            {
            }
        }

        /// <summary>Gets or sets the parameters of the configuration.</summary>
        [JsonProperty("parameters")]
        public Dictionary<string, ConfigurationParameterValue> Parameters { get; set; } = new Dictionary<string, ConfigurationParameterValue>();
    }
}
