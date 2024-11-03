namespace OrleansR.Backplane.GrainAdaptors;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.Provider;
using Core.State;
using GrainInterfaces;
using Orleans;


public class GrainAllActor : IMessageAcceptor
{
    private readonly string hubName;
    private readonly IAnonymousMessageGrain anonymousMessageGrain;


    public GrainAllActor(string hubName, IAnonymousMessageGrain anonymousMessageGrain)
    {
        this.hubName = hubName;
        this.anonymousMessageGrain = anonymousMessageGrain;
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

        return anonymousMessageGrain.AcceptMessageAsync(message, token.Token);
    }
}
