using AutoMapper;
using DynamicsAdapter.Web.PersonSearch.Models;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.OptionSets.Models;
using Fams3Adapter.Dynamics.SearchApiEvent;
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
                 .ForMember(dest => dest.Type, opt => opt.ConvertUsing(new IdentifierTypeConverter(), src => src.IdentifierType))
                 .ForMember(dest => dest.IssuedBy, opt => opt.MapFrom(src => src.IssuedBy));

            CreateMap<SSG_SearchApiRequest, PersonSearchRequest>()
                 .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.PersonGivenName))
                 .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.PersonSurname))
                 .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.PersonBirthDate))
                 .ForMember(dest => dest.Identifiers, opt => opt.MapFrom(src => src.Identifiers));       

            CreateMap<PersonSearchAccepted, SSG_SearchApiEvent>()
              .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
              .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(src => src.ProviderProfile.Name))
              .ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => src.TimeStamp))
              .ForMember(dest => dest.EventType, opt => opt.MapFrom(src =>"Accepted"))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => Keys.SEARCH_API_EVENT_NAME))
              .ForMember(dest => dest.Message, opt => opt.MapFrom(src => "Search requested accepted by provider"))
              .ReverseMap();

            CreateMap<PersonSearchRejected, SSG_SearchApiEvent>()
              .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
              .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(src => src.ProviderProfile.Name))
              .ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => src.TimeStamp))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => Keys.SEARCH_API_EVENT_NAME))
              .ForMember(dest => dest.EventType, opt => opt.MapFrom(src => "Rejected"))
              .ForMember(dest => dest.Message, opt => opt.MapFrom(src => "Search requested rejected by provider. Reasons: " + string.Join(", ", src.Reasons.Select(x => $"{x.PropertyName} : {x.ErrorMessage}"))))
              .ReverseMap();

            CreateMap<PersonSearchFailed, SSG_SearchApiEvent>()
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
             .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(src => src.ProviderProfile.Name))
             .ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => src.TimeStamp))
              .ForMember(dest => dest.Name, opt => opt.MapFrom(src => Keys.SEARCH_API_EVENT_NAME))
             .ForMember(dest => dest.EventType, opt => opt.MapFrom(src => "Failed"))
             .ForMember(dest => dest.Message, opt => opt.MapFrom(src => "Search requested failed. Reason: " + src.Cause))
             .ReverseMap();

            CreateMap<PersonSearchCompleted, SSG_SearchApiEvent>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
               .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(src => src.ProviderProfile.Name))
               .ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => src.TimeStamp))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => Keys.SEARCH_API_EVENT_NAME))
               .ForMember(dest => dest.EventType, opt => opt.MapFrom(src => "Completed"))
               .ForMember(dest => dest.Message, opt => opt.MapFrom(src => "Search requested completed"))
               .ReverseMap();

            CreateMap<PersonalIdentifier, SSG_Identifier > ()
                 .ForMember(dest => dest.Identification, opt => opt.MapFrom(src => src.SerialNumber))
                 .ForMember(dest => dest.IdentificationEffectiveDate, opt => opt.MapFrom(src => src.EffectiveDate))
                 .ForMember(dest => dest.IdentificationExpirationDate, opt => opt.MapFrom(src => src.ExpirationDate))
                 .ForMember(dest => dest.IdentifierType, opt => opt.ConvertUsing(new PersonalIdentifierTypeConverter(), src => src.Type))
                 .ForMember(dest => dest.IssuedBy, opt => opt.MapFrom(src => src.IssuedBy));
        }
    }

    public class IdentifierTypeConverter : IValueConverter<int?, int?>
    {
        public int? Convert(int? source, ResolutionContext context)
        {
            if (source == null) return null;
            else
                return Enumeration.FromValue<IdentificationType>((int)source).SearchApiValue;
        }
    }

    public class PersonalIdentifierTypeConverter : IValueConverter<PersonalIdentifierType, int?>
    {
        public int? Convert(PersonalIdentifierType source, ResolutionContext context)
        {
            return source.ToDynamicIdentifierType();
        }
    }



    public static class SearchApiIdentifierType
    {
        public static int? ToDynamicIdentifierType(this PersonalIdentifierType value)
        {
            switch (value)
            {
                case PersonalIdentifierType.DriverLicense:
                    return IdentificationType.DriverLicense.DynamicValue;
                case PersonalIdentifierType.SocialInsuranceNumber:
                    return (int)IdentificationType.SocialInsuranceNumber.DynamicValue;
                case PersonalIdentifierType.PersonalHealthNumber:
                    return (int)IdentificationType.PersonalHealthNumber.DynamicValue;
                case PersonalIdentifierType.BirthCertificate:
                    return (int)IdentificationType.BirthCertificate.DynamicValue;
                case PersonalIdentifierType.CorrectionsId:
                    return (int)IdentificationType.CorrectionsId.DynamicValue;
                case PersonalIdentifierType.NativeStatusCard:
                    return (int)IdentificationType.NativeStatusCard.DynamicValue;
                case PersonalIdentifierType.Passport:
                    return (int)IdentificationType.Passport.DynamicValue;
                case PersonalIdentifierType.WcbClaim:
                    return (int)IdentificationType.WcbClaim.DynamicValue;
                case PersonalIdentifierType.Other:
                    return (int)IdentificationType.Other.DynamicValue;
                case PersonalIdentifierType.SecurityKeyword:
                    return (int)IdentificationType.SecurityKeyword.DynamicValue;
                default:
                    return null;
            }
        }
    }

}
