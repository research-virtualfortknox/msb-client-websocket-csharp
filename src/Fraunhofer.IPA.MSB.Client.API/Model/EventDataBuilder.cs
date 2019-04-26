// <copyright file="EventDataBuilder.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

    /// <summary>
    /// Builder class to easily create <see cref="EventData"/>.
    /// </summary>
    public class EventDataBuilder
    {
        private EventData eventData;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventDataBuilder"/> class.
        /// </summary>
        /// <param name="correspondingEvent">The corresponding <see cref="Event"/> to the instantiated <see cref="EventData"/>.</param>
        public EventDataBuilder(Event correspondingEvent)
        {
            this.eventData = new EventData(correspondingEvent);
        }

        /// <summary>
        /// Sets <see cref="EventData.Value"/> of <see cref="EventData"/>.
        /// </summary>
        /// <param name="value">The <see cref="EventData.Value"/> to be set.</param>
        /// <returns>The same <see cref="EventDataBuilder"/> with property set.</returns>
        public EventDataBuilder SetValue(object value)
        {
            this.eventData.Value = value;
            return this;
        }

        /// <summary>
        /// Sets <see cref="EventData.PublishingDate"/> of <see cref="EventData"/>.
        /// </summary>
        /// <param name="publishingDate">The <see cref="EventData.PublishingDate"/> to be set.</param>
        /// <returns>The same <see cref="EventDataBuilder"/> with property set.</returns>
        public EventDataBuilder SetPublishingDate(DateTime publishingDate)
        {
            this.eventData.PublishingDate = publishingDate;
            return this;
        }

        /// <summary>
        /// Sets <see cref="EventData.CorrelationId"/> of <see cref="EventData"/>.
        /// </summary>
        /// <param name="correlationId">The <see cref="EventData.CorrelationId"/> to be set.</param>
        /// <returns>The same <see cref="EventDataBuilder"/> with property set.</returns>
        public EventDataBuilder SetCorrelationId(string correlationId)
        {
            this.eventData.CorrelationId = correlationId;
            return this;
        }

        /// <summary>
        /// Sets <see cref="EventData.EventPriority"/> of <see cref="EventData"/>.
        /// </summary>
        /// <param name="eventPriority">The <see cref="EventData.EventPriority"/> to be set.</param>
        /// <returns>The same <see cref="EventDataBuilder"/> with property set.</returns>
        public EventDataBuilder SetEventPriority(EventPriority eventPriority)
        {
            this.eventData.EventPriority = eventPriority;
            return this;
        }

        /// <summary>
        /// Sets <see cref="EventData.ShouldBeCached"/> of <see cref="EventData"/>.
        /// </summary>
        /// <param name="shouldBeCached">Value of <see cref="EventData.ShouldBeCached"/> to be set.</param>
        /// <returns>The same <see cref="EventDataBuilder"/> with property set.</returns>
        public EventDataBuilder SetShouldBeCached(bool shouldBeCached)
        {
            this.eventData.ShouldBeCached = shouldBeCached;
            return this;
        }

        /// <summary>
        /// Returns the build <see cref="EventData"/>.
        /// </summary>
        /// <returns>The <see cref="EventData"/> that was build.</returns>
        public EventData Build()
        {
            return this.eventData;
        }
    }
}
