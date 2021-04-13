// <copyright file="MsbPropertiesTests.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using System;
    using Fraunhofer.IPA.MSB.Client.API.Configuration;
    using Xunit;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:Elements should be documented", Justification = "<Ausstehend>")]
    public class MsbPropertiesTests
    {
        public MsbPropertiesTests()
        {
        }

        public class FromPropertiesFile : MsbPropertiesTests
        {
            [Fact]
            public void Read()
            {
                var msbProperties = MsbProperties.ReadFromPropertiesFile("application.properties");
                Assert.Equal("ws://localhost:8085", msbProperties.Url);
                Assert.Equal("SmartObject", msbProperties.ServiceType);
                Assert.Equal("057f5eb2-cc23-4e8a-99e1-abb8f63ca3a4", msbProperties.Uuid);
                Assert.Equal("C# Sample Client", msbProperties.Name);
                Assert.Equal("Description of C# Sample Client", msbProperties.Description);
                Assert.Equal("ecd4c916-e98c-40db-ba1c-b39050183540", msbProperties.Token);
            }
        }

        public class FromEnvironmentVariables : MsbPropertiesTests
        {
            [Fact]
            public void Read()
            {
                Environment.SetEnvironmentVariable("MSB_URL", "ws://localhost:8085");
                Environment.SetEnvironmentVariable("MSB_TYPE", "SmartObject");
                Environment.SetEnvironmentVariable("MSB_UUID", "057f5eb2-cc23-4e8a-99e1-abb8f63ca3a4");
                Environment.SetEnvironmentVariable("MSB_NAME", "C# Sample Client");
                Environment.SetEnvironmentVariable("MSB_DESCRIPTION", "Description of C# Sample Client");
                Environment.SetEnvironmentVariable("MSB_TOKEN", "ecd4c916-e98c-40db-ba1c-b39050183540");

                var msbProperties = MsbProperties.ReadFromEnvironmentVariables();
                Assert.Equal("ws://localhost:8085", msbProperties.Url);
                Assert.Equal("SmartObject", msbProperties.ServiceType);
                Assert.Equal("057f5eb2-cc23-4e8a-99e1-abb8f63ca3a4", msbProperties.Uuid);
                Assert.Equal("C# Sample Client", msbProperties.Name);
                Assert.Equal("Description of C# Sample Client", msbProperties.Description);
                Assert.Equal("ecd4c916-e98c-40db-ba1c-b39050183540", msbProperties.Token);
            }
        }
    }
}
