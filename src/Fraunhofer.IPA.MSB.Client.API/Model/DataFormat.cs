// <copyright file="DataFormat.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace Fraunhofer.IPA.MSB.Client.Websocket.Model
{
    using System;
    using System.Collections.Generic;
    using Fraunhofer.IPA.MSB.Client.API.Model;
    using Fraunhofer.IPA.MSB.Client.API.OpenApi;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Defines the data format of an MSB <see cref="Event"/> or <see cref="Function"/>.
    /// </summary>
    public class DataFormat : Dictionary<string, object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataFormat"/> class.
        /// </summary>
        public DataFormat()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataFormat"/> class.
        /// </summary>
        /// <param name="dataFormatAsDictionary">DataFormat represented as Dictionary.</param>
        public DataFormat(Dictionary<string, object> dataFormatAsDictionary)
        {
            foreach (var entry in dataFormatAsDictionary)
            {
                this.Add(entry.Key, entry.Value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataFormat"/> class.
        /// </summary>
        /// <param name="dataFormatString">String which should be parsed to DataFormat.</param>
        public DataFormat(string dataFormatString)
        {
            var dataFormatAsDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(dataFormatString);

            foreach (var entry in dataFormatAsDictionary)
            {
                this.Add(entry.Key, entry.Value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataFormat"/> class.
        /// </summary>
        /// <param name="rootNodeName">Name of the root node.</param>
        /// <param name="type">Type to convert to data format.</param>
        public DataFormat(string rootNodeName, Type type)
        {
            if (type == null)
            {
                this.Add(rootNodeName, new Dictionary<string, object>());
            }
            else
            {
                var deserializedSchema = OpenApiMapper.GetJsonSchemaOfType(type);

                if (OpenApiMapper.IsPrimitiveDataType(type) || (type.IsArray && OpenApiMapper.IsPrimitiveDataType(type.GetElementType())))
                {
                    this.Add(rootNodeName, deserializedSchema.ToObject<Dictionary<string, object>>());
                }
                else
                {
                    string mainTypeName = type.Name.Replace("[]", string.Empty);
                    this.Add(rootNodeName, new Dictionary<string, string>() { { "$ref", $"#/definitions/{mainTypeName}" } });

                    // Move definitions to root level
                    if (deserializedSchema["definitions"] != null)
                    {
                        foreach (var defintion in deserializedSchema["definitions"].ToObject<Dictionary<string, JObject>>())
                        {
                            (defintion.Value as JObject).Remove("additionalProperties");
                            this.Add(defintion.Key, defintion.Value as JObject);
                        }
                    }

                    deserializedSchema.Remove("definitions");
                    this.Add(mainTypeName, deserializedSchema.ToObject<Dictionary<string, object>>());
                }
            }
        }
    }
}
