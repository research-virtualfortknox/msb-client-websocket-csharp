// <copyright file="SimpleFunctionHandler.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace Fraunhofer.IPA.MSB.Client.Tests.Shared.Functions
{
    using System;
    using Fraunhofer.IPA.MSB.Client.API.Attributes;
    using Fraunhofer.IPA.MSB.Client.API.Model;

    [MsbFunctionHandler(Id = "SimpleFunctionHandler")]
    public class SimpleFunctionHandler : AbstractFunctionHandler
    {
        [MsbFunction]
        public void SimpleFunctionWithNoParameters(FunctionCallInfo functionCallInfo)
        {
        }

        [MsbFunction]
        public void SimpleFunctionWithParameters(
            [MsbFunctionParameter(Name = "param1")] string testParameter1,
            [MsbFunctionParameter(Name = "param2")] int testParameter2,
            [MsbFunctionParameter(Name = "param3")] DateTime testParameter3,
            FunctionCallInfo functionCallInfo)
        {
        }
    }
}
