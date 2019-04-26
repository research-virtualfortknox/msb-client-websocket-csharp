// <copyright file="ApplicationProperties.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace Fraunhofer.IPA.MSB.Client.API.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Fraunhofer.IPA.MSB.Client.API.Model;

    /// <summary>
    /// Handles the application.properties file.
    /// </summary>
    public class ApplicationProperties
    {
        private ApplicationProperties(string url, string classType, string uuid, string name, string description, string token)
        {
            this.Url = url;
            this.ServiceType = classType;
            this.Uuid = uuid;
            this.Name = name;
            this.Description = description;
            this.Token = token;
        }

        /// <summary>
        /// Gets the URL to connect to MSB read from properties file.
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Gets the type of <see cref="Service"/> read from properties file.
        /// </summary>
        public string ServiceType { get; private set; }

        /// <summary>
        /// Gets the <see cref="Service.Uuid"/> read from properties file.
        /// </summary>
        public string Uuid { get; private set; }

        /// <summary>
        /// Gets the <see cref="Service.Name"/> read from properties file.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the <see cref="Service.Description"/> read from properties file.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the <see cref="Service.Token"/> read from properties file.
        /// </summary>
        public string Token { get; private set; }

        /// <summary>
        /// Read .properties file.
        /// </summary>
        /// <returns>True, if file was successfully read.</returns>
        public static ApplicationProperties Read()
        {
            var readProperties = new Dictionary<string, string>();
            foreach (var row in File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, "application.properties")))
            {
                readProperties.Add(row.Split('=')[0], string.Join("=", row.Split('=').Skip(1).ToArray()));
            }

            var applicationProperties = new ApplicationProperties(
                readProperties["msb.url"],
                readProperties["msb.type"],
                readProperties["msb.uuid"],
                readProperties["msb.name"],
                readProperties["msb.description"],
                readProperties["msb.token"]);

            return applicationProperties;
        }
    }
}
