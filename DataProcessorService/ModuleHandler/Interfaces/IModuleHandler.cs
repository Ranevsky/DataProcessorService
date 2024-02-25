using DataProcessorService.Models;

namespace DataProcessorService.ModuleHandler.Interfaces;

public interface IModuleHandler
{
    Task HandleAsync(
        ModuleStatus module,
        CancellationToken cancellationToken = default);
}