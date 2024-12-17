namespace OrleansR.OrleansSilo;

using System;
using Backplane.GrainAdaptors;
using Backplane.GrainImplementations;
using Core;
using Core.Provider;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Hosting;


public static class Extensions
{
    public const string GROUP_STORAGE_PROVIDER = Constants.GROUP_STORAGE_PROVIDER;
    public const string USER_STORAGE_PROVIDER = Constants.USER_STORAGE_PROVIDER;

    /// <summary>
    /// This will store messages for each SignalR message stream, allowing clients to resubscribe without missing any messages
    /// This is a best effort resubscribe, and can be configured via <see cref="OrleansRSiloConfig"/>
    /// </summary>
    public const string MESSAGE_STORAGE_PROVIDER = Constants.MESSAGE_STORAGE_PROVIDER;


    /// <summary>
    /// Adds the OrleansR grains to the builder, and also automatically registers memory grain storage for group and user lists.
    /// This is useful for local development, however it is recommended that you add a persistent storage for:
    /// <see cref="GROUP_STORAGE_PROVIDER"/>, and <see cref="USER_STORAGE_PROVIDER"/>, and <see cref="MESSAGE_STORAGE_PROVIDER"/>
    /// Then you may use <see cref="AddOrleansR<T>(T builder)"/> to add OrleansR using the storage providers of your choice
    /// </summary>
    /// <param name="builder">The builder to configure</param>
    /// <returns>The silo builder, configured with memory storage and grains for the OrleansR backplane</returns>
    public static ISiloBuilder AddOrleansRWithMemoryGrainStorage(
        this ISiloBuilder builder,
        Action<OrleansRSiloConfig>? configure = null
    )
    {
        try
        {
            builder.AddMemoryGrainStorage(Constants.GROUP_STORAGE_PROVIDER);
        }
        catch
        {
            /* Do nothing, already added  */
        }

        try
        {
            builder.AddMemoryGrainStorage(Constants.USER_STORAGE_PROVIDER);
        }
        catch
        {
            /* Do nothing, already added  */
        }

        try
        {
            builder.AddMemoryGrainStorage(Constants.MESSAGE_STORAGE_PROVIDER);
        }
        catch
        {
            /* Do nothing, already added  */
        }

        return builder.AddOrleansR(configure);
    }


    /// <summary>
    /// Adds the OrleansR grains to the builder. This method is recommended for production use.
    /// You must configure storage providers for:
    /// <see cref="GROUP_STORAGE_PROVIDER"/>, and <see cref="USER_STORAGE_PROVIDER"/>, and <see cref="MESSAGE_STORAGE_PROVIDER"/>
    /// Alternatively, for local development, use: <see cref="AddOrleansRWithMemoryGrainStorage<T>(T builder)"/>
    /// </summary>
    /// <param name="builder">The builder to configure</param>
    /// <returns>The silo builder, configured with grains for the OrleansR backplane</returns>
    public static ISiloBuilder AddOrleansR(
        this ISiloBuilder builder,
        Action<OrleansRSiloConfig>? configure = null
    )
    {
        builder.ConfigureServices(
            services =>
            {
                OrleansRSiloConfig conf = new();
                configure?.Invoke(conf);
                services.Add(new ServiceDescriptor(typeof(OrleansRSiloConfig), conf));

                services.AddSingleton<IGrainFactoryProvider, GrainFactoryProvider>();
                builder.Services.AddSingleton<
                    IMessageArgsSerializer,
                    OrleansMessageArgsSerializer
                >();
                services.AddSingleton<IActorProviderFactory, GrainActorProviderFactory>();
                services.AddSingleton<IHubContextProvider, HubContextProvider>();
            }
        );
        return builder;
    }
}
