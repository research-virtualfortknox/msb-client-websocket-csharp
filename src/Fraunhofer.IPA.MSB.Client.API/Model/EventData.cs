// <copyright file="EventData.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    /// This class is used to define data which should be published to the MSB as an <see cref="Event"/>.
    /// </summary>
    public class EventData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventData"/> class.
        /// </summary>
        /// <param name="correspondingEvent">The <see cref="Event"/> related to this <see cref="EventData"/>.</param>
        public EventData(Event correspondingEvent)
        {
            this.Event = correspondingEvent;
        }

        /// <summary>Gets EventData which can be returned if no response event should be send for a MSB function call.</summary>
        public static EventData NoResponseEvent { get; } = new EventData(new Event("noresponseevent", "noresponseevent", "noresponseevent", null));

        /// <summary>Gets or sets related <see cref="Event"/> of the data</summary>
        public Event Event { get; set; }

        /// <summary>Gets or sets id used for correlation in asynchronous communication.</summary>
        public string CorrelationId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>Gets or sets priority with which the event is processed by the MSB.</summary>
        public EventPriority EventPriority { get; set; } = EventPriority.LOW;

        /// <summary>Gets or sets value of the <see cref="Event"/></summary>
        public object Value { get; set; }

        /// <summary>Gets or sets timestamp when the <see cref="Event"/> was published.</summary>
        public DateTime PublishingDate { get; set; } = DateTime.Now;

        /// <summary>Gets or sets a value indicating whether the <see cref="Event"/> should be cached if there is no connection to MSB.</summary>
        public bool ShouldBeCached { get; set; } = true;
    }
}