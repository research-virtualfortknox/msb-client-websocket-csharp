﻿// <copyright file="ComplexEvent.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using System.Collections.Generic;

    public class ComplexEvent
    {
        public SimpleEvent SimpleEvent { get; } = new SimpleEvent();

        public List<SimpleEvent> SimpleEventList { get; } = new List<SimpleEvent>() { new SimpleEvent(), new SimpleEvent() };

        public int SomeInt { get; } = 1337;

        public AllDataTypesEvent AllDataTypesObject { get; } = new AllDataTypesEvent();

        public int[] MyIntArray { get; set; } = new int[] { 1, 2, 3, 4, 5 };
    }
}
