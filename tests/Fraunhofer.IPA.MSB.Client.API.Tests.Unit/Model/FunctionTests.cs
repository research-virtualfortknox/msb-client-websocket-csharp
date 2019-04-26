// <copyright file="FunctionTests.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace Fraunhofer.IPA.MSB.Client.API.Tests.Unit.Model
{
    using System;
    using System.Reflection;
    using Fraunhofer.IPA.MSB.Client.API.Attributes;
    using Fraunhofer.IPA.MSB.Client.API.Exceptions;
    using Fraunhofer.IPA.MSB.Client.API.Model;
    using Fraunhofer.IPA.MSB.Client.Tests.Shared;
    using Xunit;
    using Xunit.Abstractions;

    public class FunctionTests : BaseTests
    {
        public class Constructors : FunctionTests
        {
            [Fact]
            public void CreateFunctionWithNotMsbFunctionAttribute()
            {
                string methodName = "MsbFunctionWithNotMsbFunctionAttribute";
                MethodInfo methodInfo = this.GetType().GetRuntimeMethod(methodName, new Type[] { typeof(FunctionCallInfo) });
                Function createdFunction = new Function(methodInfo, this);
                Assert.Equal(methodName, createdFunction.Id);
                Assert.Equal(methodName, createdFunction.Name);
                Assert.Equal(string.Empty, createdFunction.Description);
                Assert.Empty(createdFunction.ResponseEvents);
            }

            #pragma warning disable xUnit1013 // Public method should be marked as test
            public void MsbFunctionWithNotMsbFunctionAttribute(FunctionCallInfo functionCallInfo)
            {
            }
            #pragma warning restore xUnit1013 // Public method should be marked as test

            [Fact]
            public void CreateFunctionWithMissingFunctionCallInfo()
            {
                string methodName = "MsbFunctionWithMissingFunctionCallInfo";
                MethodInfo methodInfo = this.GetType().GetRuntimeMethod(methodName, new Type[] { });
                Exception ex = Assert.Throws<InvalidMsbMethodDefinitionException>(() =>
                {
                    new Function(methodInfo, this);
                });
                Assert.Contains("needs at least one parameter of type: \"FunctionCallInfo\"", ex.Message);
            }

            #pragma warning disable xUnit1013 // Public method should be marked as test
            [MsbFunction]
            public void MsbFunctionWithMissingFunctionCallInfo()
            {
            }
            #pragma warning restore xUnit1013 // Public method should be marked as test

            [Fact]
            public void CreateFunctionWithFunctionCallInfoAtWrongPosition()
            {
                string methodName = "MsbFunctionWithFunctionCallInfoAtWrongPosition";
                MethodInfo methodInfo = this.GetType().GetRuntimeMethod(methodName, new Type[] { typeof(FunctionCallInfo), typeof(bool) });
                Exception ex = Assert.Throws<InvalidMsbMethodDefinitionException>(() =>
                {
                    new Function(methodInfo, this);
                });
                Assert.Contains("Last Parameter of an function handler method has to be \"FunctionCallInfo\"", ex.Message);
            }

            #pragma warning disable xUnit1013 // Public method should be marked as test
            [MsbFunction]
            public void MsbFunctionWithFunctionCallInfoAtWrongPosition(FunctionCallInfo functionCallInfo, bool otherParameter)
            {
            }
            #pragma warning restore xUnit1013 // Public method should be marked as test

            [Fact]
            public void CreateFunctionWithResponseEventAndWrongReturnType()
            {
                string methodName = "MsbFunctionWithResponseEventAndWrongReturnType";
                MethodInfo methodInfo = this.GetType().GetRuntimeMethod(methodName, new Type[] { typeof(FunctionCallInfo) });
                Exception ex = Assert.Throws<InvalidMsbMethodDefinitionException>(() =>
                {
                    new Function(methodInfo, this);
                });
                Assert.Contains("must be 'EventData' if response events are defined", ex.Message);
            }

            #pragma warning disable xUnit1013 // Public method should be marked as test
            [MsbFunction(ResponseEvents = new string[] { "ResponseEvent1" })]
            public void MsbFunctionWithResponseEventAndWrongReturnType(FunctionCallInfo functionCallInfo)
            {
            }
            #pragma warning restore xUnit1013 // Public method should be marked as test

            [Fact]
            public void CreateFunctionWithoutResponseEventAndWrongReturnType()
            {
                string methodName = "MsbFunctionWithoutResponseEventAndWrongReturnType";
                MethodInfo methodInfo = this.GetType().GetRuntimeMethod(methodName, new Type[] { typeof(FunctionCallInfo) });
                Exception ex = Assert.Throws<InvalidMsbMethodDefinitionException>(() =>
                {
                    new Function(methodInfo, this);
                });
                Assert.Contains("must be 'void' if response events are defined", ex.Message);
            }

            #pragma warning disable xUnit1013 // Public method should be marked as test
            [MsbFunction]
            public bool MsbFunctionWithoutResponseEventAndWrongReturnType(FunctionCallInfo functionCallInfo)
            {
                return false;
            }
            #pragma warning restore xUnit1013 // Public method should be marked as test

            [Fact]
            public void CreateFunctionFromMsbFunctionWithoutSpecifiedProperties()
            {
                string methodName = "MsbFunctionWithoutSpecifiedProperties";
                MethodInfo methodInfo = this.GetType().GetRuntimeMethod(methodName, new Type[] { typeof(FunctionCallInfo) });
                Function createdFunction = new Function(methodInfo, this);
                Assert.Equal(methodName, createdFunction.Id);
                Assert.Equal(methodName, createdFunction.Name);
                Assert.Equal(string.Empty, createdFunction.Description);
                Assert.Empty(createdFunction.ResponseEventIds);
            }

            #pragma warning disable xUnit1013 // Public method should be marked as test
            [MsbFunction]
            public void MsbFunctionWithoutSpecifiedProperties(FunctionCallInfo functionCallInfo)
            {
            }
            #pragma warning restore xUnit1013 // Public method should be marked as test

            [Fact]
            public void CreateFunctionFromMsbFunctionWithSpecifiedProperties()
            {
                string methodName = "MsbFunctionWithSpecifiedProperties";
                MethodInfo methodInfo = this.GetType().GetRuntimeMethod(methodName, new Type[] { typeof(FunctionCallInfo) });
                Function createdFunction = new Function(methodInfo, this);
                Assert.Equal("TestId", createdFunction.Id);
                Assert.Equal("TestName", createdFunction.Name);
                Assert.Equal("TestDescirption", createdFunction.Description);
                Assert.Equal(2, createdFunction.ResponseEventIds.Count);
                Assert.Contains("ResponseEvent1", createdFunction.ResponseEventIds);
                Assert.Contains("ResponseEvent2", createdFunction.ResponseEventIds);
            }

            #pragma warning disable xUnit1013 // Public method should be marked as test
            [MsbFunction(
                Id = "TestId",
                Name = "TestName",
                Description = "TestDescirption",
                ResponseEvents = new string[] { "ResponseEvent1", "ResponseEvent2" })]
            public EventData MsbFunctionWithSpecifiedProperties(FunctionCallInfo functionCallInfo)
            {
                return new EventDataBuilder(functionCallInfo.ResponseEvents["ResponseEvent1"]).Build();
            }
            #pragma warning restore xUnit1013 // Public method should be marked as test

            [Fact]
            public void CreateFunctionWithOverrideOfSpecifiedMsbFunctionProperties()
            {
                string methodName = "MsbFunctionWithoutSpecifiedProperties";
                string newId = "NewId";
                string newName = "NewName";
                string newDescription = "NewDescription";
                string[] responseEvents = new string[] { "ResponseEventNew" };
                MethodInfo methodInfo = this.GetType().GetRuntimeMethod(methodName, new Type[] { typeof(FunctionCallInfo) });
                Function createdFunction = new Function(newId, newName, newDescription, responseEvents, methodInfo, this);
                Assert.Equal(newId, createdFunction.Id);
                Assert.Equal(newName, createdFunction.Name);
                Assert.Equal(newDescription, createdFunction.Description);
                Assert.Contains("ResponseEventNew", createdFunction.ResponseEventIds);
            }
        }
    }
}
