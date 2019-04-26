// <copyright file="ApplicationTests.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

    public class ApplicationTests
    {
        public ApplicationTests()
        {
            this.TestApplication = new Application(this.ExpectedUuid, this.ExpectedName, this.ExpectedDescription, this.ExpectedToken);
        }

        protected Application TestApplication { get; }

        protected string ExpectedUuid { get; } = "4a82bfff-d148-4e93-b5bc-83d9d8ef7cbb";

        protected string ExpectedName { get; } = "Test Application";

        protected string ExpectedDescription { get; } = "Description of Test Application";

        protected string ExpectedToken { get; } = "34a03d13-f01e-410f-ad27-89ff7e0c8e98";

        public class Constructor : ApplicationTests
        {
            [Fact]
            public void CheckClass()
            {
                string expectedClass = "Application";
                Assert.Equal(expectedClass, this.TestApplication.Class);
            }

            [Fact]
            public void CheckUuid()
            {
                Assert.Equal(this.ExpectedUuid, this.TestApplication.Uuid);
            }

            [Fact]
            public void CheckName()
            {
                Assert.Equal(this.ExpectedName, this.TestApplication.Name);
            }

            [Fact]
            public void CheckDescription()
            {
                Assert.Equal(this.ExpectedDescription, this.TestApplication.Description);
            }

            [Fact]
            public void CheckToken()
            {
                Assert.Equal(this.ExpectedToken, this.TestApplication.Token);
            }
        }
    }
}
