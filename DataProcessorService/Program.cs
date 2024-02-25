using DataProcessorService;
using DataProcessorService.Services;
using DataProcessorService.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => { services.AddServices(); })
    .ConfigureHostConfiguration(config =>
    {
        config.AddJsonFile("appsettings.json", false);
        config.AddEnvironmentVariables();
    })
    .Build();

var services = host.Services;

await services.GetRequiredService<IMigrator>().MigrateAsync();

await using var scope = services.CreateAsyncScope();
var serviceScope = scope.ServiceProvider;
var rabbitConsumer = serviceScope.GetRequiredService<RabbitConsumer>();
rabbitConsumer.Consume();

Console.WriteLine("Press any key to exit...");
Console.ReadKey();