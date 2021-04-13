// <copyright file="PublishingError.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace Fraunhofer.IPA.MSB.Client.API.Errors
{
    using Fraunhofer.IPA.MSB.Client.API.Model;

    /// <summary>
    /// Defines error types which can occur during publishing.
    /// </summary>
    public enum PublishingError
    {
        /// <summary>
        /// Error occurs when <see cref="EventData"/> are published via unauthorized connection.
        /// </summary>
        NIO_UNAUTHORIZED_CONNECTION,

        /// <summary>
        /// Error occurs when <see cref="EventData"/> can't be forwarded.
        /// </summary>
        NIO_EVENT_FORWARDING_ERROR,

        /// <summary>
        /// Unexpected error during event publishing.
        /// </summary>
        NIO_UNEXPECTED_EVENT_FORWARDING_ERROR,
    }
}
