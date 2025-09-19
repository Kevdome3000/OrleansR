namespace OrleansR.Core.Provider;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;


/// <summary>
/// Implements the SignalR group manager through OrleansR.
/// <see cref="HubContext"/>
/// </summary>
internal class GroupManager(string hubName, IActorProviderFactory providerFactory) : IGroupManager
{


    public Task AddToGroupAsync(
        string connectionId,
        string groupName,
        CancellationToken cancellationToken = default
    ) => providerFactory
        .GetGroupActor(hubName, groupName)
        .AddToGroupAsync(connectionId, cancellationToken);


    public Task RemoveFromGroupAsync(
        string connectionId,
        string groupName,
        CancellationToken cancellationToken = default
    ) => providerFactory
        .GetGroupActor(hubName, groupName)
        .RemoveFromGroupAsync(connectionId, cancellationToken);
}
