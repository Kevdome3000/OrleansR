namespace OrgnalR.Core.Provider;

using Orleans;


[GenerateSerializer]
public record MethodMessage(string MethodName, byte[] SerializedArgs);
