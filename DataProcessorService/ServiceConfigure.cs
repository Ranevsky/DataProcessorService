using System.Reflection;
using DataProcessorService.Contexts;
using DataProcessorService.Extensions;
using DataProcessorService.Models;
using DataProcessorService.ModuleHandler.Interfaces;
using DataProcessorService.Repositories;
using DataProcessorService.Repositories.Interfaces;
using DataProcessorService.Services;
using DataProcessorService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataProcessorService;

public static class ServiceConfigure
{
    public static void AddServices(this IServiceCollection services)
    {
        var asm = Assembly.GetExecutingAssembly();
        AddDbContext(services);
        AddModuleHandler(services);
        AddRepositories(services);
        AddMapper(services, asm);
        services.AddTransient<RabbitConsumer>();
        services.AddFromConfiguration<RabbitMqConnectionConfiguration>("Rabbit", ServiceLifetime.Scoped);
    }

    private static void AddDbContext(IServiceCollection services)
    {
        services.AddDbContext<ApplicationContext>((serviceProvider, option) =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetRequiredSection("ConnectionString:SQLite").Value;

            option.UseSqlite(connectionString);
        });

        services.AddTransient<IMigrator, EfCoreMigrator<ApplicationContext>>();
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IModuleStatusRepository, ModuleStatusRepository>();
    }

    private static void AddModuleHandler(IServiceCollection services)
    {
        services.AddScoped<IModuleHandler, ModuleHandler.ModuleHandler>();
    }

    private static void AddMapper(IServiceCollection services, Assembly asm)
    {
        services.AddAutoMapper(asm);
    }
}