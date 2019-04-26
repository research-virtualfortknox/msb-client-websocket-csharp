// <copyright file="ConfigurationParameterValue.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using Fraunhofer.IPA.MSB.Client.API.Logging;
    using Fraunhofer.IPA.MSB.Client.API.OpenApi;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a parameter of an MSB <see cref="Configuration"/>.
    /// </summary>
    public class ConfigurationParameterValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationParameterValue"/> class.
        /// </summary>
        /// <param name="value">The <see cref="ConfigurationParameterValue.Value"/> of the <see cref="ConfigurationParameterValue"/>.</param>
        public ConfigurationParameterValue(object value)
        {
            this.Value = value;
        }

        /// <summary> Gets or sets value of the parameter.</summary>
        [JsonProperty("value")]
        public object Value { get; set; }

        /// <summary> Gets type of the parameter.</summary>
        [JsonProperty("type")]
        public string Type
        {
            get
            {
                var jsonSchemaOfParameterType = OpenApiMapper.GetJsonSchemaOfType(this.Value.GetType());
                return jsonSchemaOfParameterType.GetValue("type").ToString();
            }
        }

        /// <summary> Gets format of the parameter.</summary>
        [JsonProperty("format")]
        public string Format
        {
            get
            {
                var jsonSchemaOfParameterType = OpenApiMapper.GetJsonSchemaOfType(this.Value.GetType());
                try
                {
                    return jsonSchemaOfParameterType.GetValue("format").ToString();
                }
                catch (NullReferenceException)
                {
                    return null;
                }
            }
        }
    }
}
