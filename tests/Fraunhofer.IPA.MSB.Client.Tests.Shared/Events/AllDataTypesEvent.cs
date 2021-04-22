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

namespace Fraunhofer.IPA.MSB.Client.Websocket.IntegrationTest.Events
{
    using System;
    using System.Collections.Generic;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:Elements should be documented", Justification = "<Ausstehend>")]
    public class AllDataTypesEvent
    {
        public bool SimpleBool { get; } = false;

        public string SimpleString { get; } = "simpleStringObject";

        public char SimpleChar { get; } = 'a';

        public byte SimpleByte { get; } = 0x13;

        public sbyte SimpleSByte { get; } = 0x10;

        public short SimpleInt16 { get; } = 0x1337;

        public ushort SimpleUInt16 { get; } = 123;

        public int SimpleInt32 { get; } = int.MaxValue;

        public uint SimpleUInt32 { get; } = 1234;

        public long SimpleInt64 { get; } = long.MaxValue;

        public ulong SimpleUInt64 { get; } = 123512313123;

        public float SimpleFloat { get; } = (float)Math.Sin(13.9);

        public double SimpleDouble { get; } = (double)Math.Cos(13.9);

        public decimal SimpleDecimal { get; } = 4352353245.2345324532632673253425123M;

        public DateTime SimpleDateTime { get; } = DateTime.Now;

        public string[] StringArray { get; } = new string[] { "string1", "string2" };

        public byte[] ByteArray { get; } = new byte[] { 0x02, 0x13, 0x10, 0x09 };

        public List<string> StringList { get; } = new List<string>() { "simpleStringListEntry " };

        public Dictionary<string, double> DictionaryStringDouble { get; } = new Dictionary<string, double>() { { "simpleDictKey", 1337.0f } };
    }
}
