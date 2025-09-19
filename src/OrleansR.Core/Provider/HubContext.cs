namespace OrleansR.Core.Provider;

using Microsoft.AspNetCore.SignalR;


/// <summary>
/// Implements the SignalR IHubContext using the OrleansR services - allows sending messages to connected clients from within grains
/// </summary>
internal sealed class HubContext(
    string hubName,
    IActorProviderFactory providerFactory,
    IMessageArgsSerializer serializer) : IHubContext
{


    public IHubClients Clients => new HubClients(hubName, providerFactory, serializer);

    public IGroupManager Groups => new GroupManager(hubName, providerFactory);
}
