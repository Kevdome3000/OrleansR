namespace OrgnalR.Core.Provider;

using System.Threading;
using System.Threading.Tasks;


public interface IMessageObserver
{
    Task SendAllMessageAsync(AnonymousMessage allMessage, CancellationToken cancellationToken = default);
    Task SendAddressedMessageAsync(AddressedMessage msg, CancellationToken cancellationToken = default);
}
