// <copyright file="BaseTests.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace Fraunhofer.IPA.MSB.Client.Tests.Shared
{
    using Serilog;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class BaseTests
    {
        private const string LogOutputTemplate = "[{Timestamp:yyyy-MM-dd - HH:mm:ss}] [{SourceContext:s}] [{Level:u3}] {Message:lj}{NewLine}{Exception}";

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTests"/> class.
        /// </summary>
        public BaseTests()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTests"/> class.
        /// </summary>
        /// <param name="output">The <see cref="ITestOutputHelper"/> for this instance.</param>
        public BaseTests(ITestOutputHelper output)
        {
            this.Output = output as TestOutputHelper;

            this.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Xunit(output, outputTemplate: LogOutputTemplate)
                .CreateLogger();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Xunit(output, outputTemplate: LogOutputTemplate)
                .CreateLogger();
        }

        protected ILogger Logger { get; }

        protected TestOutputHelper Output { get; }
    }
}
