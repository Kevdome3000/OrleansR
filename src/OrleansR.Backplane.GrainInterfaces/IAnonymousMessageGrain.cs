namespace OrleansR.Backplane.GrainInterfaces;

using System.Threading.Tasks;
using Core.Provider;
using Orleans;


[Alias("OrleansR.Backplane.GrainInterfaces.IAnonymousMessageGrain")]
public interface IAnonymousMessageGrain : IGrainWithStringKey
{
    Task AcceptMessageAsync(AnonymousMessage message, GrainCancellationToken cancellationToken);


    /// <summary>
    /// Subscribes to messages for this client, optionally replaying all messages since a last seen message
    /// </summary>
    /// <param name="since">The handle to get messages since, exclusive</param>
    /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the message buffer does not go back as far as the requested message</exception>
    Task SubscribeToMessages(IAnonymousMessageObserver observer, MessageHandle since = default);


    Task UnsubscribeFromMessages(IAnonymousMessageObserver observer);

}


[Alias("OrleansR.Backplane.GrainInterfaces.IAnonymousMessageObserver")]
public interface IAnonymousMessageObserver : IGrainObserver
{
    void ReceiveMessage(AnonymousMessage message, MessageHandle handle);
    void SubscriptionEnded();
}
