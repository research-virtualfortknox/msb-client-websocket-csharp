// <copyright file="AbstractFunctionHandler.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using System.Linq;
    using System.Reflection;
    using Fraunhofer.IPA.MSB.Client.API.Attributes;
    using Fraunhofer.IPA.MSB.Client.API.Logging;

    /// <summary>
    /// Abstract class to derive an handler for MSB functions.
    /// </summary>
    public abstract class AbstractFunctionHandler
    {
        private static readonly ILog Log = LogProvider.For<AbstractFunctionHandler>();

        /// <summary>
        /// Returns the index of the parameter in the parameter list of the method depending on the given name.
        /// </summary>
        /// <param name="methodInfo">Method used to search for the parameter.</param>
        /// <param name="parameterName">Name of the parameter to search for.</param>
        /// <returns>Index of the parameter in the parameter list of the method.</returns>
        public static int GetParameterIndexFromName(MethodInfo methodInfo, string parameterName)
        {
            var parameters = methodInfo.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                var customParameterAttribute = parameters[i].GetCustomAttributes(typeof(MsbFunctionParameter), true).FirstOrDefault() as MsbFunctionParameter;
                if (customParameterAttribute.Name == parameterName)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
