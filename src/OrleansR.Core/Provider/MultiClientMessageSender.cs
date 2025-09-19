namespace OrleansR.Core.Provider;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;


/// <summary>
/// A class which can be used to send messages to multiple connected clients.
/// <see cref="ClientMessageSender"> </see>
/// </summary>
internal sealed class MultiClientMessageSender(IReadOnlyList<IClientProxy> clientProxies) : IClientProxy
{


    public Task SendCoreAsync(
        string methodName,
        object?[] parameters,
        CancellationToken cancellationToken = default
    )
    {
        return Task.WhenAll(
            clientProxies.Select(c => c.SendCoreAsync(methodName, parameters, cancellationToken))
        );
    }
}
