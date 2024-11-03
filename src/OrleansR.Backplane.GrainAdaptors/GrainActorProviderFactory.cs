namespace OrleansR.Backplane.GrainAdaptors;

using Core.Provider;
using Core.State;


public class GrainActorProviderFactory : IActorProviderFactory
{
    private readonly IGrainFactoryProvider grainFactoryProvider;

    public GrainActorProviderFactory(IGrainFactoryProvider grainFactoryProvider) => this.grainFactoryProvider = grainFactoryProvider;


    public IMessageAcceptor GetAllActor(string hubName) => new GrainActorProvider(
        hubName,
        grainFactoryProvider.GetGrainFactory()
    ).GetAllActor();


    public IMessageAcceptor GetClientActor(string hubName, string connectionId) => new GrainActorProvider(
        hubName,
        grainFactoryProvider.GetGrainFactory()
    ).GetClientActor(connectionId);


    public IGroupActor GetGroupActor(string hubName, string groupName) => new GrainActorProvider(
        hubName,
        grainFactoryProvider.GetGrainFactory()
    ).GetGroupActor(groupName);


    public IUserActor GetUserActor(string hubName, string userId) => new GrainActorProvider(hubName, grainFactoryProvider.GetGrainFactory()).GetUserActor(
        userId
    );
}
