# How to use MSB C# client library

## Table of contents
1. [Prerequisites](#Prerequisites)
2. [Communication Principle](#Communication-Principle)
3. [Self-description of service](#Self-description-of-service)
    1. [Create new Application or SmartObject](#Create-new-Application-or-SmartObject)
	2. [Add Events](#Add-Events)
	3. [Add Functions](#Add-Functions)
	4. [Add Configuration Parameters](#Add-Configuration-Parameters)
4. [Connect to MSB](#Connect-to-MSB)
5. [Register self-description](#Register-self-description)
6. [Event publishing](#Event-publishing)
7. [Function call handling](#Function-call-handling)
8. [Logging](#Logging)

## Prerequisites

* [.NET](https://dotnet.microsoft.com/download) installed
    *  If you are using .NET Framework make sure the .csproj is in the [new format](https://docs.microsoft.com/en-gb/dotnet/core/tools/csproj), else you won't be able to install the nuget package in projects that use the old .csproj format. See [Nate McMaster's blog](https://natemcmaster.com/blog/2017/03/09/vs2015-to-vs2017-upgrade/) for upgrade information.
* MSB client api package as package reference in your solution
* MSB websocket client nuget package as package reference in your solution

Install nuget packages

```sh
nuget install Fraunhofer.IPA.MSB.Client.Websocket
```

Namespaces of library

```cs
Fraunhofer.IPA.MSB.Client.API
Fraunhofer.IPA.MSB.Client.Websocket
```

## Communication Principle

1. [Create self-description of your Service (SmartObject or Application)](#Self-description-of-service)
2. [Connect `MsbClient` to your MSB instance](#Connect-to-MSB)
3. [Register self-description via MSB client](#Register-self-description)
4. [Publish events](#Event-publishing) or [receive function calls](#Function-call-handling)


## Self-description of service

The figure below shows a minimal required `self-description model` of an Application / SmartObject. An  Application / SmartObject is a classification of a Service and dervies from it. Each service needs an UUID and a token. The UUID is competent for identification and the token is used to verify the Application / SmartObject for its owner on the MSB side. Generate uuids and tokens e.g. by a tool like https://www.uuidgenerator.net/.

![Self Description](images/SelfDescription.png)

### Create new Application or SmartObject

#### Alternative 1 - By application.properties

Add the main description by adding an `application.properties` file to the root of your project:

```sh
msb.url=ws://localhost:8085
msb.uuid=76499d88-34cf-4836-8cc1-7e0d9c54dacx
msb.name=YourSmartObjectName
msb.description=YourSmartObjectDesc
msb.token=5e0d9c54dacx
msb.type=SmartObject
```

You can read the file in your application using the `MsbProperties` class.
```csharp
 MsbProperties myMsbProperties = MsbProperties.ReadFromPropertiesFile("application.properties");
```
Now you can access the properties defined in the file as properties of the `myMsbProperties` object:

```csharp
SmartObject mySmartObject = new SmartObject(
                myMsbProperties.Uuid,
                myMsbProperties.Name,
                myMsbProperties.Description,
                myMsbProperties.Token);
```
or by passing the whole MsbProperties object to the constructor:
```csharp
SmartObject mySmartObject = new SmartObject(myMsbProperties);
```

#### Alternative 2 - By environment variables

Add the main description by defining environment variables:

```sh
MSB_URL=ws://localhost:8085
MSB_UUID=76499d88-34cf-4836-8cc1-7e0d9c54dacx
MSB_NAME=YourSmartObjectName
MSB_DESCRIPTION=YourSmartObjectDesc
MSB_TOKEN=5e0d9c54dacx
MSB_TYPE=SmartObject
```

You can read the environment variables in your application using the `MsbProperties` class.
```csharp
 MsbProperties myMsbProperties = MsbProperties.ReadFromEnvironmentVariables();
```
Now you can access the properties defined in the environment variables as properties of the `myMsbProperties` object:

```csharp
SmartObject mySmartObject = new SmartObject(
                myMsbProperties.Uuid,
                myMsbProperties.Name,
                myMsbProperties.Description,
                myMsbProperties.Token);
```
or by passing the whole MsbProperties object to the constructor:
```csharp
SmartObject mySmartObject = new SmartObject(myMsbProperties);
```

#### Alternative 3 - By constructor

If you do not provide an application.properties file, use the constructor 
to define the basic self description.

```csharp
SmartObject mySmartObject = new SmartObject(
                "f9f57cc2-00af-408f-9ba7-5b127e5a4822",
                "C# Sample SmartObject",
                "Description of C# Sample SmartObject",
                "98e2483b-ca03-46c6-bde8-d9f51be7f7da");
);
```

### Add Events

Add `events` to your service which can be send to MSB.

#### Add event with primitive data type

```csharp
// Create event object
Event myEvent = new Event(
                "Id of my simple event",
                "Name of my simple event",
                "Description of my simple event",
                typeof(string));
// Add object to self-description of SmartObject
mySmartObject.AddEvent(myEvent);
```

#### Add event with complex data type

A complex data type can be modeld as class. The library will convert it to the appropriate MSB data format:

```csharp
// Class that defines the complex data type
public class MyComplexEvent
{
    public string StringProperty { get; }

    public int IntProperty { get; }

    public string[] StringArrayProperty { get; }

    public List<float> FloatListProperty { get; }
}

// Add event to self-description of SmartObject
Event myEvent = new Event(
        "Id of my complex event",
        "Name of my complex event",
        "Description of my complex event",
        typeof(MyComplexEvent));
mySmartobject.AddEvent(myEvent);
```

### Add Functions

Add `functions` and their implementations your SmartObject / Application is able to handle. If a MSB function callback was received, the corresponding C# method will be called. Additional information of the function call are provided by the `FunctionCallInfo` parameter. FunctionCallInfo is a necessary parameter for all C# MSB functions.

If a function has `response events` it must return the type EventData. Response event are defined as string array of event ids within the `MsbFunction` attribute. If an function with response events is added the corresponding events must be already exist within the self-description.

#### Alternative 1: Add function

Functions can be definined via the `MsbFunctionAttribute`:
```csharp
// Definition of function which will be called via MSB
[MsbFunction(
    Id = "SampleFunction",
    Name = "Sample Function",
    Description = "Description of Sample Function")]
public void SampleMsbFunction(
    [MsbFunctionParameter(Name = "NameOfStringParameterInDataFormat")] string stringParaneter,
    [MsbFunctionParameter(Name = "NameOfIntParameterInDataFormat")] int intParameter,
    FunctionCallInfo functionCallInfo)
{
    Console.WriteLine($"Function Call via MSB: {stringParaneter} | {intParameter}");
}
```
To add the function to the self-description, the `MethodInfo` must be fetched via Reflection.
```csharp
// Get MethodInfo via Reflection
MethodInfo methodInfo = this.GetType().GetRuntimeMethod("SampleMsbFunction", new Type[] { typeof(string), typeof(int), typeof(FunctionCallInfo) });
// Create new Function based on MethodInfo
Function sampleMsbFunction = new Function(methodInfo, this);
// Add function to self-description
mySmartobject.AddFunction(sampleMsbFunction);
```

#### Alternative 2: Add function handler with multiple functions

To add multiple functions at once function handler class can be used. The class must derive from the `AbstractFunctionHandler` class and use the `MsbFunctionHandler` attribute. In the function handler class MSb functions can be definined as shown above:

```csharp
[MsbFunctionHandler(Id = "SimpleFunctionHandlerId")]
public class SimpleFunctionHandler : AbstractFunctionHandler
{
    [MsbFunction(
        Id = "SampleFunction",
        Name = "Sample Function",
        Description = "Description of Sample Function")]
    public void SampleMsbFunction(
        [MsbFunctionParameter(Name = "NameOfStringParameterInDataFormat")] string stringParaneter,
        [MsbFunctionParameter(Name = "NameOfIntParameterInDataFormat")] int intParameter,
        FunctionCallInfo functionCallInfo)
    {
        Console.WriteLine($"Function Call via MSB: {stringParaneter} | {intParameter}");
    }

    [MsbFunction(
        Id = "AnotherFunction",
        Name = "Another Function",
        Description = "Description of Another Function",
        ResponseEvents = new string[] { "ResponseEvent1", "ResponseEvent2" })]
    public EventData SampleMsbFunction(
        FunctionCallInfo functionCallInfo)
    {
        Console.WriteLine($"Function Call via MSB");
        
        return new EventDataBuilder(functionCallInfo.ResponseEvents["ResponseEvent1"])
                .SetEventPriority(EventPriority.HIGH)
                .SetShouldBeCached(true)
                .SetValue(new MyComplexEvent())
                .Build();
    }
}
```
An instance of the defined function handler class can be added directly to the self-description:
```csharp
SimpleFunctionHandler simpleFunctionHandler = new SimpleFunctionHandler();
mySmartobject.AddFunctionHandler(simpleFunctionHandler);
```

### Add Configuration parameters

Configuration parameters are a simple list of key value pairs for a Application / SmartObject. 
It is displayed and can be customized in the MSB UI to change your apps behaviour during runtime.

`Add` condifuration parameters:

```csharp
mySmartobject.AddConfigurationParameter("Parameter1", new ConfigurationParameterValue(123));
mySmartobject.AddConfigurationParameter("Parameter2", new ConfigurationParameterValue("SampleValue"));
```

`Get` configuration parameter (after changed in MSB UI) to change your app behaviour:

```csharp
// Get configuration parameter using name of parameter as key
var parameter1Value = mySmartobject.Configuration.Parameters["Parameter1"];
```

## Connect to MSB

Before interacting with the MSB, the client must connect to the MSB:

```csharp
MsbClient myMsbClient = new MsbClient("wss://SampleMsbInstance");
myMsbClient.ConnectAsync().Wait();
```

### SSL/TLS connection configuration

To enable `SSL/TLS`, you need to specify `wss://` or `https://` in the URL instead of `ws://` or `http://`. There are several properties to modify certificate validation (by default, these are set to False):

| `Property`                        | `Description`                              |
|---------------------------------|------------------------------------------|
| AllowSslCertificateChainErrors  | Allows chain error in SSL certificate    |
| AllowSslCertificateNameMismatch | Allows name mismatich in SSL certificate |
| AllowSslUnstrustedCertificate   | Allows untrusted SSL certificate         |

**Hint**: If you use an IP instead of a url during development, it will be necessary to set `AllowSslCertificateNameMismatch` to true:
```csharp
myMsbClient.AllowSslCertificateNameMismatch = true;
```

### Connection recovery

If connection to the MSB websocket interface is broken the client performs a reconnect. After a reconnect the registration at the MSB will be redone automatically by the client and all cached events will be published.

There are several properties to modfiy reconnect heaviour:

| `Property`                          | `Description`                              |
|-------------------------------------|--------------------------------------------|
| AutoReconnect                       | If set to true the client will automatically reconnect to MSB if connection is broken |
| AutoReconnectIntervalInMilliseconds | Time interval between reconnect attepmts (only relevant if AutoReconnect set to true)  |

## Register self-description

After the connection has been established self-description in the form of Applications or SmartObjects can be registered at the MSB via the client:

```csharp
myMsbClient.RegisterAsync(mySmartObject).Wait();
```

## Event publishing

### Publish event

For publishing an event to an MSB, an `EventData` object must be created. The `EventDataBuilder` allows easy creation of `EventData`:

```csharp
 var eventData = new EventDataBuilder(this.myEvent)
                        .SetCorrelationId("a5fc5da1-e7fa-4f63-bba9-63d07faa9783")
                        .SetEventPriority(EventPriority.HIGH)
                        .SetPublishingDate(DateTime.Now)
                        .SetShouldBeCached(true)
                        .SetValue(new MyComplexEvent())
                        .Build();
```

If not specified `EventData` uses the following default values for properties:

| `Property`      | `Default value`              |
|-----------------|------------------------------|
| CorrelationId   | Automatically generated UUID |
| EventPriority   | EventPriority.LOW            |
| PublishingDate  | DateTime.Now                 |
| ShouldBeCached  | true                         |
| Value           | null                         |

After the EventData have been created they can be publishd for a Application or SmartObject:

```csharp
myMsbClient.PublishAsync(mySmartobject, eventData).Wait();
```

### Event caching

If the `ShouldBeCached` property of `EventData` is set the event will be cached if the connection to MSB is broken. When the connection is recovered the cached events will be published. The cache size can be set via the `EventCacheSizePerService` property. The cached events are accessible trough the `EventCache` property with the service as key and a list of cached `EventData` as value.

## Function call handling

If a MSB function callback was received, the corresponding C# method is called. Additional information of the function call are provided by the `FunctionCallInfo` parameter (which is necessary for all C# MSB functions).

To send a response event an `EventData` object has to be returned. The MSB client will then automatically publish the response event using the correlation id of the function call (if correlation id is set in `EventData` it will be overriden):

```csharp
return new EventDataBuilder(functionCallInfo.ResponseEvents["ResponseEvent1"])
                .SetEventPriority(EventPriority.HIGH)
                .SetShouldBeCached(true)
                .SetValue(new MyComplexEvent())
                .Build();
```

## Logging

The library uses [LibLog](https://github.com/damianh/LibLog) as logging abstraction. This allows every developer to use his or her preferred logging framework as usual.

### Example: Logging with Serilog to Console

Install nuget packages for [Serilog](https://serilog.net):
```csharp
nuget install Serilog
nuget install Serilog.Sinks.Console
```

Configure Logger in your program code:
```csharp
Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console(outputTemplate:
                        "[{Timestamp:yyyy-MM-dd - HH:mm:ss}] [{SourceContext:s}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                    .CreateLogger();
Log.Information("Sample log entry");
```