using AutoMapper;
using DataProcessorService.Entities;
using DataProcessorService.Models;

namespace DataProcessorService.Profiles;

public class ModuleStatusProfile : Profile
{
    public ModuleStatusProfile()
    {
        CreateMap<ModuleStatus, ModuleStatusEntity>();
    }
}