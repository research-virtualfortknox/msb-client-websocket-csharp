﻿// <copyright file="Gateway.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using System.Collections.Generic;
    using Fraunhofer.IPA.MSB.Client.API.Configuration;
    using Fraunhofer.IPA.MSB.Client.API.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents an MSB Gateway.
    /// </summary>
    public class Gateway : Service
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Gateway"/> class.
        /// </summary>
        /// <param name="uuid">The <see cref="Service.Uuid"/> of the <see cref="Gateway"/>.</param>
        /// <param name="name">The <see cref="Service.Name"/> of the <see cref="Gateway"/>.</param>
        /// <param name="description">The <see cref="Service.Description"/> of the <see cref="Gateway"/>.</param>
        /// <param name="token">The <see cref="Service.Token"/> of the <see cref="Gateway"/>.</param>
        public Gateway(string uuid, string name, string description, string token)
            : base(uuid, name, description, token)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gateway"/> class.
        /// </summary>
        /// <param name="uuid">The <see cref="Service.Uuid"/> of the <see cref="Gateway"/>.</param>
        /// <param name="name">The <see cref="Service.Name"/> of the <see cref="Gateway"/>.</param>
        /// <param name="description">The <see cref="Service.Description"/> of the <see cref="Gateway"/>.</param>
        /// <param name="token">The <see cref="Service.Token"/> of the <see cref="Gateway"/>.</param>
        /// <param name="services">The <see cref="Gateway.Services"/> of the <see cref="Gateway"/>.</param>
        public Gateway(string uuid, string name, string description, string token, List<Service> services)
            : this(uuid, name, description, token)
        {
            this.Services = services;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gateway"/> class.
        /// </summary>
        /// <param name="msbProperties">MSB properties to use for creation of the <see cref="Gateway"/>.</param>
        public Gateway(MsbProperties msbProperties)
            : base(msbProperties.Uuid, msbProperties.Name, msbProperties.Description, msbProperties.Token)
        {
        }

        /// <summary>Gets definition of MSB type.</summary>
        public override string Class
        {
            get { return "Gateway"; }
        }

        /// <summary>Gets or sets the Services of the Gateway.</summary>
        [JsonProperty("services")]
        public List<Service> Services { get; set; } = new List<Service>();

        /// <summary>
        /// Adds a new <see cref="Service"/> to this <see cref="Gateway"/>.
        /// </summary>
        /// <param name="service">The <see cref="Service"/> to be added.</param>
        public void AddService(Service service)
        {
            this.Services.Add(service);
        }

        /// <summary>
        /// Removes a <see cref="Service"/> from this <see cref="Gateway"/>.
        /// </summary>
        /// <param name="service">The <see cref="Service"/> to be removed.</param>
        public void RemoveService(Service service)
        {
            this.Services.Remove(service);
        }
    }
}
