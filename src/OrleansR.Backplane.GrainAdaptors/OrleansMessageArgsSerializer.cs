namespace OrleansR.Backplane.GrainAdaptors;

using Core.Provider;
using Orleans.Serialization;


public class OrleansMessageArgsSerializer : IMessageArgsSerializer
{
    private readonly Serializer serializer;

    public OrleansMessageArgsSerializer(Serializer serializer) => this.serializer = serializer;


    public object?[] Deserialize(byte[] serialized) => serializer.Deserialize<object?[]>(serialized);

    public byte[] Serialize(object?[] args) => serializer.SerializeToArray(args);
}
