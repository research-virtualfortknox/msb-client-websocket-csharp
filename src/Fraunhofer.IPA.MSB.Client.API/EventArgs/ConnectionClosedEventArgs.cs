// <copyright file="ConnectionClosedEventArgs.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

    /// <summary>
    /// Args for event when connection of client closed.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class ConnectionClosedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionClosedEventArgs"/> class.
        /// </summary>
        /// <param name="code">The code for the close.</param>
        /// <param name="reason">The reason for the close.</param>
        public ConnectionClosedEventArgs(int code, string reason)
        {
            this.Code = code;
            this.Reason = reason;
        }

        /// <summary>
        /// Gets the empty <see cref="ConnectionClosedEventArgs"/>.
        /// </summary>
        public static new ConnectionClosedEventArgs Empty
        {
            get
            {
                return new ConnectionClosedEventArgs(-1, "Empty Args");
            }
        }

        /// <summary>Gets the code for the close.</summary>
        public int Code { get; }

        /// <summary>Gets the reason for the close.</summary>
        public string Reason { get; }
    }
}
