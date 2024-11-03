namespace OrleansR.Core.Provider;

public interface IMessageArgsSerializer
{
    byte[] Serialize(object?[] args);

    object?[] Deserialize(byte[] serialized);
}
