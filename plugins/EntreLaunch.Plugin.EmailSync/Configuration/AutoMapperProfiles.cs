using AutoMapper;
using EntreLaunch.Entities;
using EntreLaunch.Plugin.EmailSync.DTOs;

namespace EntreLaunch.Plugin.EmailSync.Configuration;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<ImapAccount, ImapAccountCreateDto>().ReverseMap();
        CreateMap<ImapAccount, ImapAccountUpdateDto>()
            .ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<ImapAccountUpdateDto, ImapAccount>()
            .ForAllMembers(m => m.Condition(PropertyNeedsMapping));
        CreateMap<ImapAccount, ImapAccountDetailsDto>();
    }

    private static bool PropertyNeedsMapping(object source, object target, object sourceValue, object targetValue)
    {
        return sourceValue != null;
    }
}
