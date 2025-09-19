namespace OrleansR.Backplane.GrainInterfaces;

using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Provider;
using Orleans;


[Alias("OrleansR.Backplane.GrainInterfaces.IRewindableMessageGrain`1")]
public interface IRewindableMessageGrain<T> : IGrainWithStringKey
{
    /// <summary>
    /// Will get all messages since <paramref name="lastHandle"/>, exclusive
    /// </summary>
    /// <param name="lastHandle">The handle to get messages since, exclusive</param>
    /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the message buffer does not go back as far as the requested message</exception>
    Task<List<(T message, MessageHandle handle)>> GetMessagesSinceAsync(MessageHandle lastHandle);


    Task<MessageHandle> PushMessageAsync(T message);
}
