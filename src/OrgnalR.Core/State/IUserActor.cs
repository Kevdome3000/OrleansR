namespace OrgnalR.Core.State;

using System.Threading;
using System.Threading.Tasks;


public interface IUserActor : IMessageAcceptor
{
    Task AddToUserAsync(string connectionId, CancellationToken cancellationToken = default);


    Task RemoveFromUserAsync(
        string connectionId,
        CancellationToken cancellationToken = default
    );
}
