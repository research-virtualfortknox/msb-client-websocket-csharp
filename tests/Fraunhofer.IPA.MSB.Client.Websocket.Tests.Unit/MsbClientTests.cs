// <copyright file="MsbClientTests.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace Fraunhofer.IPA.MSB.Client.Websocket.Tests.Unit
{
    using Fraunhofer.IPA.MSB.Client.Tests.Shared;
    using Xunit;
    using Xunit.Abstractions;

    public class MsbClientTests : BaseTests
    {
        public MsbClientTests()
        {
        }

        public MsbClientTests(ITestOutputHelper output)
                : base(output)
        {
        }

        public class Constructors : MsbClientTests
        {
            public Constructors(ITestOutputHelper output)
                : base(output)
            {
            }

            [Fact]
            public void ConstructorWithUrl_http()
            {
                string expectedMsbUrl = "http://localhost:8085";

                MsbClient testMsbClient = new MsbClient(expectedMsbUrl);
                Assert.Equal(expectedMsbUrl.Replace("http://", "ws://"), testMsbClient.MsbUrl);
                Assert.False(testMsbClient.IsSslEnabled);
            }

            [Fact]
            public void ConstructorWithUrl_ws()
            {
                string expectedMsbUrl = "ws://localhost:8085";

                MsbClient testMsbClient = new MsbClient(expectedMsbUrl);
                Assert.Equal(expectedMsbUrl, testMsbClient.MsbUrl);
                Assert.False(testMsbClient.IsSslEnabled);
            }

            [Fact]
            public void ConstructorWithUrl_https()
            {
                string expectedMsbUrl = "https://localhost:8085";

                MsbClient testMsbClient = new MsbClient(expectedMsbUrl);
                Assert.Equal(expectedMsbUrl.Replace("https://", "wss://"), testMsbClient.MsbUrl);
                Assert.True(testMsbClient.IsSslEnabled);
            }

            [Fact]
            public void ConstructorWithUrl_wss()
            {
                string expectedMsbUrl = "wss://localhost:8085";

                MsbClient testMsbClient = new MsbClient(expectedMsbUrl);
                Assert.Equal(expectedMsbUrl, testMsbClient.MsbUrl);
                Assert.True(testMsbClient.IsSslEnabled);
            }

            [Fact]
            public void ConstructorWithSingleArguments_ws()
            {
                string expectedMsbUrl = "ws://localhost:8085/";

                MsbClient testMsbClient = new MsbClient("localhost", 8085, false, string.Empty);
                Assert.Equal(expectedMsbUrl, testMsbClient.MsbUrl);
                Assert.False(testMsbClient.IsSslEnabled);
            }

            [Fact]
            public void ConstructorWithSingleArguments_wss()
            {
                string expectedMsbUrl = "wss://localhost:8085/";

                MsbClient testMsbClient = new MsbClient("localhost", 8085, true, string.Empty);
                Assert.Equal(expectedMsbUrl, testMsbClient.MsbUrl);
                Assert.True(testMsbClient.IsSslEnabled);
            }
        }

        public class HeatbeatIntervalInMilliseconds : MsbClientTests
        {
            [Fact]
            public void CheckProperty()
            {
                int expectedHeartbeatIntervalInMilliseconds = 30000;

                MsbClient testMsbClient = new MsbClient("http://localhost:8085");
                testMsbClient.HeatbeatIntervalInMilliseconds = expectedHeartbeatIntervalInMilliseconds;
                Assert.Equal(expectedHeartbeatIntervalInMilliseconds, testMsbClient.HeatbeatIntervalInMilliseconds);
            }
        }

        public class AutoReconnectIntervalInMilliseconds : MsbClientTests
        {
            [Fact]
            public void IntervalGreaterThenMiniumInterval()
            {
                int expectedAutoReconnectIntervalInMiliseconds = 30000;

                MsbClient testMsbClient = new MsbClient("http://localhost:8085");
                testMsbClient.AutoReconnectIntervalInMilliseconds = expectedAutoReconnectIntervalInMiliseconds;
                Assert.Equal(expectedAutoReconnectIntervalInMiliseconds, testMsbClient.AutoReconnectIntervalInMilliseconds);
            }

            [Fact]
            public void IntervalLowerThenMiniumInterval()
            {
                int expectedAutoReconnectIntervalInMiliseconds = 10;

                MsbClient testMsbClient = new MsbClient("http://localhost:8085");
                testMsbClient.AutoReconnectIntervalInMilliseconds = expectedAutoReconnectIntervalInMiliseconds;
                Assert.Equal(MsbClient.MinAutoReconnectIntervalInMilliseconds, testMsbClient.AutoReconnectIntervalInMilliseconds);
            }
        }
    }
}
