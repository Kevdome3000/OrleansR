namespace OrleansR.Backplane.GrainImplementations;

using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Provider;
using GrainInterfaces;
using Orleans;


public class AnonymousMessageGrain : Grain, IAnonymousMessageGrain
{

    private readonly GrainObserverManager<IAnonymousMessageObserver> observers = new()
    {
        ExpirationDuration = TimeSpan.FromMinutes(5),
        OnFailBeforeDefunct = x => x.SubscriptionEnded()
    };

    private IRewindableMessageGrain<AnonymousMessage> rewoundMessagesGrain = null!;


    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        rewoundMessagesGrain = GrainFactory.GetGrain<IRewindableMessageGrain<AnonymousMessage>>(this.GetPrimaryKeyString());
        return base.OnActivateAsync(cancellationToken);
    }


    public override Task OnDeactivateAsync(DeactivationReason deactivationReason, CancellationToken cancellationToken)
    {
        foreach (IAnonymousMessageObserver observer in observers)
        {
            observer.SubscriptionEnded();
        }
        return base.OnDeactivateAsync(deactivationReason, cancellationToken);
    }


    public async Task AcceptMessageAsync(AnonymousMessage message, GrainCancellationToken cancellationToken)
    {
        MessageHandle handle = await rewoundMessagesGrain.PushMessageAsync(message);
        observers.Notify(x => x.ReceiveMessage(message, handle));
    }


    public async Task SubscribeToMessages(IAnonymousMessageObserver observer, MessageHandle since)
    {
        observers.Subscribe(observer);

        if (since != default)
        {
            foreach ((AnonymousMessage message, MessageHandle handle) in await rewoundMessagesGrain.GetMessagesSinceAsync(since))
            {
                observer.ReceiveMessage(message, handle);
            }
        }
    }


    public Task UnsubscribeFromMessages(IAnonymousMessageObserver observer)
    {
        observers.Unsubscribe(observer);
        return Task.CompletedTask;
    }
}
