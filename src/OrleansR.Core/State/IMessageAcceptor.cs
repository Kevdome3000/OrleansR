namespace OrleansR.Core.State;

using System.Threading;
using System.Threading.Tasks;
using Provider;


public interface IMessageAcceptor
{
    Task AcceptMessageAsync(
        AnonymousMessage targetedMessage,
        CancellationToken cancellationToken = default
    );
}
