// <copyright file="MessageGenerator.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using System.Text;
    using Fraunhofer.IPA.MSB.Client.API.Model;
    using Fraunhofer.IPA.MSB.Client.API.Utils;
    using Fraunhofer.IPA.MSB.Client.Websocket.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// Generates Web socket messages.
    /// </summary>
    public class MessageGenerator
    {
        private static readonly ILog Log = LogProvider.For<MessageGenerator>();

        /// <summary>
        /// Generates a registration message for a <see cref="Service"/>
        /// </summary>
        /// <param name="serviceToRegister">The <see cref="Service"/> to generate registration message for.</param>
        /// <returns>The registration message as JSON string.</returns>
        public static string GenerateRegistrationMessage(Service serviceToRegister)
        {
            serviceToRegister.GenerateAtIds();
            string message = $"{MessageType.REGISTRATION} {SerializeObject(serviceToRegister)}";
            return message;
        }

        /// <summary>
        /// Generates the event message.
        /// </summary>
        /// <param name="service">The <see cref="Service"/> to create event message for.</param>
        /// <param name="eventData">The <see cref="EventData"/> to generate event message for.</param>
        /// <param name="useSockJs">Indicates if SockJS should be used.</param>
        /// <returns>The event message as JSON string.</returns>
        public static string GenerateEventMessage(Service service, EventData eventData, bool useSockJs)
        {
            IncomingData incomingData = new IncomingData(service.Uuid, eventData.Event.Id, eventData.EventPriority, eventData.PublishingDate, eventData.Value, eventData.CorrelationId);
            if (useSockJs)
            {
                return AddSockJsFrame($"{MessageType.EVENT} {EscapeStringValue(SerializeObject(incomingData))}");
            }
            else
            {
                return $"{MessageType.EVENT} {SerializeObject(incomingData)}";
            }
        }

        /// <summary>
        /// Generates the ping answer message.
        /// </summary>
        /// <param name="useSockJs">Indicates if SockJS should be used.</param>
        /// <returns>The answer message as string.</returns>
        public static string GeneratePingAnswerMessage(bool useSockJs)
        {
            if (useSockJs)
            {
                return AddSockJsFrame("pong");
            }
            else
            {
                return "pong";
            }
        }

        private static string AddSockJsFrame(string message)
        {
            return $"[\"{message}\"]";
        }

        private static string SerializeObject(object objectToSerialize)
        {
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.None,
                PreserveReferencesHandling = PreserveReferencesHandling.None,
            };
            jsonSerializerSettings.Converters.Add(new CustomIsoDateTimeConverter());
            string jsonString = JsonConvert.SerializeObject(objectToSerialize, Formatting.None, settings: jsonSerializerSettings);

            jsonString = jsonString.Replace("$id", "@id");
            return jsonString;
        }

        private static string EscapeStringValue(string value)
        {
            const char BACK_SLASH = '\\';
            const char SLASH = '/';
            const char DBL_QUOTE = '"';

            var output = new StringBuilder(value.Length);
            foreach (char c in value)
            {
                switch (c)
                {
                    case SLASH:
                        output.AppendFormat("{0}{1}", BACK_SLASH, SLASH);
                        break;

                    case BACK_SLASH:
                        output.AppendFormat("{0}{0}", BACK_SLASH);
                        break;

                    case DBL_QUOTE:
                        output.AppendFormat("{0}{1}", BACK_SLASH, DBL_QUOTE);
                        break;

                    default:
                        output.Append(c);
                        break;
                }
            }

            return output.ToString();
        }
    }
}
