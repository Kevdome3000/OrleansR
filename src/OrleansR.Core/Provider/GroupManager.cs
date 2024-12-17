namespace OrleansR.Core.Provider;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;


/// <summary>
/// Implements the SignalR group manager through OrleansR.
/// <see cref="HubContext"/>
/// </summary>
internal class GroupManager : IGroupManager
{
    private readonly string hubName;
    private readonly IActorProviderFactory providerFactory;


    public GroupManager(string hubName, IActorProviderFactory providerFactory)
    {
        this.hubName = hubName;
        this.providerFactory = providerFactory;
    }


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
