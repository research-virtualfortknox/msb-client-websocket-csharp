// <copyright file="EventTests.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using FluentAssertions;
    using FluentAssertions.Json;
    using Fraunhofer.IPA.MSB.Client.API.Model;
    using Fraunhofer.IPA.MSB.Client.Websocket.Model;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Xunit;

    public class EventTests
    {
        public EventTests()
        {
            this.TestEvent = new Event(this.ExpectedEventId, this.ExpectedEventName, this.ExpectedEventDescription, typeof(string));
        }

        protected string ExpectedEventId { get; } = "TestEvent1";

        protected string ExpectedEventName { get; } = "Test Event 1";

        protected string ExpectedEventDescription { get; } = "Test Event Description";

        protected DataFormat ExpectedEventDataFormat { get; } = new DataFormat("dataObject", typeof(string));

        protected Event TestEvent { get; }

        public class Constructors : EventTests
        {
            [Fact]
            public void CheckId()
            {
                Assert.Equal(this.ExpectedEventId, this.TestEvent.Id);
            }

            [Fact]
            public void CheckName()
            {
                Assert.Equal(this.ExpectedEventName, this.TestEvent.Name);
            }

            [Fact]
            public void CheckDescription()
            {
                Assert.Equal(this.ExpectedEventDescription, this.TestEvent.Description);
            }

            [Fact]
            public void CheckDataFormat()
            {
                this.TestEvent.DataFormat.Should().ContainKey("dataObject");
            }
        }

        public class Serialization : EventTests
        {
            [Fact]
            public void CheckJsonSerialization()
            {
                JObject actualJsonObject = JObject.Parse(JsonConvert.SerializeObject(this.TestEvent));
                actualJsonObject.Should().HaveElement("@id");
                actualJsonObject.Should().HaveElement("eventId");
                actualJsonObject.GetValue("eventId").Should().HaveValue(this.ExpectedEventId);
                actualJsonObject.Should().HaveElement("name");
                actualJsonObject.GetValue("name").Should().HaveValue(this.ExpectedEventName);
                actualJsonObject.Should().HaveElement("description");
                actualJsonObject.GetValue("description").Should().HaveValue(this.ExpectedEventDescription);
                actualJsonObject.Should().HaveElement("dataFormat");
                actualJsonObject.GetValue("dataFormat").Should().BeEquivalentTo(JObject.Parse(JsonConvert.SerializeObject(this.ExpectedEventDataFormat)));
            }
        }
    }
}
