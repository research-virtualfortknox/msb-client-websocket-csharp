// <copyright file="SmartObjectTests.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using Fraunhofer.IPA.MSB.Client.API.Model;
    using Xunit;

    public class SmartObjectTests
    {
        public SmartObjectTests()
        {
            this.TestSmartObject = new SmartObject(this.Uuid, this.Name, this.Description, this.Token);
        }

        private SmartObject TestSmartObject { get; }

        private string Uuid { get; } = "66783a65-46e2-41bc-9231-e84c474cba43";

        private string Name { get; } = "Test SmartObject";

        private string Description { get; } = "Description of Test SmartObject";

        private string Token { get; } = "29c9e6b2-96a7-44e1-a56e-36fc13f83414";

        public class Constructors : SmartObjectTests
        {
            [Fact]
            public void CheckClass()
            {
                string exepectedClass = "SmartObject";
                Assert.Equal(exepectedClass, this.TestSmartObject.Class);
            }

            [Fact]
            public void CheckUuid()
            {
                Assert.Equal(this.Uuid, this.TestSmartObject.Uuid);
            }

            [Fact]
            public void CheckName()
            {
                Assert.Equal(this.Name, this.TestSmartObject.Name);
            }

            [Fact]
            public void CheckDescription()
            {
                Assert.Equal(this.Description, this.TestSmartObject.Description);
            }

            [Fact]
            public void CheckToken()
            {
                Assert.Equal(this.Token, this.TestSmartObject.Token);
            }
        }
    }
}
