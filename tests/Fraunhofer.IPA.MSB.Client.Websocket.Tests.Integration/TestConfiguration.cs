// <copyright file="TestConfiguration.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace Fraunhofer.IPA.MSB.Client.Websocket.Tests.Integration
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Serilog;

    public class TestConfiguration
    {
        // private const string Profile = "OpenSourceMsb";
        private const string Profile = "Local";

        private const string MsbWebsocketInterfaceUrlEnvName = "MSB_WEBSOCKET_INTERFACE_URL";

        private const string MsbSmartObjectMgmtUrlEnvName = "MSB_SMARTOBJECTMGMT_URL";

        private const string MsbIntegrationDesignMgmtUrlEnvKey = "MSB_INTEGRATIONDESIGNMGMT_URL";

        static TestConfiguration()
        {
            var envVars = Environment.GetEnvironmentVariables();

            if (envVars.Contains(MsbWebsocketInterfaceUrlEnvName) && envVars.Contains(MsbSmartObjectMgmtUrlEnvName) && envVars.Contains(MsbIntegrationDesignMgmtUrlEnvKey))
            {
                Log.Information("Environment variables for MSB URLs are set -> Using values of environment variables");
            }
            else
            {
                Log.Information("Environment variables for MSB URLs are not set  -> Using values of launchSettings.json");
                using (var file = File.OpenText("Properties/launchSettings.json"))
                {
                    var reader = new JsonTextReader(file);
                    var jObject = JObject.Load(reader);

                    var variables = jObject["profiles"]
                        .Value<JObject>()[Profile]
                        .Value<JObject>()["environmentVariables"]
                        .Children<JProperty>()
                        .ToList();

                    foreach (var variable in variables)
                    {
                        Environment.SetEnvironmentVariable(variable.Name, variable.Value.ToString());
                    }
                }
            }

            if (Environment.GetEnvironmentVariable(MsbWebsocketInterfaceUrlEnvName) is string websocketInterfaceEnvVarValue)
            {
                MsbWebsocketInterfaceUrl = websocketInterfaceEnvVarValue;
                Log.Debug($"{MsbWebsocketInterfaceUrlEnvName}={MsbWebsocketInterfaceUrl}");
            }

            if (Environment.GetEnvironmentVariable(MsbSmartObjectMgmtUrlEnvName) is string smartObjectMgmtEnvVarValue)
            {
                MsbSmartObjectMgmtUrl = smartObjectMgmtEnvVarValue;
                Log.Debug($"{MsbSmartObjectMgmtUrlEnvName}={MsbSmartObjectMgmtUrl}");
            }

            if (Environment.GetEnvironmentVariable(MsbIntegrationDesignMgmtUrlEnvKey) is string integrationDesignMgmtEnvVarValue)
            {
                MsbIntegrationDesignMgmtUrl = integrationDesignMgmtEnvVarValue;
                Log.Debug($"{MsbIntegrationDesignMgmtUrlEnvKey}={MsbIntegrationDesignMgmtUrl}");
            }
        }

        public static string MsbWebsocketInterfaceUrl { get; private set; } = "ws://localhost:8085";

        public static string MsbSmartObjectMgmtUrl { get; private set; } = "http://localhost:8081";

        public static string MsbIntegrationDesignMgmtUrl { get; private set; } = "http://localhost:8082";
    }
}
