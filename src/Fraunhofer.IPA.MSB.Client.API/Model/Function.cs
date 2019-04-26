// <copyright file="Function.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using System.Linq.Expressions;
    using System.Reflection;
    using Fraunhofer.IPA.MSB.Client.API.Attributes;
    using Fraunhofer.IPA.MSB.Client.API.Exceptions;
    using Fraunhofer.IPA.MSB.Client.API.Logging;
    using Fraunhofer.IPA.MSB.Client.Websocket.Model;
    using Newtonsoft.Json;

    /// <summary>
    /// Function of an MSB <see cref="Service"/>.
    /// </summary>
    public class Function
    {
        private static readonly ILog Log = LogProvider.For<Function>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Function"/> class.
        /// </summary>
        /// <param name="methodInfo">The info about the method.</param>
        /// <param name="callableObjectForMethod">Object of which the method will be called when callback was received.</param>
        public Function(MethodInfo methodInfo, object callableObjectForMethod)
        {
            if (this.IsValidMsbMethodDefintion(methodInfo))
            {
                var id = this.GetFunctionIdFromMethodInfo(methodInfo);
                var name = this.GetFunctionNameFromMethodInfo(methodInfo);
                var description = this.GetFunctionDescriptionFromMethodInfo(methodInfo);
                this.Init(id, name, description, methodInfo, callableObjectForMethod);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Function"/> class.
        /// </summary>
        /// <param name="id">The <see cref="Function.Id"/> of the <see cref="Function"/>.</param>
        /// <param name="name">The <see cref="Function.Name"/> of the <see cref="Function"/>.</param>
        /// <param name="description">The <see cref="Function.Description"/> of the <see cref="Function"/>.</param>
        /// <param name="methodInfo">The info about the method.</param>
        /// <param name="callableObjectForMethod">Object of which the method will be called when callback was received.</param>
        public Function(string id, string name, string description, MethodInfo methodInfo, object callableObjectForMethod)
        {
            if (this.IsValidMsbMethodDefintion(methodInfo))
            {
                this.Init(id, name, description, methodInfo, callableObjectForMethod);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Function"/> class.
        /// </summary>
        /// <param name="id">The <see cref="Function.Id"/> of the <see cref="Function"/>.</param>
        /// <param name="name">The <see cref="Function.Name"/> of the <see cref="Function"/>.</param>
        /// <param name="description">The <see cref="Function.Description"/> of the <see cref="Function"/>.</param>
        /// <param name="responseEvents">The <see cref="Function.ResponseEventIds"/> of the <see cref="Function"/>.</param>
        /// <param name="methodInfo">The info about the method.</param>
        /// <param name="callableObjectForMethod">Object of which the method will be called when callback was received.</param>
        public Function(string id, string name, string description, string[] responseEvents, MethodInfo methodInfo, object callableObjectForMethod)
        {
            if (this.IsValidMsbMethodDefintion(methodInfo))
            {
                this.Init(id, name, description, responseEvents.ToList<string>(), methodInfo, callableObjectForMethod);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Function"/> class.
        /// </summary>
        /// <param name="functionHandler">The <see cref="AbstractFunctionHandler"/> that contains the method.</param>
        /// <param name="methodInfo">The info about the method.</param>
        public Function(AbstractFunctionHandler functionHandler, MethodInfo methodInfo)
        {
            if (this.IsValidMsbMethodDefintion(methodInfo))
            {
                var id = this.GetFunctionIdFromHandler(functionHandler, methodInfo);
                var name = this.Name = this.GetFunctionNameFromMethodInfo(methodInfo);
                var description = this.GetFunctionDescriptionFromMethodInfo(methodInfo);
                this.Init(id, name, description, methodInfo, functionHandler);
            }
        }

        /// <summary>Gets or sets id of the function.</summary>
        [JsonProperty("functionId")]
        public string Id { get; set; }

        /// <summary>Gets or sets name of the function.</summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>Gets or sets description of the function.</summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>Gets or sets data format of the function.</summary>
        [JsonProperty("dataFormat")]
        public DataFormat DataFormat { get; set; }

        /// <summary>Gets or sets ids of the response <see cref="Event"/>s.</summary>
        [JsonIgnore]
        public List<string> ResponseEventIds { get; set; }

        /// <summary>Gets or sets <see cref="Event.AtId"/> of the response events. This id is used for referencing in JSON.</summary>
        [JsonProperty("responseEvents")]
        public List<int> ResponseEvents { get; set; } = new List<int>();

        /// <summary>Gets or sets pointer to the function which should be called when an MSB function call is received.</summary>
        [JsonIgnore]
        public Delegate FunctionPointer { get; set; }

        private void Init(string id, string name, string description, MethodInfo methodInfo, object callableObjectForMethod)
        {
            List<string> responseEvents = this.GetResponseEventsFromMethodInfo(methodInfo);
             this.Init(id, name, description, responseEvents, methodInfo, callableObjectForMethod);
        }

        private void Init(string id, string name, string description, List<string> responseEvents, MethodInfo methodInfo, object callableObjectForMethod)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.ResponseEventIds = responseEvents;
            this.FunctionPointer = this.CreateFunctionPointer(methodInfo, callableObjectForMethod);
            this.DataFormat = this.CreateDataFormatFromMethodInfo(methodInfo);
        }

        private bool IsValidMsbMethodDefintion(MethodInfo methodInfo)
        {
            if (methodInfo.GetParameters().Length == 0)
            {
                throw new InvalidMsbMethodDefinitionException("Method \"" + methodInfo.Name + "\" needs at least one parameter of type: \"FunctionCallInfo\" ");
            }

            if (methodInfo.GetParameters().Last().ParameterType != typeof(FunctionCallInfo))
            {
                throw new InvalidMsbMethodDefinitionException("Last Parameter of an function handler method has to be \"FunctionCallInfo\" in order to receive the function information");
            }

            if (this.GetResponseEventsFromMethodInfo(methodInfo).Count == 0)
            {
                if (methodInfo.ReturnType != typeof(void))
                {
                    throw new InvalidMsbMethodDefinitionException($"Return type of method '{methodInfo.Name}' must be 'void' if response events are defined");
                }
            }

            if (this.GetResponseEventsFromMethodInfo(methodInfo).Count > 0)
            {
                if (methodInfo.ReturnType != typeof(EventData))
                {
                    throw new InvalidMsbMethodDefinitionException($"Return type of method '{methodInfo.Name}' must be 'EventData' if response events are defined");
                }
            }

            return true;
        }

        private Delegate CreateFunctionPointer(MethodInfo methodInfo, object callableObjectForMethod)
        {
            var functionParameterTypes = from parameter in methodInfo.GetParameters() select parameter.ParameterType;
            Type delgateType;
            if (methodInfo.ReturnType == typeof(void))
            {
                delgateType = Expression.GetActionType(functionParameterTypes.ToArray());
            }
            else
            {
                functionParameterTypes = functionParameterTypes.Concat(new[] { methodInfo.ReturnType });
                delgateType = Expression.GetFuncType(functionParameterTypes.ToArray());
            }

            return methodInfo.CreateDelegate(delgateType, callableObjectForMethod);
        }

        private DataFormat CreateDataFormatFromMethodInfo(MethodInfo methodInfo)
        {
            DataFormat msbFunctionDataFormat = new DataFormat();

            foreach (var msbFunctionParameter in methodInfo.GetParameters().Where(m => m.ParameterType != typeof(FunctionCallInfo)))
            {
                if (msbFunctionParameter.GetCustomAttributes(typeof(MsbFunctionParameter), true).FirstOrDefault() is MsbFunctionParameter functionParameterAttribute)
                {
                    DataFormat dataFormatOfParameter = new DataFormat(functionParameterAttribute.Name, msbFunctionParameter.ParameterType);
                    foreach (var subDataFormat in dataFormatOfParameter)
                    {
                        msbFunctionDataFormat.Add(subDataFormat.Key, subDataFormat.Value);
                    }
                }
            }

            return msbFunctionDataFormat;
        }

        private List<string> GetResponseEventsFromMethodInfo(MethodInfo methodInfo)
        {
            if (methodInfo.GetCustomAttributes(typeof(MsbFunctionAttribute), true).FirstOrDefault() is MsbFunctionAttribute functionCallbackAttribute)
            {
                return functionCallbackAttribute.ResponseEvents.ToList<string>();
            }
            else
            {
                return new List<string>();
            }
        }

        private string GetFunctionIdFromMethodInfo(MethodInfo methodInfo)
        {
            var functionId = string.Empty;
            if (methodInfo.GetCustomAttributes(typeof(MsbFunctionAttribute), true).FirstOrDefault() is MsbFunctionAttribute methodFunctionAttribute)
            {
                functionId = methodFunctionAttribute.Id;
                if (functionId.Equals(string.Empty))
                {
                    functionId = methodInfo.Name;
                    Log.Warn("Id of MsbFunctionAttribute is not set. Name from MethodInfo is used as id.");
                }
            }
            else
            {
                functionId = methodInfo.Name;
            }

            return functionId;
        }

        private string GetFunctionIdFromHandler(AbstractFunctionHandler functionhandler, MethodInfo methodInfo)
        {
            var functionId = string.Empty;

            // Process path of IFunctionHandler
            var functionHandlerId = string.Empty;
            if (functionhandler.GetType().GetTypeInfo().GetCustomAttributes(typeof(MsbFunctionHandlerAttribute), true).FirstOrDefault() is MsbFunctionHandlerAttribute functionHandlerAttribute)
            {
                functionHandlerId = functionHandlerAttribute.Id;
                if (functionHandlerId.Equals(string.Empty))
                {
                    functionHandlerId = functionhandler.GetType().Name;
                    Log.Warn("Id of MsbFunctionHandlerAttribute is not set. Name of type is used as id.");
                }
            }
            else
            {
                functionHandlerId += functionhandler.GetType().Name;
            }

            functionId += functionHandlerId;
            if (!functionId.EndsWith("/") && !functionId.StartsWith("/"))
            {
                functionId = functionId + "/";
            }

            functionId += this.GetFunctionIdFromMethodInfo(methodInfo);
            return functionId;
        }

        private string GetFunctionNameFromMethodInfo(MethodInfo methodInfo)
        {
            var functionName = string.Empty;
            if (methodInfo.GetCustomAttributes(typeof(MsbFunctionAttribute), true).FirstOrDefault() is MsbFunctionAttribute methodFunctionAttribute)
            {
                functionName = methodFunctionAttribute.Name;
                if (functionName.Equals(string.Empty))
                {
                    functionName = methodInfo.Name;
                    Log.Warn("Name of MsbFunctionAttribute is not set. Name from MethodInfo is used as name.");
                }
            }
            else
            {
                functionName = methodInfo.Name;
            }

            return functionName;
        }

        private string GetFunctionDescriptionFromMethodInfo(MethodInfo methodInfo)
        {
            var description = string.Empty;
            if (methodInfo.GetCustomAttributes(typeof(MsbFunctionAttribute), true).FirstOrDefault() is MsbFunctionAttribute methodPathAttribute)
            {
                description = methodPathAttribute.Description;
            }
            else
            {
                description = string.Empty;
            }

            return description;
        }
    }
}
