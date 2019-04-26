// <copyright file="AllDataTypesEvent.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace Fraunhofer.IPA.MSB.Client.Websocket.Sample.Events
{
    using System;
    using System.Collections.Generic;

    public class AllDataTypesEvent
    {
        public AllDataTypesEvent()
        {
            this.SimpleString = "simpleStringObject";

            this.SimpleByte = 0x13;
            this.SimpleChar = 'a';

            this.SimpleInt16 = 0x1337;
            this.SimpleInt32 = int.MaxValue;
            this.SimpleInt64 = long.MaxValue;

            this.SimpleFloat = (float)Math.Sin(13.9);
            this.SimpleDouble = (double)Math.Cos(13.9);

            this.SimpleDateTime = DateTime.Now;

            this.StringList = new List<string>() { "simpleStringListEntry " };
            this.DictionaryStringDouble = new Dictionary<string, double>() { { "simpleDictKey", 1337.0f } };
        }

        public string SimpleString { get; set; }

        public byte SimpleByte { get; set; }

        public char SimpleChar { get; set; }

        public short SimpleInt16 { get; set; }

        public int SimpleInt32 { get; set; }

        public long SimpleInt64 { get; set; }

        public float SimpleFloat { get; set; }

        public double SimpleDouble { get; set; }

        public DateTime SimpleDateTime { get; set; }

        public List<string> StringList { get; set; }

        public Dictionary<string, double> DictionaryStringDouble { get; set; }
    }
}
