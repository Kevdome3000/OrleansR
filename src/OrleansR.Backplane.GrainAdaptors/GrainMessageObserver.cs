namespace OrleansR.Backplane.GrainAdaptors;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.Provider;
using GrainInterfaces;
using Orleans;


public class GrainMessageObserver : IMessageObserver
{
    private readonly string hubName;
    private readonly IGrainFactory grainFactory;


    public GrainMessageObserver(
        string hubName,
        IGrainFactory grainFactory
    )
    {
        this.hubName = hubName ?? throw new ArgumentNullException(nameof(hubName));
        this.grainFactory = grainFactory ?? throw new ArgumentNullException(nameof(grainFactory));
    }


    public Task SendAddressedMessageAsync(AddressedMessage msg, CancellationToken cancellationToken = default)
    {
        GrainCancellationTokenSource token = new();

        if (cancellationToken != default)
        {
            cancellationToken.Register(() => token.Cancel());
        }
        return grainFactory.GetGrain<IClientGrain>($"{hubName}::{msg.ConnectionId}").AcceptMessageAsync(msg.Payload, token.Token);
    }


    public Task SendAllMessageAsync(AnonymousMessage allMessage, CancellationToken cancellationToken = default)
    {
        GrainCancellationTokenSource token = new();

        if (cancellationToken != default)
        {
            cancellationToken.Register(() => token.Cancel());
        }
        AnonymousMessage hubNamespacesAllMessage = new(
            allMessage.Excluding.Select(id => $"{hubName}::{id}").ToSet(),
            allMessage.Payload
        );
        return grainFactory.GetGrain<IAnonymousMessageGrain>(hubName).AcceptMessageAsync(allMessage, token.Token);
    }
}
