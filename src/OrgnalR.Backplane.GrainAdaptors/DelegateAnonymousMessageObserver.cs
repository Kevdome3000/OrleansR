namespace OrgnalR.Backplane.GrainAdaptors;

using System;
using System.Threading.Tasks;
using Core.Provider;
using GrainInterfaces;
using Orleans;


public class DelegateAnonymousMessageObserver : IAnonymousMessageObserver
{
    private readonly SubscriptionHandle subscriptionHandle;
    private readonly Func<AnonymousMessage, MessageHandle, Task> messageCallback;
    private readonly Func<SubscriptionHandle, Task> onSubscriptionEnded;


    public DelegateAnonymousMessageObserver(SubscriptionHandle subscriptionHandle, Func<AnonymousMessage, MessageHandle, Task> messageCallback, Func<SubscriptionHandle, Task> onSubscriptionEnded)
    {
        this.subscriptionHandle = subscriptionHandle ?? throw new ArgumentNullException(nameof(subscriptionHandle));
        this.messageCallback = messageCallback ?? throw new ArgumentNullException(nameof(messageCallback));
        this.onSubscriptionEnded = onSubscriptionEnded ?? throw new ArgumentNullException(nameof(onSubscriptionEnded));
    }


    public void ReceiveMessage(AnonymousMessage message, MessageHandle handle)
    {
        messageCallback(message, handle).Ignore();
    }


    public void SubscriptionEnded()
    {
        onSubscriptionEnded(subscriptionHandle).Ignore();
    }
}
