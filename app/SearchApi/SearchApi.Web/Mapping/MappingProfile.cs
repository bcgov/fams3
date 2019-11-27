using AutoMapper;
using SearchApi.Core.Adapters.Contracts;
using SearchApi.Core.Adapters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchApi.Web.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PersonSearchAccepted, ProviderSearchEventStatus>()
                .ForMember(dest => dest.SearchRequestId, opt => opt.MapFrom(src => src.SearchRequestId))
                .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(src => src.ProviderProfile.Name))
                .ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => src.TimeStamp))
                .ForMember(dest => dest.EventType, opt => opt.MapFrom(src => nameof(PersonSearchAccepted)))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => "Search requested accepted by provider"))
                .ReverseMap();
        }
    }
}
