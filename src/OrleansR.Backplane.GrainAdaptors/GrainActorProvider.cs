namespace OrleansR.Backplane.GrainAdaptors;

using System;
using Core.State;
using GrainInterfaces;
using Orleans;


public class GrainActorProvider
{
    private readonly string hubName;
    private readonly IGrainFactory grainFactory;


    public GrainActorProvider(string hubName, IGrainFactory grainFactory)
    {
        this.hubName = hubName ?? throw new ArgumentNullException(nameof(hubName));
        this.grainFactory =
            grainFactory ?? throw new ArgumentNullException(nameof(grainFactory));
    }


    public IMessageAcceptor GetAllActor() => new GrainAllActor(
        hubName,
        grainFactory.GetGrain<IAnonymousMessageGrain>(hubName)
    );


    public IMessageAcceptor GetClientActor(string connectionId) => new GrainClientActor(
        hubName,
        grainFactory.GetGrain<IClientGrain>($"{hubName}::{connectionId}")
    );


    public IGroupActor GetGroupActor(string groupName) => new GrainGroupActor(
        hubName,
        grainFactory.GetGrain<IGroupActorGrain>($"{hubName}::{groupName}")
    );


    public IUserActor GetUserActor(string userId) => new GrainUserActor(
        hubName,
        grainFactory.GetGrain<IUserActorGrain>($"{hubName}::{userId}")
    );
}
