using AutoMapper;
using DataProcessorService.Entities;
using DataProcessorService.Models;
using DataProcessorService.ModuleHandler.Interfaces;
using DataProcessorService.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace DataProcessorService.ModuleHandler;

public class ModuleHandler : IModuleHandler
{
    private readonly ILogger<ModuleHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IModuleStatusRepository _moduleStatusRepository;

    public ModuleHandler(
        IModuleStatusRepository moduleStatusRepository,
        IMapper mapper,
        ILogger<ModuleHandler> logger)
    {
        _moduleStatusRepository = moduleStatusRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task HandleAsync(
        ModuleStatus module,
        CancellationToken cancellationToken = default)
    {
        var moduleEntity = await _moduleStatusRepository.GetByCategoryIdAsync(
            module.CategoryId,
            cancellationToken);

        if (moduleEntity is null)
        {
            _logger.LogInformation("Module status not found");
            moduleEntity = _mapper.Map<ModuleStatusEntity>(module);

            await _moduleStatusRepository.CreateAsync(moduleEntity, cancellationToken);

            return;
        }

        if (string.Equals(moduleEntity.State, module.State, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("State doesn't changed");

            return;
        }

        await _moduleStatusRepository.SetStateByIdAsync(
            moduleEntity.Id,
            module.State,
            cancellationToken);
    }
}