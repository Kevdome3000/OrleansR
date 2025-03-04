# OrleansR

[![Actions Status](https://github.com/LiamMorrow/OrleansR/workflows/build/badge.svg)](https://github.com/LiamMorrow/OrleansR/actions)
[![Actions Status](https://github.com/LiamMorrow/OrleansR/workflows/test/badge.svg)](https://github.com/LiamMorrow/OrleansR/actions)  
OrleansR is a backplane for [SignalR core](https://github.com/aspnet/AspNetCore/tree/master/src/SignalR), implemented
through [Orleans](https://github.com/dotnet/orleans)!
It allows your SignalR servers to scale out with all the capacity of Orleans grains.

This is an alternative to the Redis backplane, and [SignalR.Orleans](https://github.com/OrleansContrib/SignalR.Orleans). This implementation does not use Orleans streams at all. This project was born out of issues with deadlocks that occured with Orleans streams, and since [SignalR.Orleans](https://github.com/OrleansContrib/SignalR.Orleans) uses them, issues with signalr messages not going through.

## Getting started
### Compatibility

Orleans `7.0.0` introduced a large breaking change around serialization and other Orleans primitives. As of `^2.0.0`,
OrleansR only supports .net/Orleans `7.0.0` and up. If you need to use this package for an older release of Orleans and
.net (including .netstandard), see the `1.X.X` releases.

### Installing

OrleansR comes in two packages, one for the Orleans Silo, and one for the SignalR application.

#### SignalR

<a href="https://www.nuget.org/packages/OrleansR.Signalr">![OrleansR SignalR](https://img.shields.io/nuget/v/OrleansR.SignalR?logo=SignalR)</a>

```
dotnet add package OrleansR.SignalR
```

#### Orleans Silo

<a href="https://www.nuget.org/packages/OrleansR.OrleansSilo">![OrleansR OrleansSilo](https://img.shields.io/nuget/v/OrleansR.OrleansSilo?logo=OrleansSilo)</a>

```
dotnet add package OrleansR.OrleansSilo
```

### Configuring

OrleansR can be configured via extension methods on both the Orleans client/silo builders, and the SignalR builder.

#### SignalR

Somewhere in your `Startup.cs` (or wherever you configure your SignalR server), you will need to add an extension method
to the SignalR builder. The extension method lives in the `OrleansR.SignalR` namespace, so be sure to add a using for
that namespace.

```c#
using OrleansR.SignalR;
class Startup {
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSignalR()
                .UseOrleansR();
    }
}
```

Next, Orleans needs to know how to serialize your Hub Requests and Responses.
The easiest way to achieve this is to annotate your request/response classes with the `GenerateSerializer` attribute, as you would with any of your normal grain models.

Example:

```c#
[GenerateSerializer]
public record SendMessageRequest(string ChatName, string SenderName, string Message);

// Usage in a hub
public class ChatHub : Hub<IChatClient>
{

    public async Task SendMessage(SendMessageRequest request)
}
```

Please see the [example directory](example) for a fully working solution.

#### Orleans Silo

Wherever you configure your orleans Silo, you will need to configure OrleansR's grains. This is again accomplished by an
extension method, however there are two different modes. For development, it is easiest to use the
`AddOrleansRWithMemoryGrainStorage` extension method, which registers the storage providers for the grains with memory
storage. This is undesirable for production as if the silo dies, the information on connections in which groups is lost.

For production usage it is best to configure actual persistent storage for `OrleansR_USER_STORAGE`,
`OrleansR_GROUP_STORAGE`, and `MESSAGE_STORAGE_PROVIDER`, then use the `AddOrleansR` extension method.

Both of these methods are found in the `OrleansR.Silo` namespace.

##### Development

```c#
var builder = new SiloHostBuilder()
/* Your other configuration options */
// Note here we use the memory storage option.
// This is good for quick development, but we should register proper storage for production
                .AddOrleansRWithMemoryGrainStorage()
```

##### Production

```c#
var builder = new SiloHostBuilder()
/* Your other configuration options */
// Note here we specify the storage we will use for group and user membership
                .ConfigureServices(services =>
                {
                    services.AddSingletonNamedService<IGrainStorage, YourStorageProvider>(Extensions.USER_STORAGE_PROVIDER);
                    services.AddSingletonNamedService<IGrainStorage, YourStorageProvider>(Extensions.GROUP_STORAGE_PROVIDER);
                    services.AddSingletonNamedService<IGrainStorage, YourStorageProvider>(Extensions.MESSAGE_STORAGE_PROVIDER);
                })
                .AddOrleansR()
```

And that's it! Your SignalR server will now use the OrleansR backplane to send messages, and maintain groups / users.

## Sending Messages to clients from grains

Sometimes it is useful to send messages to clients from outside of the Hub. SignalR exposes an interface `IHubContext<T>` for this mechanism inside of the server apps which expose SignalR hubs.

However, in the context of an orleans app, this requirement might still be necessary from the Silo host. To facilitate
this, OrleansR exposes a interface: [`IHubContextProvider`](/src/OrleansR.Core/Provider/HubContextProvider.cs).

To send messages to connected clients in a hub, simply inject this interface into your grain (or service).
It exposes methods for getting clients by group/user/connectionID. You can then call `SendAsync` to send them a message.
Note that `SendAsync` is an extension method provided by the `Microsoft.AspNetCore.SignalR` namespace.

Example:

```csharp
class MyGrain : IMyGrain{

    private readonly IHubContextProvider hubContextProvider;

    constructor(IHubContextProvider hubContextProvider)
    {
        this.hubContextProvider = hubContextProvider;
    }

    async Task MyGrainMethod()
    {
        // Do stuff
        // ...
        // Send message to all connected clients in "MyHub"
        await hubContextProvider
            .GetHubContext<IMyHub>()
            .Clients
            .All // can also use Group, or User, or Connection
            .SendAsync("MyClientMethod", new MyClientMethodRequest("Sent a message from a grain!"));
    }
}
```

Alternatively, if your application has defined interfaces for strongly typed client methods, you can use the generic form:

```csharp
interface IMyClient{
    Task MyClientMethod(MyClientMethodRequest request);
}

class MyGrain : IMyGrain{

    private readonly IHubContextProvider hubContextProvider;

    constructor(IHubContextProvider hubContextProvider)
    {
        this.hubContextProvider = hubContextProvider;
    }

    async Task MyGrainMethod()
    {
        // Do stuff
        // ...
        // Send message to all connected clients in "MyHub"
        await hubContextProvider
            .GetHubContext<IMyHub, IMyClient>()
            .Clients
            .All // can also use Group, or User, or Connection
            .MyClientMethod(new MyClientMethodRequest("Sent a message from a grain!"));
    }
}
```

# Examples

Examples can be found in the [example directory](example)
The current example is a chat room which uses grains to store the messages, and OrleansR as a SignalR backplane. React
frontend.

# Contributing

Contributions are welcome! Simply fork the repository, and submit a PR. If you have an issue, feel free to submit an issue :)
