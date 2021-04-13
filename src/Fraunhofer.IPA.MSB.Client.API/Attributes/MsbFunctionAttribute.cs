// <copyright file="MsbFunctionAttribute.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace Fraunhofer.IPA.MSB.Client.API.Attributes
{
    using System;
    using Fraunhofer.IPA.MSB.Client.API.Logging;
    using Fraunhofer.IPA.MSB.Client.API.Model;

    /// <summary>
    /// Marks a method as an MSB function.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class MsbFunctionAttribute : Attribute
    {
        /// <summary>Gets or sets the id of the MSB function.</summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>Gets or sets the name of the MSB function.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the description of the MSB function.</summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets array of event ids which should be declared as response events of the MSB function. When the function is added to a <see cref="Service"/>.
        /// </summary>
        public string[] ResponseEvents { get; set; } = new string[] { };
    }
}