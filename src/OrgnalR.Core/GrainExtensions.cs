namespace OrgnalR.Core;

using Microsoft.AspNetCore.SignalR;
using Orleans;


public static class GrainExtensions
{
    /// <summary>
    /// Extension method for grains to easily access SignalR hubs
    /// </summary>
    public static IHubContext<THub> GetHub<THub>(this Grain grain)
        where THub : Hub => grain.GrainFactory.GetHub<THub>();


    /// <summary>
    /// Extension method for grains to easily access strongly-typed SignalR hubs
    /// </summary>
    public static IHubContext<THub, TClient> GetHub<THub, TClient>(this Grain grain)
        where THub : Hub<TClient>
        where TClient : class => grain.GrainFactory.GetHub<THub, TClient>();


    /// <summary>
    /// Extension method for grains to access hubs by name
    /// </summary>
    public static IHubContext GetHub(this Grain grain, string hubName) => grain.GrainFactory.GetHub(hubName);


    /// <summary>
    /// Extension method for grains to access strongly-typed hubs by name
    /// </summary>
    public static IHubContext<Hub<TClient>, TClient> GetHub<TClient>(this Grain grain, string hubName)
        where TClient : class => grain.GrainFactory.GetHub<TClient>(hubName);
}
