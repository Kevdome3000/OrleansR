namespace OrgnalR.Core.State;

using System.Threading;
using System.Threading.Tasks;


public interface IGroupActor : IMessageAcceptor
{
    Task AddToGroupAsync(string connectionId, CancellationToken cancellationToken = default);


    Task RemoveFromGroupAsync(
        string connectionId,
        CancellationToken cancellationToken = default
    );
}
