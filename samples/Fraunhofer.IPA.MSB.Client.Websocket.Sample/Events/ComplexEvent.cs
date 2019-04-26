// <copyright file="ComplexEvent.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    public class ComplexEvent
    {
        public ComplexEvent()
        {
            // SimpleEvents = new List<SimpleEvent>();
            this.SimpleEv = new SimpleEvent();
            this.SomeInt = 1337;
            this.Si2 = new SimpleEvent() { MyString = "TEST STRING FROM 2" };
            this.AllTypesObject = new AllDataTypesEvent();
        }

        public SimpleEvent SimpleEv { get; set; }

        public SimpleEvent Si2 { get; set; }

        public int SomeInt { get; set; }

        public AllDataTypesEvent AllTypesObject { get; set; }

        public long[] MyIntArray { get; set; }
    }
}
