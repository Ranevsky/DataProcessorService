namespace DataProcessorService.Services.Interfaces;

public interface IMigrator
{
    Task MigrateAsync(CancellationToken cancellationToken = default);
}