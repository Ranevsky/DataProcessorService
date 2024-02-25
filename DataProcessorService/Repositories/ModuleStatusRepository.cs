using DataProcessorService.Contexts;
using DataProcessorService.Entities;
using DataProcessorService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataProcessorService.Repositories;

public class ModuleStatusRepository : IModuleStatusRepository
{
    private readonly ApplicationContext _db;
    private readonly ILogger<ModuleStatusRepository> _logger;

    public ModuleStatusRepository(
        ApplicationContext db,
        ILogger<ModuleStatusRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task SetStateByIdAsync(
        int id,
        string state,
        CancellationToken cancellationToken = default)
    {
        var device = new ModuleStatusEntity
        {
            Id = id,
            State = state,
        };

        _logger.LogInformation("Set state");

        var entityEntry = _db.ModulesInformation.Attach(device);
        entityEntry.Property(entity => entity.State).IsModified = true;

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task CreateAsync(
        ModuleStatusEntity moduleStatus,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Create new state");
        await _db.AddAsync(moduleStatus, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<ModuleStatusEntity?> GetByCategoryIdAsync(
        string categoryId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Get device status");

        return await _db.ModulesInformation
            .AsNoTracking()
            .Where(device => device.CategoryId == categoryId)
            .FirstOrDefaultAsync(cancellationToken);
    }
}