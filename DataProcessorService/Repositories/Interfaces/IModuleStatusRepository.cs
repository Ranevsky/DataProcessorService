using DataProcessorService.Entities;

namespace DataProcessorService.Repositories.Interfaces;

public interface IModuleStatusRepository
{
    Task<ModuleStatusEntity?> GetByCategoryIdAsync(
        string categoryId,
        CancellationToken cancellationToken = default);

    Task CreateAsync(
        ModuleStatusEntity moduleStatus,
        CancellationToken cancellationToken = default);

    Task SetStateByIdAsync(
        int id,
        string state,
        CancellationToken cancellationToken = default);
}