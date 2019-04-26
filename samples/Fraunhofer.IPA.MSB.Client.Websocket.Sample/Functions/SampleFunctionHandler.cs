// <copyright file="SampleFunctionHandler.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace Fraunhofer.IPA.MSB.Client.Websocket.Sample.Functions
{
    using Fraunhofer.IPA.MSB.Client.API.Attributes;
    using Fraunhofer.IPA.MSB.Client.API.Model;
    using Serilog;

    [MsbFunctionHandler(Id = "SimpleFunctionHandlerId")]
    public class SampleFunctionHandler : AbstractFunctionHandler
    {
        public SampleFunctionHandler()
        {
        }

        [MsbFunction(
            Id = "EmptySampleFunction",
            Name = "Empty Sample Function",
            Description = "Description of EmptySampleFunction")]
        public void EmptySampleFunction(FunctionCallInfo info)
        {
            Log.Information("EmptySampleFunction has been called");
        }

        [MsbFunction(
            Id = "SampleFunctionWithParameters",
            Name = "Sample Function With Parameters",
            Description = "Description of SampleFunctionWithParameters")]
        public void SampleFunctionWithParameters([MsbFunctionParameter(Name = "a")]string a, [MsbFunctionParameter(Name = "b")] Events.SimpleEvent input2, FunctionCallInfo info)
        {
            Log.Information($"SampleFunctionWithParameters has been called with the following parameters: a={a} ; input2.myInteger={input2.MyInteger}");
        }

        [MsbFunction(
            Id = "/SampleFunctionWithResponseEvent",
            Name = "sample Function With Response Event",
            Description = "Description of SampleFunctionWithResponseEvent",
            ResponseEvents = new string[] { "SimpleEventId", "ComplexEventId" })]
        public EventData SampleFunctionWithResponseEvent(
            [MsbFunctionParameter(Name = "testStringParam")] string testString,
            [MsbFunctionParameter(Name = "testNestedObjectParam")] Events.ComplexEvent testNestedObject,
            FunctionCallInfo functionCallInfo)
        {
            Log.Information("SampleFunctionWithResponseEvent has been called");

            return new EventDataBuilder(functionCallInfo.ResponseEvents["SimpleEventId"]).SetValue(new Events.SimpleEvent()).Build();
        }
    }
}
