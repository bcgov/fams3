using AutoMapper;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.OptionSets.Models;
using Fams3Adapter.Dynamics.SearchApiRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.SearchRequest
{
   public class SearchRequestAutoMapperProfile : Profile
    {
        public SearchRequestAutoMapperProfile()
        {
            CreateMap<SSG_Identifier, Identifier>()
                 .ForMember(dest => dest.SerialNumber, opt => opt.MapFrom(src => src.Identification))
                 .ForMember(dest => dest.EffectiveDate, opt => opt.MapFrom(src => src.IdentificationEffectiveDate))
                 .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => src.IdentificationExpirationDate))
                 .ForMember(dest => dest.Type, opt => opt.MapFrom(src=>src.IdentifierType))
                 //.ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enumeration.FromValue<IdentificationType>((int)src.IdentifierType).SearchApiValue))
                 .ForMember(dest => dest.IssuedBy, opt => opt.MapFrom(src => src.IssuedBy));

            CreateMap<SSG_SearchApiRequest, PersonSearchRequest>()
                 .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.PersonGivenName))
                 .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.PersonSurname))
                 .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.PersonBirthDate))
                 .ForMember(dest => dest.Identifiers, opt => opt.MapFrom(src => src.Identifiers));
        }
    }
}
