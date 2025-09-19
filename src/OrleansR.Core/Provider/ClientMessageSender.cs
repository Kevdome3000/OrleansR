namespace OrleansR.Core.Provider;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using State;


/// <summary>
/// A class which can be used to send messages to connected clients.
/// <see cref="HubContext"> </see>
/// </summary>
internal sealed class ClientMessageSender(
    IMessageAcceptor messageAcceptor,
    IMessageArgsSerializer serializer,
    ISet<string> excluding) : IClientProxy
{


    public Task SendCoreAsync(
        string methodName,
        object?[] parameters,
        CancellationToken cancellationToken = default
    ) => messageAcceptor.AcceptMessageAsync(
        new AnonymousMessage(
            excluding,
            new MethodMessage(methodName, serializer.Serialize(parameters))
        ),
        cancellationToken
    );
}
