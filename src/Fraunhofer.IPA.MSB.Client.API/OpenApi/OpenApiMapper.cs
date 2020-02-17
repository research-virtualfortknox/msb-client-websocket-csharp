// <copyright file="OpenApiMapper.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace Fraunhofer.IPA.MSB.Client.API.OpenApi
{
    using System;
    using System.Reflection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NJsonSchema;
    using NJsonSchema.Generation;
    using NJsonSchema.Generation.TypeMappers;

    /// <summary>
    /// Mapper to convert .NET types to OpenAPI schema.
    /// </summary>
    public class OpenApiMapper
    {
        /// <summary>
        /// Checks if a type is primitive.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True, if type is primitive.</returns>
        public static bool IsPrimitiveDataType(Type type)
        {
            return type.GetTypeInfo().IsPrimitive || type == typeof(decimal) || type == typeof(string) || type == typeof(DateTime);
        }

        /// <summary>
        /// Converts a type to a JSON schema in OpenAPI format.
        /// </summary>
        /// <param name="type">The type to be converted.</param>
        /// <returns>The type as JSON schema in OpenAPI format.</returns>
        public static JObject GetJsonSchemaOfType(Type type)
        {
            new PrimitiveTypeMapper(typeof(bool), (s) =>
            {
                s.Type = JsonObjectType.String;
            });

            JsonSchemaGeneratorSettings settings = new JsonSchemaGeneratorSettings
            {
                DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull,
                SchemaType = SchemaType.Swagger2,
                TypeMappers =
                {
                    new PrimitiveTypeMapper(typeof(char), (s) =>
                        {
                            s.Type = JsonObjectType.String;
                        }),
                    new PrimitiveTypeMapper(typeof(byte), (s) =>
                        {
                            s.Type = JsonObjectType.Integer;
                            s.Format = JsonFormatStrings.Integer;
                            s.Minimum = byte.MinValue;
                            s.Maximum = byte.MaxValue;
                        }),
                    new PrimitiveTypeMapper(typeof(sbyte), (s) =>
                        {
                            s.Type = JsonObjectType.Integer;
                            s.Format = JsonFormatStrings.Integer;
                            s.Minimum = sbyte.MinValue;
                            s.Maximum = sbyte.MaxValue;
                        }),
                    new PrimitiveTypeMapper(typeof(short), (s) =>
                        {
                            s.Type = JsonObjectType.Integer;
                            s.Format = JsonFormatStrings.Integer;
                            s.Minimum = short.MinValue;
                            s.Maximum = short.MaxValue;
                        }),
                    new PrimitiveTypeMapper(typeof(ushort), (s) =>
                        {
                            s.Type = JsonObjectType.Integer;
                            s.Format = JsonFormatStrings.Integer;
                            s.Minimum = ushort.MinValue;
                            s.Maximum = ushort.MaxValue;
                        }),
                    new PrimitiveTypeMapper(typeof(uint), (s) =>
                        {
                            s.Type = JsonObjectType.Integer;
                            s.Format = JsonFormatStrings.Long;
                            s.Minimum = uint.MinValue;
                            s.Maximum = uint.MaxValue;
                        }),
                    new PrimitiveTypeMapper(typeof(ulong), (s) =>
                        {
                            s.Type = JsonObjectType.Integer;
                            s.Format = JsonFormatStrings.Long;
                            s.Minimum = ulong.MinValue;
                            s.Maximum = ulong.MaxValue;
                        }),
                    new PrimitiveTypeMapper(typeof(float), (s) =>
                        {
                            s.Type = JsonObjectType.Number;
                            s.Format = JsonFormatStrings.Float;
                        }),
                    new PrimitiveTypeMapper(typeof(decimal), (s) =>
                        {
                            s.Type = JsonObjectType.Number;
                            s.Format = JsonFormatStrings.Double;
                        }),
                }
            };

            var schema = JsonSchema.FromType(type, settings);

            schema.Title = null;
            JObject deserializedSchema = JsonConvert.DeserializeObject(schema.ToJson()) as JObject;
            deserializedSchema.Remove("$schema");
            return deserializedSchema;
        }
    }
}
