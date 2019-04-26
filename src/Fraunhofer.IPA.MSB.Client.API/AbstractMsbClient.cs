// <copyright file="AbstractMsbClient.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace Fraunhofer.IPA.MSB.Client.API
{
    using System.Threading.Tasks;
    using Fraunhofer.IPA.MSB.Client.API.Model;

    /// <summary>
    /// Interface that must be implemented by an MSB client.
    /// </summary>
    public abstract class AbstractMsbClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractMsbClient"/> class.
        /// </summary>
        protected AbstractMsbClient()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractMsbClient"/> class.
        /// </summary>
        /// <param name="msbUrl">The URL to the MSB interface.</param>
        protected AbstractMsbClient(string msbUrl)
        {
            this.MsbUrl = msbUrl;
        }

        /// <summary>
        /// Gets or sets the URL of MSB websocket interface.
        /// </summary>
        public string MsbUrl { get; set; }

        /// <summary>
        /// Connect to the MSB.
        /// </summary>
        /// <returns>True if connection was successfully established.</returns>
        public abstract Task<bool> ConnectAsync();

        /// <summary>
        /// Disconnect from MSB.
        /// </summary>
        public abstract void Disconnect();

        /// <summary>
        /// Returns if connection to MSB has been established.
        /// </summary>
        /// <returns>True, if client is connected.</returns>
        public abstract bool IsConnected();

        /// <summary>
        /// Registers a <see cref="Service"/> at the MSB.
        /// </summary>
        /// <param name="serviceToRegister">The <see cref="Service"/> to be registered</param>
        /// <returns>True, if the registration was successful.</returns>
        public abstract Task<bool> RegisterAsync(Service serviceToRegister);

        /// <summary>
        /// Publishes <see cref="EventData"/> for an <see cref="Event"/>.
        /// </summary>
        /// <param name="service">The <see cref="Service"/> to publish <see cref="EventData"/> for.</param>
        /// <param name="eventData">The <see cref="EventData"/> to be published</param>
        /// /// <returns>True, if publish was successful.</returns>
        public abstract Task<bool> PublishAsync(Service service, EventData eventData);
    }
}
