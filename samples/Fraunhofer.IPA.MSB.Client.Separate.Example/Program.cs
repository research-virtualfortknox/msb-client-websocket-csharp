namespace Fraunhofer.IPA.MSB.Client.Separate.Example
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using Fraunhofer.IPA.MSB.Client.API.Attributes;
    using Fraunhofer.IPA.MSB.Client.API.Configuration;
    using Fraunhofer.IPA.MSB.Client.API.Model;

    class Program
    {
        public class SimpleEvent
        {
            public SimpleEvent()
            {
                this.MyInteger = 2;
                this.MyFloat = 3.9f;
                this.MyDouble = 13849378.423423f;
                this.MyString = "test test";
            }

            public int MyInteger { get; set; }

            public float MyFloat { get; set; }

            public double MyDouble { get; set; }

            public string MyString { get; set; }

            public string[] MyStringArray { get; set; }
        }

        public class funktionen
        {
            public void Funktion1(string data1, FunctionCallInfo info)
            {
                Console.WriteLine("Funktion1: " + data1);
            }

            public void Funktion2(string data1, int MyInteger, FunctionCallInfo info)
            {
                Console.WriteLine("Funktion2: " + data1 + " " + MyInteger);
            }

            public void Funktion3(SimpleEvent ev, FunctionCallInfo info)
            {
                Console.WriteLine("Funktion3: " + ev.MyString);
            }
        }

        static void Main(string[] args)
        {
            const string MyMsbSmartObjectUuid = "1a17b5e3-3a6a-4e62-97b0-82cfdd1cc818";
            const string MyMsbSmartObjectName = "C# Sample SmartObject";
            const string MyMsbSmartObjectDescription = "Description of my C# sample SmartObject";
            const string MyMsbSmartObjectToken = "30e47482-c140-49a9-a79f-6f2396d8e0ab";

            // Create a new MsbClient which allows SmartObjects and Applications to communicate with the MSB
            Separate.SeparateClient myMsbClient = new Separate.SeparateClient();

            // Create the self description of a sample SmartObject and a sample Application
            SmartObject myMsbSmartObject = new SmartObject(MyMsbSmartObjectUuid, MyMsbSmartObjectName, MyMsbSmartObjectDescription, MyMsbSmartObjectToken);

            myMsbSmartObject.Configuration.LoadFromFile("Configuration.json");

            // Add events
            Event e1 = new Event("event1", "Name of simple event", "Event with simple data format", typeof(string));
            Event e2 = new Event("event2", "Name of event", "Event with data format", typeof(SimpleEvent));
            myMsbSmartObject.AddEvent(e1);
            myMsbSmartObject.AddEvent(e2);

            var f = new funktionen();
            Function f1 = new Function("function1", "Funktion 1", "Funktion 1", typeof(funktionen).GetMethod("Funktion1"), f);
            Function f2 = new Function("function2", "Funktion 2", "Funktion 2", typeof(funktionen).GetMethod("Funktion2"), f);
            Function f3 = new Function("function3", "Funktion 3", "Funktion 3", typeof(funktionen).GetMethod("Funktion3"), f);
            myMsbSmartObject.AddFunction(f1);
            myMsbSmartObject.AddFunction(f2);
            myMsbSmartObject.AddFunction(f3);

            // Register stuff internally
            myMsbClient.RegisterAsync(myMsbSmartObject).Wait();

            // Start up interfaces
            myMsbClient.ConnectAsync().Wait();

            // Publish events
            while (true)
            {
                Thread.Sleep(3000);
                EventData eventData_e1 = new EventDataBuilder(e1).SetValue("TestString").Build();

                myMsbClient.PublishAsync(myMsbSmartObject, eventData_e1).Wait();

                EventData eventData_e2 = new EventDataBuilder(e2).SetValue(new SimpleEvent()).Build();

                myMsbClient.PublishAsync(myMsbSmartObject, eventData_e2).Wait();
            }
        }
    }
}
