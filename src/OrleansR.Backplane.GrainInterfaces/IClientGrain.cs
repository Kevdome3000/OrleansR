namespace OrleansR.Backplane.GrainInterfaces;

using System.Threading.Tasks;
using Core.Provider;
using Orleans;

[Alias("OrleansR.Backplane.GrainInterfaces.IClientGrain")]
public interface IClientGrain : IGrainWithStringKey
{
    Task AcceptMessageAsync(MethodMessage message, GrainCancellationToken cancellationToken);


    /// <summary>
    /// Subscribes to messages for this client, optionally replaying all messages since a last seen message
    /// </summary>
    /// <param name="since">The handle to get messages since, exclusive</param>
    /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the message buffer does not go back as far as the requested message</exception>
    Task SubscribeToMessages(IClientMessageObserver observer, MessageHandle since = default);


    Task UnsubscribeFromMessages(IClientMessageObserver observer);
}

[Alias("OrleansR.Backplane.GrainInterfaces.IClientMessageObserver")]
public interface IClientMessageObserver : IGrainObserver
{
    void ReceiveMessage(MethodMessage message, MessageHandle handle);
    void SubscriptionEnded();
}
