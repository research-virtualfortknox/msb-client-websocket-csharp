// <copyright file="ApplicationPropertiesTests.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace Fraunhofer.IPA.MSB.Client.API.Tests.Unit.Configuration
{
    using Fraunhofer.IPA.MSB.Client.API.Configuration;
    using Xunit;

    public class ApplicationPropertiesTests
    {
        public ApplicationPropertiesTests()
        {
        }

        public class Read : ApplicationPropertiesTests
        {
            [Fact]
            public void ReadApplicationPropertiesFile()
            {
                var applicationProperties = ApplicationProperties.Read();
                Assert.Equal("ws://localhost:8085", applicationProperties.Url);
                Assert.Equal("SmartObject", applicationProperties.ServiceType);
                Assert.Equal("057f5eb2-cc23-4e8a-99e1-abb8f63ca3a4", applicationProperties.Uuid);
                Assert.Equal("C# Sample Client", applicationProperties.Name);
                Assert.Equal("Description of C# Sample Client", applicationProperties.Description);
                Assert.Equal("ecd4c916-e98c-40db-ba1c-b39050183540", applicationProperties.Token);
            }
        }
    }
}
