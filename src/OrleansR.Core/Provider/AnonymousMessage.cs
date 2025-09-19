namespace OrleansR.Core.Provider;

using System.Collections.Generic;
using Orleans;


[GenerateSerializer]
public class AnonymousMessage(ISet<string> excluding, MethodMessage payload)
{
    [Id(0)]
    public ISet<string> Excluding { get; } = excluding;

    [Id(1)]
    public MethodMessage Payload { get; } = payload;


}
