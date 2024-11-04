namespace OrleansR.Backplane.GrainInterfaces;

using System.Threading.Tasks;
using Core.Provider;
using Orleans;

[Alias("OrleansR.Backplane.GrainInterfaces.IUserActorGrain")]
public interface IUserActorGrain : IGrainWithStringKey
{
    Task AddToUserAsync(string connectionId, GrainCancellationToken cancellationToken);
    Task RemoveFromUserAsync(string connectionId, GrainCancellationToken cancellationToken);
    Task AcceptMessageAsync(AnonymousMessage message, GrainCancellationToken cancellationToken);

}
