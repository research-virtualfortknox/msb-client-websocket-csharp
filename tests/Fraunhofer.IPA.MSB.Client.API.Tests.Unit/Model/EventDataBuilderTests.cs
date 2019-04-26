// <copyright file="EventDataBuilderTests.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using Fraunhofer.IPA.MSB.Client.API.Model;
    using Xunit;

    public class EventDataBuilderTests
    {
        public EventDataBuilderTests()
        {
        }

        public class Build : EventDataBuilderTests
        {
            private readonly string expectedCorrelationId = "8423fefc-0c32-4fae-9bc8-4575f055e780";

            private readonly EventPriority expectedEventPriority = EventPriority.HIGH;

            private readonly bool expectedShouldBeCached = true;

            private readonly string expectedValue = "TestValue";

            private readonly DateTime expectedPublishingDate = DateTime.Now;

            private readonly EventData testEventData;

            public Build()
            {
                Event testEvent = new Event("id", "name", "description", typeof(string));
                this.testEventData = new EventDataBuilder(testEvent)
                    .SetCorrelationId(this.expectedCorrelationId)
                    .SetEventPriority(this.expectedEventPriority)
                    .SetPublishingDate(this.expectedPublishingDate)
                    .SetShouldBeCached(this.expectedShouldBeCached)
                    .SetValue(this.expectedValue)
                    .Build();
            }

            [Fact]
            public void CheckCorrelationid()
            {
                Assert.Equal(this.expectedCorrelationId, this.testEventData.CorrelationId);
            }

            [Fact]
            public void CheckEventPriority()
            {
                Assert.Equal(this.expectedEventPriority, this.testEventData.EventPriority);
            }

            [Fact]
            public void CheckPublishingDate()
            {
                Assert.Equal(this.expectedPublishingDate, this.testEventData.PublishingDate);
            }

            [Fact]
            public void CheckShouldBeCached()
            {
                Assert.Equal(this.expectedShouldBeCached, this.testEventData.ShouldBeCached);
            }

            [Fact]
            public void CheckSetValue()
            {
                Assert.Equal(this.expectedValue, this.testEventData.Value);
            }
        }
    }
}
