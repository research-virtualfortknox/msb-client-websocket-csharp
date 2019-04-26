// <copyright file="OpenApiMapperTests.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace Fraunhofer.IPA.MSB.Client.API.Tests.Unit.OpenApi
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using FluentAssertions.Json;
    using Fraunhofer.IPA.MSB.Client.API.OpenApi;
    using Fraunhofer.IPA.MSB.Client.Tests.Shared;
    using Fraunhofer.IPA.MSB.Client.Websocket.IntegrationTest.Events;
    using Newtonsoft.Json.Linq;
    using Xunit;
    using Xunit.Abstractions;

    public class OpenApiMapperTests : BaseTests
    {
        public OpenApiMapperTests(ITestOutputHelper output)
            : base(output)
        {
        }

        public class GetJsonSchemaOfType : OpenApiMapperTests
        {
            public GetJsonSchemaOfType(ITestOutputHelper output)
                : base(output)
            {
            }

            private enum SampleEnum
            {
                EnumValue1,
                EnumValue2,
                Enumvalue3
            }

            /// <summary>
            /// Gets test data for <see cref="DataFormatTests.Constructors.DataFormatGenerationFromType(Type, JObject)"/>
            /// </summary>
            public static IEnumerable<object[]> ExpectedSchemas =>
                new List<object[]>
                {
                    new object[] { typeof(bool), JObject.Parse("{ \"type\": \"boolean\" }") },
                    new object[] { typeof(byte), JObject.Parse("{ \"type\": \"integer\", \"format\": \"int32\", \"maximum\": 255.0, \"minimum\": 0.0 }") },
                    new object[] { typeof(sbyte), JObject.Parse("{ \"type\": \"integer\", \"format\": \"int32\", \"maximum\": 127.0, \"minimum\": -128.0 }") },
                    new object[] { typeof(char), JObject.Parse("{ \"type\": \"string\" }") },
                    new object[] { typeof(string), JObject.Parse("{ \"type\": \"string\" }") },
                    new object[] { typeof(DateTime), JObject.Parse("{ \"type\": \"string\", \"format\": \"date-time\"}") },
                    new object[] { typeof(decimal), JObject.Parse("{ \"type\": \"number\", \"format\": \"double\" }") },
                    new object[] { typeof(double), JObject.Parse("{ \"type\": \"number\", \"format\": \"double\" }") },
                    new object[] { typeof(float), JObject.Parse("{ \"type\": \"number\", \"format\": \"float\" }") },
                    new object[] { typeof(short), JObject.Parse("{ \"type\": \"integer\", \"format\": \"int32\", \"maximum\": 32767.0, \"minimum\": -32768.0 }") },
                    new object[] { typeof(ushort), JObject.Parse("{ \"type\": \"integer\", \"format\": \"int32\", \"maximum\": 65535.0, \"minimum\": 0.0 }") },
                    new object[] { typeof(int), JObject.Parse("{ \"type\": \"integer\", \"format\": \"int32\" }") },
                    new object[] { typeof(uint), JObject.Parse("{ \"type\": \"integer\", \"format\": \"int64\", \"maximum\": 4294967295.0, \"minimum\": 0.0 }") },
                    new object[] { typeof(long), JObject.Parse("{ \"type\": \"integer\", \"format\": \"int64\" }") },
                    new object[] { typeof(ulong), JObject.Parse("{ \"type\": \"integer\", \"format\": \"int64\", \"maximum\": 1.8446744073709552E+19, \"minimum\": 0.0 }") },
                    new object[] { typeof(List<bool>), JObject.Parse("{ \"type\": \"array\", \"items\": { \"type\": \"boolean\" } }") },
                    new object[] { typeof(List<byte>), JObject.Parse("{ \"type\": \"array\", \"items\": { \"type\": \"integer\", \"format\": \"int32\", \"maximum\": 255.0, \"minimum\": 0.0 } }") },
                    new object[] { typeof(List<sbyte>), JObject.Parse("{ \"type\": \"array\", \"items\": { \"type\": \"integer\", \"format\": \"int32\", \"maximum\": 127.0, \"minimum\": -128.0 } }") },
                    new object[] { typeof(List<char>), JObject.Parse("{ \"type\": \"array\", \"items\": { \"type\": \"string\" } }") },
                    new object[] { typeof(List<string>), JObject.Parse("{ \"type\": \"array\", \"items\": { \"type\": \"string\" } }") },
                    new object[] { typeof(List<DateTime>), JObject.Parse("{ \"type\": \"array\", \"items\": { \"type\": \"string\", \"format\": \"date-time\" } }") },
                    new object[] { typeof(List<decimal>), JObject.Parse("{ \"type\": \"array\", \"items\": { \"type\": \"number\", \"format\": \"double\" } } ") },
                    new object[] { typeof(List<double>), JObject.Parse("{ \"type\": \"array\", \"items\": { \"type\": \"number\", \"format\": \"double\" } }") },
                    new object[] { typeof(List<float>), JObject.Parse("{ \"type\": \"array\", \"items\": { \"type\": \"number\", \"format\": \"float\" } } ") },
                    new object[] { typeof(List<short>), JObject.Parse("{ \"type\": \"array\", \"items\": { \"type\": \"integer\", \"format\": \"int32\", \"maximum\": 32767.0, \"minimum\": -32768.0 } }") },
                    new object[] { typeof(List<ushort>), JObject.Parse("{ \"type\": \"array\", \"items\": { \"type\": \"integer\", \"format\": \"int32\", \"maximum\": 65535.0, \"minimum\": 0.0 } }") },
                    new object[] { typeof(List<int>), JObject.Parse("{ \"type\": \"array\", \"items\": { \"type\": \"integer\", \"format\": \"int32\" } }") },
                    new object[] { typeof(List<uint>), JObject.Parse("{ \"type\": \"array\", \"items\": { \"type\": \"integer\", \"format\": \"int64\", \"maximum\": 4294967295.0, \"minimum\": 0.0 } }") },
                    new object[] { typeof(List<long>), JObject.Parse("{ \"type\": \"array\", \"items\": { \"type\": \"integer\", \"format\": \"int64\" } }") },
                    new object[] { typeof(List<ulong>), JObject.Parse("{ \"type\": \"array\", \"items\": { \"type\": \"integer\", \"format\": \"int64\", \"maximum\": 1.8446744073709552E+19, \"minimum\": 0.0 } }") },
                    new object[] { typeof(bool[]), JObject.Parse("{ \"type\": \"array\", \"items\": { \"type\": \"boolean\" } }") },
                    new object[] { typeof(byte[]), JObject.Parse("{ \"type\": \"string\", \"format\": \"byte\" }") },
                    new object[] { typeof(sbyte[]), JObject.Parse("{ \"type\": \"array\", \"items\": { \"type\": \"integer\", \"format\": \"int32\", \"maximum\": 127.0, \"minimum\": -128.0 } }") },
                    new object[] { typeof(char[]), JObject.Parse("{ \"type\": \"array\", \"items\": { \"type\": \"string\" } }") },
                    new object[] { typeof(string[]), JObject.Parse("{ \"type\": \"array\", \"items\": { \"type\": \"string\" } }") },
                    new object[] { typeof(DateTime[]), JObject.Parse("{ \"type\": \"array\", \"items\": { \"type\": \"string\", \"format\": \"date-time\" } }") },
                    new object[] { typeof(decimal[]), JObject.Parse("{ \"type\": \"array\", \"items\": { \"type\": \"number\", \"format\": \"double\" } } ") },
                    new object[] { typeof(double[]), JObject.Parse("{ \"type\": \"array\", \"items\": { \"type\": \"number\", \"format\": \"double\" } }") },
                    new object[] { typeof(float[]), JObject.Parse("{ \"type\": \"array\", \"items\": { \"type\": \"number\", \"format\": \"float\" } } ") },
                    new object[] { typeof(short[]), JObject.Parse("{ \"type\": \"array\", \"items\": { \"type\": \"integer\", \"format\": \"int32\", \"maximum\": 32767.0, \"minimum\": -32768.0 } }") },
                    new object[] { typeof(ushort[]), JObject.Parse("{ \"type\": \"array\", \"items\": { \"type\": \"integer\", \"format\": \"int32\", \"maximum\": 65535.0, \"minimum\": 0.0 } }") },
                    new object[] { typeof(int[]), JObject.Parse("{ \"type\": \"array\", \"items\": { \"type\": \"integer\", \"format\": \"int32\" } }") },
                    new object[] { typeof(uint[]), JObject.Parse("{ \"type\": \"array\", \"items\": { \"type\": \"integer\", \"format\": \"int64\", \"maximum\": 4294967295.0, \"minimum\": 0.0 } }") },
                    new object[] { typeof(long[]), JObject.Parse("{ \"type\": \"array\", \"items\": { \"type\": \"integer\", \"format\": \"int64\" } }") },
                    new object[] { typeof(ulong[]), JObject.Parse("{ \"type\": \"array\", \"items\": { \"type\": \"integer\", \"format\": \"int64\", \"maximum\": 1.8446744073709552E+19, \"minimum\": 0.0 } }") },
                    new object[] { typeof(Dictionary<string, string>), JObject.Parse("{ \"type\": \"object\", \"additionalProperties\": { \"type\": \"string\" } } ") },
                    new object[] { typeof(Dictionary<string, int>), JObject.Parse("{ \"type\": \"object\", \"additionalProperties\": { \"type\": \"integer\", \"format\": \"int32\" } } ") },
                    new object[] { typeof(SampleEnum), JObject.Parse("{ \"type\": \"integer\", \"description\": \"\", \"x-enumNames\": [ \"EnumValue1\", \"EnumValue2\", \"Enumvalue3\"], \"enum\": [0, 1, 2 ] } ") },
                    new object[] { typeof(SimpleEvent), JObject.Parse("{ \"type\": \"object\", \"additionalProperties\": false, \"required\": [ \"MyInteger\", \"MyFloat\", \"MyDouble\", \"MyString\", \"MyStringArray\" ], \"properties\": { \"MyInteger\": { \"type\": \"integer\", \"format\": \"int32\" }, \"MyFloat\": { \"type\": \"number\", \"format\": \"float\" }, \"MyDouble\": { \"type\": \"number\", \"format\": \"double\" }, \"MyString\": { \"type\": \"string\" }, \"MyStringArray\": { \"type\": \"array\", \"items\": { \"type\": \"string\" } } } }") },
                    new object[] { typeof(AllDataTypesEvent), JObject.Parse("{ \"type\": \"object\", \"additionalProperties\": false, \"required\": [ \"SimpleBool\", \"SimpleString\", \"SimpleChar\", \"SimpleByte\", \"SimpleSByte\", \"SimpleInt16\", \"SimpleUInt16\", \"SimpleInt32\", \"SimpleUInt32\", \"SimpleInt64\", \"SimpleUInt64\", \"SimpleFloat\", \"SimpleDouble\", \"SimpleDecimal\", \"SimpleDateTime\", \"StringArray\", \"ByteArray\", \"StringList\", \"DictionaryStringDouble\" ], \"properties\": { \"SimpleBool\": { \"type\": \"boolean\" }, \"SimpleString\": { \"type\": \"string\" }, \"SimpleChar\": { \"type\": \"string\" }, \"SimpleByte\": { \"type\": \"integer\", \"format\": \"int32\", \"maximum\": 255.0, \"minimum\": 0.0 }, \"SimpleSByte\": { \"type\": \"integer\", \"format\": \"int32\", \"maximum\": 127.0, \"minimum\": -128.0 }, \"SimpleInt16\": { \"type\": \"integer\", \"format\": \"int32\", \"maximum\": 32767.0, \"minimum\": -32768.0 }, \"SimpleUInt16\": { \"type\": \"integer\", \"format\": \"int32\", \"maximum\": 65535.0, \"minimum\": 0.0 }, \"SimpleInt32\": { \"type\": \"integer\", \"format\": \"int32\" }, \"SimpleUInt32\": { \"type\": \"integer\", \"format\": \"int64\", \"maximum\": 4294967295.0, \"minimum\": 0.0 }, \"SimpleInt64\": { \"type\": \"integer\", \"format\": \"int64\" }, \"SimpleUInt64\": { \"type\": \"integer\", \"format\": \"int64\", \"maximum\": 1.8446744073709552E+19, \"minimum\": 0.0 }, \"SimpleFloat\": { \"type\": \"number\", \"format\": \"float\" }, \"SimpleDouble\": { \"type\": \"number\", \"format\": \"double\" }, \"SimpleDecimal\": { \"type\": \"number\", \"format\": \"double\" }, \"SimpleDateTime\": { \"type\": \"string\", \"format\": \"date-time\" }, \"StringArray\": { \"type\": \"array\", \"items\": { \"type\": \"string\" } }, \"ByteArray\": { \"type\": \"string\", \"format\": \"byte\" }, \"StringList\": { \"type\": \"array\", \"items\": { \"type\": \"string\" } }, \"DictionaryStringDouble\": { \"type\": \"object\", \"additionalProperties\": { \"type\": \"number\", \"format\": \"double\" } } } }") },
                    new object[] { typeof(ComplexEvent), JObject.Parse("{  \"type\": \"object\",  \"additionalProperties\": false,  \"required\": [ \"SimpleEvent\", \"SimpleEventList\", \"SomeInt\", \"AllDataTypesObject\", \"MyIntArray\"  ],  \"properties\": { \"SimpleEvent\": { \"$ref\": \"#/definitions/SimpleEvent\" }, \"SimpleEventList\": { \"type\": \"array\", \"items\": { \"$ref\": \"#/definitions/SimpleEvent\" } }, \"SomeInt\": { \"type\": \"integer\", \"format\": \"int32\" }, \"AllDataTypesObject\": { \"$ref\": \"#/definitions/AllDataTypesEvent\" }, \"MyIntArray\": { \"type\": \"array\", \"items\": { \"type\": \"integer\", \"format\": \"int32\" } }  },  \"definitions\": { \"SimpleEvent\": { \"type\": \"object\", \"additionalProperties\": false, \"required\": [ \"MyInteger\", \"MyFloat\", \"MyDouble\", \"MyString\", \"MyStringArray\" ], \"properties\": { \"MyInteger\": {  \"type\": \"integer\",  \"format\": \"int32\" }, \"MyFloat\": {  \"type\": \"number\",  \"format\": \"float\" }, \"MyDouble\": {  \"type\": \"number\",  \"format\": \"double\" }, \"MyString\": {  \"type\": \"string\" }, \"MyStringArray\": {  \"type\": \"array\",  \"items\": {  \"type\": \"string\"  } } } }, \"AllDataTypesEvent\": { \"type\": \"object\", \"additionalProperties\": false, \"required\": [ \"SimpleBool\", \"SimpleString\", \"SimpleChar\", \"SimpleByte\", \"SimpleSByte\", \"SimpleInt16\", \"SimpleUInt16\", \"SimpleInt32\", \"SimpleUInt32\", \"SimpleInt64\", \"SimpleUInt64\", \"SimpleFloat\", \"SimpleDouble\", \"SimpleDecimal\", \"SimpleDateTime\", \"StringArray\", \"ByteArray\", \"StringList\", \"DictionaryStringDouble\" ], \"properties\": { \"SimpleBool\": {  \"type\": \"boolean\" }, \"SimpleString\": {  \"type\": \"string\" }, \"SimpleChar\": {  \"type\": \"string\" }, \"SimpleByte\": {  \"type\": \"integer\",  \"format\": \"int32\",  \"maximum\": 255.0,  \"minimum\": 0.0 }, \"SimpleSByte\": {  \"type\": \"integer\",  \"format\": \"int32\",  \"maximum\": 127.0,  \"minimum\": -128.0 }, \"SimpleInt16\": {  \"type\": \"integer\",  \"format\": \"int32\",  \"maximum\": 32767.0,  \"minimum\": -32768.0 }, \"SimpleUInt16\": {  \"type\": \"integer\",  \"format\": \"int32\",  \"maximum\": 65535.0,  \"minimum\": 0.0 }, \"SimpleInt32\": {  \"type\": \"integer\",  \"format\": \"int32\" }, \"SimpleUInt32\": {  \"type\": \"integer\",  \"format\": \"int64\",  \"maximum\": 4294967295.0,  \"minimum\": 0.0 }, \"SimpleInt64\": {  \"type\": \"integer\",  \"format\": \"int64\" }, \"SimpleUInt64\": {  \"type\": \"integer\",  \"format\": \"int64\",  \"maximum\": 1.8446744073709552E+19,  \"minimum\": 0.0 }, \"SimpleFloat\": {  \"type\": \"number\",  \"format\": \"float\" }, \"SimpleDouble\": {  \"type\": \"number\",  \"format\": \"double\" }, \"SimpleDecimal\": {  \"type\": \"number\",  \"format\": \"double\" }, \"SimpleDateTime\": {  \"type\": \"string\",  \"format\": \"date-time\" }, \"StringArray\": {  \"type\": \"array\",  \"items\": {  \"type\": \"string\"  } }, \"ByteArray\": {  \"type\": \"string\",  \"format\": \"byte\" }, \"StringList\": {  \"type\": \"array\",  \"items\": {  \"type\": \"string\"  } }, \"DictionaryStringDouble\": {  \"type\": \"object\",  \"additionalProperties\": {  \"type\": \"number\",  \"format\": \"double\"  } } } } } }") },
           };

            [Theory]
            [MemberData(nameof(ExpectedSchemas))]
            public void CheckSchemaGeneration(Type type, JObject expectedSchemaAsJson)
            {
                JObject actualJsonSchemaOfType = OpenApiMapper.GetJsonSchemaOfType(type);
                actualJsonSchemaOfType.Should().BeEquivalentTo(expectedSchemaAsJson);
            }
        }
    }
}
