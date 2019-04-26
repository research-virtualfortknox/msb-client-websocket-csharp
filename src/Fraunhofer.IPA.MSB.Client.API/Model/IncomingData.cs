// <copyright file="IncomingData.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using NJsonSchema;
    using NJsonSchema.Annotations;

    /// <summary>
    /// Represents incoming data for the MSB.
    /// </summary>
    public class IncomingData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IncomingData"/> class.
        /// </summary>
        /// <param name="uuid">The <see cref="IncomingData.Uuid"/> of the <see cref="Service"/></param>
        /// <param name="eventId">The <see cref="IncomingData.EventId"/> of the <see cref="Event"/></param>
        /// <param name="priority">The <see cref="IncomingData.Priority"/> of the data.</param>
        /// <param name="postDate">The <see cref="IncomingData.PostDate"/> when the data was posted.</param>
        /// <param name="dataObject">The <see cref="IncomingData.DataObject"/> of the incoming data.</param>
        /// <param name="correlationId">The <see cref="IncomingData.CorrelationId"/> of the data.</param>
        public IncomingData(string uuid, string eventId, EventPriority priority, DateTime postDate, object dataObject, string correlationId)
        {
            this.Uuid = uuid;
            this.EventId = eventId;
            this.Priority = priority;
            this.PostDate = postDate;
            this.DataObject = dataObject;
            this.CorrelationId = correlationId;
        }

        /// <summary>Gets or sets id of the <see cref="Service"/> that sends the data.</summary>
        [JsonProperty(Required = Required.Always, PropertyName = "uuid")]
        public string Uuid { get; set; }

        /// <summary>Gets or sets id of the <see cref="Event"/> that sends the data.</summary>
        [JsonProperty("eventId")]
        public string EventId { get; set; }

        /// <summary>Gets or sets priority the data should be processed with.</summary>
        [JsonProperty(Required = Required.Always, PropertyName = "priority")]
        public EventPriority Priority { get; set; }

        /// <summary>Gets or sets timestamp when the data was published.</summary>
        [JsonSchema(JsonObjectType.String, Format = "date-time")]
        [JsonProperty("postDate", ItemConverterType = typeof(IsoDateTimeConverter))]
        public DateTime PostDate { get; set; }

        /// <summary>Gets or sets correlation id of the <see cref="Event"/> that sends the data.</summary>
        [JsonProperty("correlationId")]
        public string CorrelationId { get; set; }

        /// <summary>Gets or sets the data which are send.</summary>
        [JsonProperty("dataObject")]
        public object DataObject { get; set; }
    }
}