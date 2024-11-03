namespace OrgnalR.Backplane.GrainAdaptors;

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.Provider;
using GrainInterfaces;
using Orleans;


public class GrainMessageObservable : IMessageObservable
{
    private readonly string hubName;
    private readonly IGrainFactory grainFactory;
    private readonly GrainProviderReadier grainProviderReadier;

    private readonly ConcurrentDictionary<
        Guid,
        (IAnonymousMessageObserver raw, IAnonymousMessageObserver obj)
    > anonymousObservers = new();

    private readonly ConcurrentDictionary<
        string,
        (IClientMessageObserver raw, IClientMessageObserver obj)
    > clientObservers = new();


    public GrainMessageObservable(
        string hubName,
        IGrainFactory grainFactory,
        GrainProviderReadier grainProviderReadier
    )
    {
        this.hubName = hubName ?? throw new ArgumentNullException(nameof(hubName));
        this.grainFactory =
            grainFactory ?? throw new ArgumentNullException(nameof(grainFactory));
        this.grainProviderReadier =
            grainProviderReadier
            ?? throw new ArgumentNullException(nameof(grainProviderReadier));
    }


    public async Task<SubscriptionHandle> SubscribeToAllAsync(
        Func<AnonymousMessage, MessageHandle, Task> messageCallback,
        Func<SubscriptionHandle, Task> onSubscriptionEnd,
        MessageHandle since = default,
        CancellationToken cancellationToken = default
    )
    {
        await grainProviderReadier.ClusterClientReady.WithCancellation(cancellationToken);
        SubscriptionHandle handle = new(Guid.NewGuid());
        DelegateAnonymousMessageObserver handler = new(
            handle,
            messageCallback,
            onSubscriptionEnd
        );

        IAnonymousMessageGrain? messageGrain = grainFactory.GetGrain<IAnonymousMessageGrain>(hubName);
        IAnonymousMessageObserver? handlerRef = grainFactory.CreateObjectReference<IAnonymousMessageObserver>(handler);
        anonymousObservers[handle.SubscriptionId] = (handler, handlerRef);
        await messageGrain.SubscribeToMessages(handlerRef, since).ConfigureAwait(false);
        return handle;
    }


    public async Task UnsubscribeFromAllAsync(
        SubscriptionHandle subscriptionHandle,
        CancellationToken cancellationToken = default
    )
    {
        await grainProviderReadier.ClusterClientReady.WithCancellation(cancellationToken);

        if (!anonymousObservers.TryRemove(subscriptionHandle.SubscriptionId, out (IAnonymousMessageObserver raw, IAnonymousMessageObserver obj) handler))
        {
            return;
        }
        IAnonymousMessageGrain? messageGrain = grainFactory.GetGrain<IAnonymousMessageGrain>(hubName);
        await messageGrain.UnsubscribeFromMessages(handler.obj).ConfigureAwait(false);
    }


    public async Task SubscribeToConnectionAsync(
        string connectionId,
        Func<AddressedMessage, MessageHandle, Task> messageCallback,
        Func<string, Task> onSubscriptionEnd,
        MessageHandle since = default,
        CancellationToken cancellationToken = default
    )
    {
        await grainProviderReadier.ClusterClientReady.WithCancellation(cancellationToken);
        DelegateClientMessageObserver handler = new(
            connectionId,
            messageCallback,
            onSubscriptionEnd
        );
        IClientMessageObserver? handlerRef = await Task.FromResult(
                grainFactory.CreateObjectReference<IClientMessageObserver>(handler)
            )
            .ConfigureAwait(false);
        clientObservers[connectionId] = (handler, handlerRef);
        IClientGrain? messageGrain = grainFactory.GetGrain<IClientGrain>($"{hubName}::{connectionId}");
        await messageGrain.SubscribeToMessages(handlerRef, since).ConfigureAwait(false);
    }


    public async Task UnsubscribeFromConnectionAsync(
        string connectionId,
        CancellationToken cancellationToken = default
    )
    {
        await grainProviderReadier.ClusterClientReady.WithCancellation(cancellationToken);

        if (!clientObservers.TryRemove(connectionId, out (IClientMessageObserver raw, IClientMessageObserver obj) handler))
        {
            return;
        }
        IClientGrain? messageGrain = grainFactory.GetGrain<IClientGrain>($"{hubName}::{connectionId}");
        await messageGrain.UnsubscribeFromMessages(handler.obj).ConfigureAwait(false);
    }
}
