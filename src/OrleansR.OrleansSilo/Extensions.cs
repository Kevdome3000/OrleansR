namespace OrleansR.OrleansSilo;

using System;
using Core;
using Core.Provider;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Hosting;
using Backplane.GrainAdaptors;
using Backplane.GrainImplementations;


public static class Extensions
{
    public const string GROUP_STORAGE_PROVIDER = Constants.GROUP_STORAGE_PROVIDER;
    public const string USER_STORAGE_PROVIDER = Constants.USER_STORAGE_PROVIDER;

    /// <summary>
    /// This will store messages for each SignalR message stream, allowing clients to resubscribe without missing any messages
    /// This is a best effort resubscribe, and can be configured via <see cref="OrgnalRSiloConfig"/>
    /// </summary>
    public const string MESSAGE_STORAGE_PROVIDER = Constants.MESSAGE_STORAGE_PROVIDER;


    /// <summary>
    /// Adds the OrgnalR grains to the builder, and also automatically registers memory grain storage for group and user lists.
    /// This is useful for local development, however it is recommended that you add a persistent storage for:
    /// <see cref="GROUP_STORAGE_PROVIDER"/>, and <see cref="USER_STORAGE_PROVIDER"/>, and <see cref="MESSAGE_STORAGE_PROVIDER"/>
    /// Then you may use <see cref="AddOrleansR<T>(T builder)"/> to add orgnalr using the storage providers of your choice
    /// </summary>
    /// <param name="builder">The builder to configure</param>
    /// <returns>The silo builder, configured with memory storage and grains for the OrgnalR backplane</returns>
    public static ISiloBuilder AddOrleansRWithMemoryGrainStorage(
        this ISiloBuilder builder,
        Action<OrgnalRSiloConfig>? configure = null
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
    /// Adds the OrgnalR grains to the builder. This method is recommended for production use.
    /// You must configure storage providers for:
    /// <see cref="GROUP_STORAGE_PROVIDER"/>, and <see cref="USER_STORAGE_PROVIDER"/>, and <see cref="MESSAGE_STORAGE_PROVIDER"/>
    /// Alternatively, for local development, use: <see cref="AddOrleansRWithMemoryGrainStorage<T>(T builder)"/>
    /// </summary>
    /// <param name="builder">The builder to configure</param>
    /// <returns>The silo builder, configured with grains for the OrgnalR backplane</returns>
    public static ISiloBuilder AddOrleansR(
        this ISiloBuilder builder,
        Action<OrgnalRSiloConfig>? configure = null
    )
    {
        builder.ConfigureServices(
            services =>
            {
                OrgnalRSiloConfig conf = new();
                configure?.Invoke(conf);
                services.Add(new ServiceDescriptor(typeof(OrgnalRSiloConfig), conf));

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
