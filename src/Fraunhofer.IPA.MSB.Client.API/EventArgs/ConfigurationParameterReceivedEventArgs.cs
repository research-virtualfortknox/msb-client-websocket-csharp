// <copyright file="ConfigurationParameterReceivedEventArgs.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace Fraunhofer.IPA.MSB.Client.API.EventArgs
{
    using System;
    using System.Collections.Generic;
    using Fraunhofer.IPA.MSB.Client.API.Model;

    /// <summary>
    /// Args for event when client received new configuration parameters.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class ConfigurationParameterReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationParameterReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="service">The <see cref="Service"/> for which the parameters are intended.</param>
        /// <param name="value">The configuration parameter values.</param>
        public ConfigurationParameterReceivedEventArgs(Service service, Dictionary<string, object> value)
        {
            this.Service = service;
            this.Value = value;
        }

        /// <summary>Gets the <see cref="Service"/> for which the parameters are intended.</summary>
        public Service Service { get; }

        /// <summary>Gets the values of the new configuration parameters.</summary>
        public Dictionary<string, object> Value { get; }
    }
}
