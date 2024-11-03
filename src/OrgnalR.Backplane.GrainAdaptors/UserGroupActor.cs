namespace OrgnalR.Backplane.GrainAdaptors;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.Provider;
using Core.State;
using GrainInterfaces;
using Orleans;


public class GrainUserActor : IUserActor
{
    private readonly IUserActorGrain userActorGrain;
    private readonly string hubName;


    public GrainUserActor(string hubName, IUserActorGrain userActorGrain)
    {
        this.hubName = hubName;
        this.userActorGrain = userActorGrain;
    }


    public Task AcceptMessageAsync(AnonymousMessage targetedMessage, CancellationToken cancellationToken = default)
    {
        targetedMessage = new AnonymousMessage(targetedMessage.Excluding.Select(x => $"{hubName}::{x}").ToSet(), targetedMessage.Payload);
        GrainCancellationTokenSource token = new();

        if (cancellationToken != default)
        {
            cancellationToken.Register(() => token.Cancel());
        }

        return userActorGrain.AcceptMessageAsync(targetedMessage, token.Token);
    }


    public Task AddToUserAsync(string connectionId, CancellationToken cancellationToken = default)
    {
        connectionId = $"{hubName}::{connectionId}";
        GrainCancellationTokenSource token = new();

        if (cancellationToken != default)
        {
            cancellationToken.Register(() => token.Cancel());
        }

        return userActorGrain.AddToUserAsync(connectionId, token.Token);
    }


    public Task RemoveFromUserAsync(string connectionId, CancellationToken cancellationToken = default)
    {
        connectionId = $"{hubName}::{connectionId}";
        GrainCancellationTokenSource token = new();

        if (cancellationToken != default)
        {
            cancellationToken.Register(() => token.Cancel());
        }

        return userActorGrain.RemoveFromUserAsync(connectionId, token.Token);
    }
}
