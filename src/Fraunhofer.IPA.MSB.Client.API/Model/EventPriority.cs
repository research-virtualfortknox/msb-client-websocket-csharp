// <copyright file="EventPriority.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Priority with which the event is processed by the MSB.
    /// </summary>
    public enum EventPriority
    {
        /// <summary>
        /// Event will be handled with low priority.
        /// </summary>
        LOW,

        /// <summary>
        /// Event will be handled with medium priority.
        /// </summary>
        MEDIUM,

        /// <summary>
        /// Event will be handled with high priority.
        /// </summary>
        HIGH,
    }
}
