// <copyright file="FunctionCallInfo.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using System.Collections.Generic;
    using Fraunhofer.IPA.MSB.Client.API.Logging;

    /// <summary>
    /// Provides information about an function call to the invoked <see cref="Function.FunctionPointer"/>.
    /// </summary>
    public class FunctionCallInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionCallInfo"/> class.
        /// </summary>
        /// <param name="msbClient">The MSB client.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="service">The service.</param>
        /// <param name="function">The function.</param>
        /// <param name="responseEvents">The response events.</param>
        public FunctionCallInfo(AbstractMsbClient msbClient, string correlationId, Service service, Function function, Dictionary<string, Event> responseEvents)
        {
            this.MsbClient = msbClient;
            this.CorrelationId = correlationId;
            this.Service = service;
            this.Function = function;
            this.ResponseEvents = responseEvents;
        }

        /// <summary>Gets MSB client that received the function callback.</summary>
        public AbstractMsbClient MsbClient { get; }

        /// <summary>Gets <see cref="EventData.CorrelationId"/> of the <see cref="Event"/> that initially triggered the <see cref="Function"/> call.</summary>
        public string CorrelationId { get; }

        /// <summary>Gets <see cref="Service"/> which contains the called <see cref="Function"/>.</summary>
        public Service Service { get; }

        /// <summary>Gets <see cref="Function.Id"/> of the called function.</summary>
        public Function Function { get; }

        /// <summary>Gets <see cref="Function.Id"/> of the called function.</summary>
        public Dictionary<string, Event> ResponseEvents { get; }
    }
}
