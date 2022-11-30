using Orleans;

namespace OrgnalR.Core.Provider
{
    [GenerateSerializer]
    public record MethodMessage(string MethodName, object?[] Args);
}
