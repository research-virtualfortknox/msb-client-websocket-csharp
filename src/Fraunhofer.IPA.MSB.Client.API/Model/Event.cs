// <copyright file="Event.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using Fraunhofer.IPA.MSB.Client.Websocket.Model;
    using Newtonsoft.Json;

    /// <summary>
    /// Event of an MSB <see cref="Service"/>.
    /// </summary>
    public class Event
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> class.
        /// </summary>
        /// <param name="id">The <see cref="Event.Id"/> of the <see cref="Event"/>.</param>
        /// <param name="name">The <see cref="Event.Name"/> of the <see cref="Event"/>.</param>
        /// <param name="description">The <see cref="Event.Description"/> of the <see cref="Event"/>.</param>
        /// <param name="dataFormatType">The Type used to generate <see cref="Event.DataFormat"/> of the <see cref="Event"/>.</param>
        public Event(string id, string name, string description, Type dataFormatType)
            : this(id, name, description, new DataFormat("dataObject", dataFormatType))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> class.
        /// </summary>
        /// <param name="id">The <see cref="Event.Id"/> of the <see cref="Event"/>.</param>
        /// <param name="name">The <see cref="Event.Name"/> of the <see cref="Event"/>.</param>
        /// <param name="description">The <see cref="Event.Description"/> of the <see cref="Event"/>.</param>
        /// <param name="dataFormat">The <see cref="Event.DataFormat"/> of the <see cref="Event"/>.</param>
        public Event(string id, string name, string description, DataFormat dataFormat)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.DataFormat = dataFormat;
        }

        /// <summary>Gets or sets id for referencing in JSON.</summary>
        [JsonProperty("@id")]
        public int AtId { get; set; }

        /// <summary>Gets or sets id of the event.</summary>
        [JsonProperty("eventId")]
        public string Id { get; set; }

        /// <summary>Gets or sets name of the event.</summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>Gets or sets description of the event.</summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>Gets or sets data format of the event.</summary>
        [JsonProperty("dataFormat")]
        public DataFormat DataFormat { get; set; }
    }
}
