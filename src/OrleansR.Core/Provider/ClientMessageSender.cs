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
internal sealed class ClientMessageSender : IClientProxy
{
    private readonly IMessageAcceptor messageAcceptor;
    private readonly IMessageArgsSerializer serializer;
    private readonly ISet<string> excluding;


    public ClientMessageSender(
        IMessageAcceptor messageAcceptor,
        IMessageArgsSerializer serializer,
        ISet<string> excluding
    )
    {
        this.messageAcceptor = messageAcceptor;
        this.serializer = serializer;
        this.excluding = excluding;
    }


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
