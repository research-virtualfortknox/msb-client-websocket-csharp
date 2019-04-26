// <copyright file="DataFormatTests.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using FluentAssertions;
    using FluentAssertions.Json;
    using Fraunhofer.IPA.MSB.Client.Websocket.Model;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Xunit;

    public class DataFormatTests
    {
        public class Constructors : DataFormatTests
        {
            [Fact]
            public void DataFormatGenerationFromType()
            {
                DataFormat dataFormat = new DataFormat("dataFormat", typeof(string));
                JObject dataFormatAsJson = JObject.Parse(JsonConvert.SerializeObject(dataFormat));

                dataFormatAsJson.Should().HaveElement("dataFormat");
            }
        }
    }
}
