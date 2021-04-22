// <copyright file="DictionaryConverter.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace Fraunhofer.IPA.MSB.Client.API.Utils
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Custom JSON converter for dictionaries.
    /// </summary>
    public class DictionaryConverter : JsonConverter
    {
        /// <summary>
        /// Write JSON.
        /// </summary>
        /// <param name="writer">JsonWriter.</param>
        /// <param name="value">Value to write.</param>
        /// <param name="serializer">JsonSerializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            this.WriteValue(writer, value);
        }

        /// <summary>
        /// Read JSON.
        /// </summary>
        /// <param name="reader">JsonReader.</param>
        /// <param name="objectType">Type of object.</param>
        /// <param name="existingValue">Value of object.</param>
        /// <param name="serializer">JsonSerializer.</param>
        /// <returns>Read JSON as object.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return this.ReadValue(reader);
        }

        /// <summary>
        /// Gets if a type can be converted.
        /// </summary>
        /// <param name="objectType">Type to be converted.</param>
        /// <returns>True, if type can be converted.</returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(IDictionary<string, object>).IsAssignableFrom(objectType);
        }

        private void WriteValue(JsonWriter writer, object value)
        {
            var t = JToken.FromObject(value);
            switch (t.Type)
            {
                case JTokenType.Object:
                    this.WriteObject(writer, value);
                    break;
                case JTokenType.Array:
                    this.WriteArray(writer, value);
                    break;
                default:
                    writer.WriteValue(value);
                    break;
            }
        }

        private void WriteObject(JsonWriter writer, object value)
        {
            writer.WriteStartObject();
            var obj = value as IDictionary<string, object>;
            foreach (var kvp in obj)
            {
                writer.WritePropertyName(kvp.Key);
                this.WriteValue(writer, kvp.Value);
            }

            writer.WriteEndObject();
        }

        private void WriteArray(JsonWriter writer, object value)
        {
            writer.WriteStartArray();
            var array = value as IEnumerable<object>;
            foreach (var o in array)
            {
                this.WriteValue(writer, o);
            }

            writer.WriteEndArray();
        }

        private object ReadValue(JsonReader reader)
        {
            while (reader.TokenType == JsonToken.Comment)
            {
                if (!reader.Read())
                {
                    throw new JsonSerializationException("Unexpected Token when converting IDictionary<string, object>");
                }
            }

            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    return this.ReadObject(reader);
                case JsonToken.StartArray:
                    return this.ReadArray(reader);
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.String:
                case JsonToken.Boolean:
                case JsonToken.Undefined:
                case JsonToken.Null:
                case JsonToken.Date:
                case JsonToken.Bytes:
                    return reader.Value;
                default:
                    throw new JsonSerializationException(
                        string.Format("Unexpected token when converting IDictionary<string, object>: {0}", reader.TokenType));
            }
        }

        private object ReadArray(JsonReader reader)
        {
            IList<object> list = new List<object>();

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.Comment:
                        break;
                    default:
                        var v = this.ReadValue(reader);

                        list.Add(v);
                        break;
                    case JsonToken.EndArray:
                        return list;
                }
            }

            throw new JsonSerializationException("Unexpected end when reading IDictionary<string, object>");
        }

        private object ReadObject(JsonReader reader)
        {
            var obj = new Dictionary<string, object>();

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        var propertyName = reader.Value.ToString();

                        if (!reader.Read())
                        {
                            throw new JsonSerializationException("Unexpected end when reading IDictionary<string, object>");
                        }

                        var v = this.ReadValue(reader);

                        obj[propertyName] = v;
                        break;
                    case JsonToken.Comment:
                        break;
                    case JsonToken.EndObject:
                        return obj;
                }
            }

            throw new JsonSerializationException("Unexpected end when reading IDictionary<string, object>");
        }
    }
}
