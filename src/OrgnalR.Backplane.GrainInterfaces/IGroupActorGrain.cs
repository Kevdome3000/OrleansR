namespace OrgnalR.Backplane.GrainInterfaces;

using System.Threading.Tasks;
using Core.Provider;
using Orleans;


public interface IGroupActorGrain : IGrainWithStringKey
{
    Task AddToGroupAsync(string connectionId, GrainCancellationToken cancellationToken);
    Task RemoveFromGroupAsync(string connectionId, GrainCancellationToken cancellationToken);
    Task AcceptMessageAsync(AnonymousMessage message, GrainCancellationToken cancellationToken);

}
