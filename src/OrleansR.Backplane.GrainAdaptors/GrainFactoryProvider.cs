namespace OrleansR.Backplane.GrainAdaptors;

using System;
using Microsoft.Extensions.DependencyInjection;
using Orleans;


/// <summary>
/// Provides the grain factory to the OrleansR backplane.
/// See <see cref="GrainFactoryProvider"/> for a simple implementation.
/// </summary>
public interface IGrainFactoryProvider
{
    IGrainFactory GetGrainFactory();
}


public class GrainFactoryProvider : IGrainFactoryProvider
{
    private readonly IServiceProvider serviceProvider;


    public GrainFactoryProvider(IServiceProvider serviceProvider) => this.serviceProvider =
        serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));


    public IGrainFactory GetGrainFactory() =>
        serviceProvider.GetRequiredService<IClusterClient>();
}
