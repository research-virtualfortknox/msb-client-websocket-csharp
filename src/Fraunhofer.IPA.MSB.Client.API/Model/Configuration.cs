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
    using Fraunhofer.IPA.MSB.Client.API.Logging;
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

        /// <summary>Gets or sets the parameters of the configuration.</summary>
        [JsonProperty("parameters")]
        public Dictionary<string, ConfigurationParameterValue> Parameters { get; set; } = new Dictionary<string, ConfigurationParameterValue>();
    }
}
