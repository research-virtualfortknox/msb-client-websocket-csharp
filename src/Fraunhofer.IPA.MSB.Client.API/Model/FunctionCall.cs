// <copyright file="FunctionCall.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a function call of the MSB.
    /// </summary>
    public class FunctionCall
    {
        /// <summary>Gets or sets uUID of the <see cref="Service"/> which contains the called <see cref="Function"/>.</summary>
        [JsonProperty("uuid")]
        public string ServiceUuid { get; set; }

        /// <summary>Gets or sets id of the called <see cref="Function"/>.</summary>
        [JsonProperty("functionId")]
        public string FunctionId { get; set; }

        /// <summary>Gets or sets correlation id of the event that initially triggered the function call.</summary>
        [JsonProperty("correlationId")]
        public string CorrelationId { get; set; }

        /// <summary>Gets or sets parameters of the function call.</summary>
        [JsonProperty("functionParameters")]
        public Dictionary<string, object> FunctionParameters { get; set; }
    }
}
