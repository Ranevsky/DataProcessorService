using DataProcessorService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataProcessorService.Services;

public class EfCoreMigrator<TContext> : IMigrator
    where TContext : DbContext
{
    private readonly TContext _db;

    public EfCoreMigrator(TContext db)
    {
        _db = db;
    }

    public async Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        await _db.Database.MigrateAsync(cancellationToken);
    }
}