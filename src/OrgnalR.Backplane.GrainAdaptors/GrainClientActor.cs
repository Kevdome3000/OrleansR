namespace OrgnalR.Backplane.GrainAdaptors;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.Provider;
using Core.State;
using GrainInterfaces;
using Orleans;


public class GrainClientActor : IMessageAcceptor
{
    private readonly string hubName;
    private readonly IClientGrain clientGrain;


    public GrainClientActor(string hubName, IClientGrain clientGrain)
    {
        this.hubName = hubName;
        this.clientGrain = clientGrain;
    }


    public Task AcceptMessageAsync(
        AnonymousMessage message,
        CancellationToken cancellationToken = default
    )
    {
        message = new AnonymousMessage(
            message.Excluding.Select(x => $"{hubName}::{x}").ToSet(),
            message.Payload
        );
        GrainCancellationTokenSource token = new();

        if (cancellationToken != default)
        {
            cancellationToken.Register(() => token.Cancel());
        }

        return clientGrain.AcceptMessageAsync(message.Payload, token.Token);
    }
}
