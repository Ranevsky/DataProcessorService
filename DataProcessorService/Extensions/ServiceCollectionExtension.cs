using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataProcessorService.Extensions;

public static class ServiceCollectionExtension
{
    public static void AddFromConfiguration<TService>(
        this IServiceCollection services,
        string configurationKey,
        ServiceLifetime lifetime)
        where TService : class
    {
        var descriptor = new ServiceDescriptor(
            typeof(TService),
            service => service
                           .GetRequiredService<IConfiguration>()
                           .GetRequiredSection(configurationKey)
                           .Get<TService>()
                       ?? throw new ArgumentException(
                           $"Configuration value with key = '{configurationKey}' is not valid"),
            lifetime);

        services.Add(descriptor);
    }
}