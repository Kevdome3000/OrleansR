namespace OrleansR.Backplane.GrainImplementations;

using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Provider;
using GrainInterfaces;
using Orleans;


public class ClientGrain : Grain, IClientGrain
{
    private readonly GrainObserverManager<IClientMessageObserver> observers = new()
    {
        ExpirationDuration = TimeSpan.FromMinutes(5),
        OnFailBeforeDefunct = x => x.SubscriptionEnded()
    };

    private IRewindableMessageGrain<MethodMessage> rewoundMessagesGrain = null!;


    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        rewoundMessagesGrain = GrainFactory.GetGrain<IRewindableMessageGrain<MethodMessage>>(this.GetPrimaryKeyString());
        return base.OnActivateAsync(cancellationToken);
    }


    public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        foreach (IClientMessageObserver observer in observers)
        {
            observer.SubscriptionEnded();
        }
        return base.OnDeactivateAsync(reason, cancellationToken);
    }


    public async Task AcceptMessageAsync(MethodMessage message, GrainCancellationToken cancellationToken)
    {
        MessageHandle handle = await rewoundMessagesGrain.PushMessageAsync(message);
        observers.Notify(x => x.ReceiveMessage(message, handle));
    }


    public async Task SubscribeToMessages(IClientMessageObserver observer, MessageHandle since)
    {
        observers.Subscribe(observer);

        if (since != default)
        {
            foreach ((MethodMessage message, MessageHandle handle) in await rewoundMessagesGrain.GetMessagesSinceAsync(since))
            {
                observer.ReceiveMessage(message, handle);
            }
        }
    }


    public Task UnsubscribeFromMessages(IClientMessageObserver observer)
    {
        observers.Unsubscribe(observer);
        return Task.CompletedTask;
    }

}
