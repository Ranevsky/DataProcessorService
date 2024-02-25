using DataProcessorService.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataProcessorService.Contexts;

public class ApplicationContext(DbContextOptions<ApplicationContext> options) : DbContext(options)
{
    public DbSet<ModuleStatusEntity> ModulesInformation => Set<ModuleStatusEntity>();
}