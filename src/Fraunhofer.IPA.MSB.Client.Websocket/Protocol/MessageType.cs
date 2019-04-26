// <copyright file="MessageType.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace Fraunhofer.IPA.MSB.Client.Websocket.Protocol
{
    using System;

    /// <summary>
    /// Defines the messages types that are send by the MSB via Websocket.
    /// </summary>
    public class MessageType
    {
#pragma warning disable SA1310 // Field names must not contain underscore
#pragma warning disable CS1591 // Missing XML comment for public visible type or element
#pragma warning disable SA1600 // Elements must be documented
        public const string REGISTRATION = "R";
        public const string EVENT = "E";
        public const string FUNCTION_CALLBACK = "C";
        public const string CONFIG = "K";
        public const string IO_CONNECTED = "IO_CONNECTED";
        public const string IO_REGISTERED = "IO_REGISTERED";
        public const string IO_PUBLISHED = "IO_PUBLISHED";
        public const string NIO_ALREADY_CONNECTED = "NIO_ALREADY_CONNECTED";
        public const string NIO_REGISTRATION_ERROR = "NIO_REGISTRATION_ERROR";
        public const string NIO_UNEXPECTED_REGISTRATION_ERROR = "NIO_UNEXPECTED_REGISTRATION_ERROR";
        public const string NIO_UNAUTHORIZED_CONNECTION = "NIO_UNAUTHORIZED_CONNECTION";
        public const string NIO_EVENT_FORWARDING_ERROR = "NIO_EVENT_FORWARDING_ERROR";
        public const string NIO_UNEXPECTED_EVENT_FORWARDING_ERROR = "NIO_UNEXPECTED_EVENT_FORWARDING_ERROR";
        public const string PING = "PING";
        public const string UNKOWN = "UNKOWN";
        #pragma warning restore SA1310 // Field names must not contain underscore
        #pragma warning restore CS1591 // Missing XML comment for public visible type or element
        #pragma warning restore SA1600 // Elements must be documented

        /// <summary>
        /// Returns the type of a message.
        /// </summary>
        /// <param name="message">The message for which the type is determined.</param>
        /// <returns>Type of the message as String.</returns>
        public static string GetTypeOfMessage(string message)
        {
            if (message.ToUpper().StartsWith(MessageType.REGISTRATION))
            {
                return REGISTRATION;
            }
            else if (message.ToUpper().StartsWith(MessageType.EVENT))
            {
                return EVENT;
            }
            else if (message.ToUpper().StartsWith(MessageType.FUNCTION_CALLBACK))
            {
                return FUNCTION_CALLBACK;
            }
            else if (message.ToUpper().StartsWith(MessageType.CONFIG))
            {
                return CONFIG;
            }
            else if (message.ToUpper().StartsWith(MessageType.PING))
            {
                return PING;
            }
            else if (message.ToUpper().StartsWith(MessageType.IO_CONNECTED))
            {
                return IO_CONNECTED;
            }
            else if (message.ToUpper().StartsWith(MessageType.IO_REGISTERED))
            {
                return IO_REGISTERED;
            }
            else if (message.ToUpper().StartsWith(MessageType.IO_PUBLISHED))
            {
                return IO_PUBLISHED;
            }
            else if (message.ToUpper().StartsWith(MessageType.NIO_ALREADY_CONNECTED))
            {
                return NIO_ALREADY_CONNECTED;
            }
            else if (message.ToUpper().StartsWith(MessageType.NIO_REGISTRATION_ERROR))
            {
                return NIO_REGISTRATION_ERROR;
            }
            else if (message.ToUpper().StartsWith(MessageType.NIO_UNEXPECTED_REGISTRATION_ERROR))
            {
                return NIO_UNEXPECTED_REGISTRATION_ERROR;
            }
            else if (message.ToUpper().StartsWith(MessageType.NIO_UNAUTHORIZED_CONNECTION))
            {
                return NIO_UNAUTHORIZED_CONNECTION;
            }
            else if (message.ToUpper().StartsWith(MessageType.NIO_EVENT_FORWARDING_ERROR))
            {
                return NIO_EVENT_FORWARDING_ERROR;
            }
            else if (message.ToUpper().StartsWith(MessageType.NIO_UNEXPECTED_EVENT_FORWARDING_ERROR))
            {
                return NIO_UNEXPECTED_EVENT_FORWARDING_ERROR;
            }
            else
            {
                return UNKOWN;
            }
        }
    }
}
