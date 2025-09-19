namespace OrleansR.Core.Provider;

public class AddressedMessage(string connectionId, MethodMessage payload)
{
    public string ConnectionId { get; } = connectionId;
    public MethodMessage Payload { get; } = payload;


}
