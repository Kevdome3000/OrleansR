namespace OrleansR.Tests.Grains;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backplane.GrainInterfaces;
using Microsoft.Extensions.DependencyInjection;
using OrleansR.Core;
using OrleansR.Core.Provider;
using Orleans.Hosting;
using Orleans.TestingHost;
using OrleansR.OrleansSilo;
using Xunit;


public class TestSiloConfigurationsMax1 : ISiloConfigurator
{
    public void Configure(ISiloBuilder siloBuilder)
    {
        siloBuilder.AddOrleansRWithMemoryGrainStorage();
        siloBuilder.ConfigureServices(services => { services.AddTransient(config => new OrleansRSiloConfig { MaxMessageRewind = 1 }); });
    }
}


public class TestSiloConfigurationsMax10 : ISiloConfigurator
{
    public void Configure(ISiloBuilder siloBuilder)
    {
        siloBuilder.AddOrleansRWithMemoryGrainStorage();
        siloBuilder.ConfigureServices(services => { services.AddTransient(config => new OrleansRSiloConfig { MaxMessageRewind = 10 }); });
    }
}


public class RewindableMessageGrainTests
{
    public TestCluster? Cluster { get; set; }


    [Fact]
    public async Task GetMessageSinceReturnsAllMessagesIfInBounds()
    {
        TestClusterBuilder builder = new();
        builder.AddSiloBuilderConfigurator<TestSiloConfigurationsMax1>();
        Cluster = builder.Build();
        await Cluster.DeployAsync();
        IRewindableMessageGrain<AnonymousMessage>? grain = Cluster.GrainFactory.GetGrain<IRewindableMessageGrain<AnonymousMessage>>(
            Guid.NewGuid().ToString()
        );
        MessageHandle handle = await grain.PushMessageAsync(
            new AnonymousMessage(
                new HashSet<string>(),
                new MethodMessage("TestTarget1", Array.Empty<byte>())
            )
        );
        List<(AnonymousMessage message, MessageHandle handle)> since = await grain.GetMessagesSinceAsync(handle);
        Assert.Empty(since);
        AnonymousMessage secondMsg = new(
            new HashSet<string>(),
            new MethodMessage("TestTarget2", new byte[] { 1, 2, 3 })
        );
        MessageHandle handle2 = await grain.PushMessageAsync(secondMsg);
        since = await grain.GetMessagesSinceAsync(handle2);
        Assert.Empty(since);
        since = await grain.GetMessagesSinceAsync(handle);
        Assert.NotEmpty(since);
        Assert.Equal(handle2, since.Single().handle);
        Assert.Equal(
            secondMsg.Payload.SerializedArgs,
            since.Single().message.Payload.SerializedArgs
        );
    }


    [Fact]
    public async Task GetMessageSinceReturnsAllMessagesIfInBoundsLargerSet()
    {
        int maxRewind = 10;
        TestClusterBuilder builder = new();
        builder.AddSiloBuilderConfigurator<TestSiloConfigurationsMax10>();
        Cluster = builder.Build();
        await Cluster.DeployAsync();

        IRewindableMessageGrain<AnonymousMessage>? grain = Cluster.GrainFactory.GetGrain<IRewindableMessageGrain<AnonymousMessage>>(
            Guid.NewGuid().ToString()
        );
        List<MessageHandle> handles = Enumerable
            .Range(0, 20)
            .Select(
                i =>
                    grain.PushMessageAsync(
                        new AnonymousMessage(
                            new HashSet<string>(),
                            new MethodMessage(i.ToString(), Array.Empty<byte>())
                        )
                    )
            )
            .Select(x => x.Result)
            .ToList();

        for (int i = 0; i < handles.Count; i++)
        {
            if (i + 1 < maxRewind)
            {
                await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                    async () => await grain.GetMessagesSinceAsync(handles[i])
                );
            }
            else
            {
                List<(AnonymousMessage message, MessageHandle handle)> since = await grain.GetMessagesSinceAsync(handles[i]);
                List<MessageHandle> expectedSinceHandles = handles
                    .SkipWhile(x => x.MessageId <= handles[i].MessageId)
                    .ToList();
                Assert.Equal(expectedSinceHandles, since.Select(x => x.handle).ToList());

                if (i != handles.Count - 1)
                {
                    Assert.NotEmpty(since);
                }
            }
        }
    }


    [Fact]
    public async Task GetMessageSinceThrowsWhenOutOfBounds()
    {
        TestClusterBuilder builder = new();
        builder.AddSiloBuilderConfigurator<TestSiloConfigurationsMax1>();
        Cluster = builder.Build();
        await Cluster.DeployAsync();
        IRewindableMessageGrain<AnonymousMessage>? grain = Cluster.GrainFactory.GetGrain<IRewindableMessageGrain<AnonymousMessage>>(
            Guid.NewGuid().ToString()
        );
        MessageHandle handle = await grain.PushMessageAsync(
            new AnonymousMessage(
                new HashSet<string>(),
                new MethodMessage("TestTarget1", Array.Empty<byte>())
            )
        );
        await grain.PushMessageAsync(
            new AnonymousMessage(
                new HashSet<string>(),
                new MethodMessage("TestTarget2", Array.Empty<byte>())
            )
        );
        await grain.PushMessageAsync(
            new AnonymousMessage(
                new HashSet<string>(),
                new MethodMessage("TestTarget3", Array.Empty<byte>())
            )
        );
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => { await grain.GetMessagesSinceAsync(handle); });
    }


    [Fact]
    public async Task GetMessageSinceReturnsEmptyWhenGroupChanges()
    {
        TestClusterBuilder builder = new();
        builder.AddSiloBuilderConfigurator<TestSiloConfigurationsMax1>();
        Cluster = builder.Build();
        await Cluster.DeployAsync();
        IRewindableMessageGrain<AnonymousMessage>? grain = Cluster.GrainFactory.GetGrain<IRewindableMessageGrain<AnonymousMessage>>(
            Guid.NewGuid().ToString()
        );
        MessageHandle handle = await grain.PushMessageAsync(
            new AnonymousMessage(
                new HashSet<string>(),
                new MethodMessage("TestTarget1", Array.Empty<byte>())
            )
        );
        handle = new MessageHandle(handle.MessageId, Guid.NewGuid());
        Assert.Empty(await grain.GetMessagesSinceAsync(handle));
    }


    [Fact]
    public async Task GetMessageSinceReturnsEmptyWhenHandleNewer()
    {
        TestClusterBuilder builder = new();
        builder.AddSiloBuilderConfigurator<TestSiloConfigurationsMax1>();
        Cluster = builder.Build();
        await Cluster.DeployAsync();
        IRewindableMessageGrain<AnonymousMessage>? grain = Cluster.GrainFactory.GetGrain<IRewindableMessageGrain<AnonymousMessage>>(
            Guid.NewGuid().ToString()
        );
        MessageHandle handle = await grain.PushMessageAsync(
            new AnonymousMessage(
                new HashSet<string>(),
                new MethodMessage("TestTarget1", Array.Empty<byte>())
            )
        );
        handle = new MessageHandle(handle.MessageId + 1, handle.MessageGroup);
        Assert.Empty(await grain.GetMessagesSinceAsync(handle));
    }
}
