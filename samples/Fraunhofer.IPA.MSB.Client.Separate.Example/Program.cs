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
        public class funktionen {
            public void Funktion1(string data1, FunctionCallInfo info)
            {
                Console.WriteLine(data1);
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
            myMsbSmartObject.AddEvent(e1);

            var f = new funktionen();
            Function f1 = new Function("function1", "Funktion 1", "Funktion 1", typeof(funktionen).GetMethod("Funktion1"), f);
            myMsbSmartObject.AddFunction(f1);

            // Register stuff internally
            myMsbClient.RegisterAsync(myMsbSmartObject).Wait();

            // Start up interfaces
            myMsbClient.ConnectAsync().Wait();

            // Publish events
            while (true)
            {
                Thread.Sleep(3000);
                EventData eventData_SimpleEvent = new EventDataBuilder(e1).SetValue("TestString").Build();

                myMsbClient.PublishAsync(myMsbSmartObject, eventData_SimpleEvent).Wait();
            }
        }
    }
}
