namespace OrleansR.Backplane.GrainInterfaces;

using System.Threading.Tasks;
using Core.Provider;
using Orleans;


public interface IUserActorGrain : IGrainWithStringKey
{
    Task AddToUserAsync(string connectionId, GrainCancellationToken cancellationToken);
    Task RemoveFromUserAsync(string connectionId, GrainCancellationToken cancellationToken);
    Task AcceptMessageAsync(AnonymousMessage message, GrainCancellationToken cancellationToken);

}
