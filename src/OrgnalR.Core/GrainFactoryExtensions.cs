namespace OrgnalR.Core;

using System;
using Microsoft.AspNetCore.SignalR;
using Orleans;
using Provider;


public static class GrainFactoryExtensions
{
    private static IHubContextProvider GetProvider(IGrainFactory grainFactory)
    {
        // Access the provider through the grain's ServiceProvider
        var provider = grainFactory.GetServiceProvider().GetService<IHubContextProvider>();

        if (provider == null)
        {
            throw new InvalidOperationException("OrgnalR services not registered. Call AddOrgnalR() in your Startup.");
        }

        return provider;
    }


    /// <summary>
    /// Gets a hub context for the specified hub type, allowing easy SignalR communication from grains
    /// </summary>
    /// <typeparam name="THub">The hub type</typeparam>
    /// <returns>Hub context for sending messages to connected clients</returns>
    public static IHubContext<THub> GetHub<THub>(this IGrainFactory grainFactory)
        where THub : Hub
    {
        IHubContextProvider provider = GetProvider(grainFactory);
        return (IHubContext<THub>)provider.GetHubContext<THub>();
    }


    /// <summary>
    /// Gets a strongly-typed hub context for the specified hub type
    /// </summary>
    /// <typeparam name="THub">The hub type</typeparam>
    /// <typeparam name="TClient">The strongly-typed hub client interface</typeparam>
    public static IHubContext<THub, TClient> GetHub<THub, TClient>(this IGrainFactory grainFactory)
        where THub : Hub<TClient>
        where TClient : class
    {
        IHubContextProvider provider = GetProvider(grainFactory);
        return (IHubContext<THub, TClient>)provider.GetHubContext<THub, TClient>();
    }


    /// <summary>
    /// Gets a hub context by hub name
    /// </summary>
    /// <param name="hubName">Name of the hub</param>
    public static IHubContext GetHub(this IGrainFactory grainFactory, string hubName)
    {
        IHubContextProvider provider = GetProvider(grainFactory);
        return provider.GetHubContext(hubName);
    }


    /// <summary>
    /// Gets a strongly-typed hub context by hub name
    /// </summary>
    /// <typeparam name="TClient">The strongly-typed hub client interface</typeparam>
    /// <param name="hubName">Name of the hub</param>
    public static IHubContext<Hub<TClient>, TClient> GetHub<TClient>(this IGrainFactory grainFactory, string hubName)
        where TClient : class
    {
        IHubContextProvider provider = GetProvider(grainFactory);
        return provider.GetHubContext<TClient>(hubName);
    }
}
