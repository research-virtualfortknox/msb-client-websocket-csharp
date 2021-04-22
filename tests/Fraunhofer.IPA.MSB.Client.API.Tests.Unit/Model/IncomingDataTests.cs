// <copyright file="IncomingDataTests.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using FluentAssertions;
    using FluentAssertions.Json;
    using Fraunhofer.IPA.MSB.Client.API.Model;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Linq;
    using Xunit;

    public class IncomingDataTests
    {
        public IncomingDataTests()
        {
            this.TestIncomingData = new IncomingData(this.ExpectedServiceUuid, this.ExpectedEventId, this.ExpectedPriority, this.ExpectedPostDate, this.ExpectedDataObject, this.ExpectedCorrelationId);
        }

        protected string ExpectedServiceUuid { get; } = "beed9406-aacb-4e18-8c3d-69ff0db623cd";

        protected string ExpectedEventId { get; } = "TestEventId";

        protected DateTime ExpectedPostDate { get; } = DateTime.Now;

        protected object ExpectedDataObject { get; } = "TestValue";

        protected EventPriority ExpectedPriority { get; } = EventPriority.HIGH;

        protected string ExpectedCorrelationId { get; } = "232f0105-7335-4c36-b625-d5f6e41954be";

        protected IncomingData TestIncomingData { get; }

        public class Constructors : IncomingDataTests
        {
            [Fact]
            public void CheckServiceUuid()
            {
                Assert.Equal(this.ExpectedServiceUuid, this.TestIncomingData.Uuid);
            }

            [Fact]
            public void CheckEventId()
            {
                Assert.Equal(this.ExpectedEventId, this.TestIncomingData.EventId);
            }

            [Fact]
            public void CheckEventPriority()
            {
                Assert.Equal(this.ExpectedPriority, this.TestIncomingData.Priority);
            }

            [Fact]
            public void CheckPostDate()
            {
                Assert.Equal(this.ExpectedPostDate, this.TestIncomingData.PostDate);
            }

            [Fact]
            public void CheckDataObject()
            {
                Assert.Equal(this.ExpectedDataObject, this.TestIncomingData.DataObject);
            }

            [Fact]
            public void CheckCorrelationId()
            {
                Assert.Equal(this.ExpectedCorrelationId, this.TestIncomingData.CorrelationId);
            }
        }

        public class Serialization : IncomingDataTests
        {
            [Fact]
            public void CheckJsonSerialization()
            {
                JObject actualJsonObject = JObject.Parse(JsonConvert.SerializeObject(this.TestIncomingData));
                actualJsonObject.Should().HaveElement("uuid");
                actualJsonObject.GetValue("uuid").Should().HaveValue(this.ExpectedServiceUuid);
                actualJsonObject.Should().HaveElement("eventId");
                actualJsonObject.GetValue("eventId").Should().HaveValue(this.ExpectedEventId);
                actualJsonObject.Should().HaveElement("priority");
                actualJsonObject.GetValue("priority").Should().HaveValue(JsonConvert.SerializeObject(this.ExpectedPriority));
                JsonSerializerSettings dateFormatSettings = new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                };
                actualJsonObject.Should().HaveElement("postDate");
                actualJsonObject.GetValue("postDate").ToString(Formatting.None, new IsoDateTimeConverter()).Should().Equals(JsonConvert.SerializeObject(this.ExpectedPostDate, dateFormatSettings));
                actualJsonObject.Should().HaveElement("dataObject");
                actualJsonObject.GetValue("dataObject").Should().HaveValue(this.ExpectedDataObject.ToString());
                actualJsonObject.Should().HaveElement("correlationId");
                actualJsonObject.GetValue("correlationId").Should().HaveValue(this.ExpectedCorrelationId);
            }
        }
    }
}
