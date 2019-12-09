using AutoMapper;
using DynamicsAdapter.Web.PersonSearch.Models;
using Fams3Adapter.Dynamics.Address;
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
                 .ForMember(dest => dest.IssuedBy, opt => opt.ConvertUsing(new InformationSourceConverter(), src => src.InformationSource));

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
              .ForMember(dest => dest.Message, opt => opt.MapFrom(src => "Auto search has been accepted for processing"))
              .ReverseMap();

            CreateMap<PersonSearchRejected, SSG_SearchApiEvent>()
              .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
              .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(src => src.ProviderProfile.Name))
              .ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => src.TimeStamp))
              .ForMember(dest => dest.Name, opt => opt.MapFrom(src => Keys.SEARCH_API_EVENT_NAME))
              .ForMember(dest => dest.EventType, opt => opt.MapFrom(src => "Rejected"))
              .ForMember(dest => dest.Message, opt => opt.MapFrom(src => "Auto search has been rejected. Reasons: " + string.Join(", ", src.Reasons.Select(x => $"{x.PropertyName} : {x.ErrorMessage}"))))
              .ReverseMap();

            CreateMap<PersonSearchFailed, SSG_SearchApiEvent>()
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
             .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(src => src.ProviderProfile.Name))
             .ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => src.TimeStamp))
              .ForMember(dest => dest.Name, opt => opt.MapFrom(src => Keys.SEARCH_API_EVENT_NAME))
             .ForMember(dest => dest.EventType, opt => opt.MapFrom(src => "Failed"))
             .ForMember(dest => dest.Message, opt => opt.MapFrom(src => "Auto search processing failed. Reason: " + src.Cause))
             .ReverseMap();

            CreateMap<PersonSearchCompleted, SSG_SearchApiEvent>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
               .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(src => src.ProviderProfile.Name))
               .ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => src.TimeStamp))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => Keys.SEARCH_API_EVENT_NAME))
               .ForMember(dest => dest.EventType, opt => opt.MapFrom(src => "Completed"))
               .ForMember(dest => dest.Message, opt => opt.MapFrom(src => $"Auto search processing completed successfully. {src.MatchedPerson.Identifiers.Count()} results found."))
               .ReverseMap();

            CreateMap<Address, SSG_Address>()
                 .ForMember(dest => dest.AddressLine1, opt => opt.MapFrom(src => src.AddressLine1))
                 .ForMember(dest => dest.AddressLine2, opt => opt.MapFrom(src => src.AddressLine2))
                 .ForMember(dest => dest.Province, opt => opt.ConvertUsing(new ProvinceConverter(), src => src.Province))
                 .ForMember(dest => dest.InformationSource, opt => opt.ConvertUsing(new IssuedByTypeConverter(), src => src.SuppliedBy))
                 .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                 .ForMember(dest => dest.Country, opt => opt.ConvertUsing(new CountryConverter(), src => src.Country))
                 .ForMember(dest => dest.Category, opt => opt.ConvertUsing(new AddressTypeConverter(), src => src.Type))
                 .ForMember(dest => dest.NonCanadianState, opt => opt.MapFrom(src => src.NonCanadianState))
                 .ForMember(dest => dest.FullText, opt => opt.MapFrom<FullTextResolver>())
                 .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.PostalCode))
                 .ForMember(dest => dest.StateCode, opt => opt.MapFrom(src => 0))
                 .ForMember(dest => dest.StatusCode, opt => opt.MapFrom(src => 1));

            CreateMap<PersonalIdentifier, SSG_Identifier>()
                 .ForMember(dest => dest.Identification, opt => opt.MapFrom(src => src.SerialNumber))
                 .ForMember(dest => dest.IdentificationEffectiveDate, opt => opt.MapFrom(src => src.EffectiveDate))
                 .ForMember(dest => dest.IdentificationExpirationDate, opt => opt.MapFrom(src => src.ExpirationDate))
                 .ForMember(dest => dest.IdentifierType, opt => opt.ConvertUsing(new PersonalIdentifierTypeConverter(), src => src.Type))
                 .ForMember(dest => dest.InformationSource, opt => opt.ConvertUsing(new IssuedByTypeConverter(), src=>src.IssuedBy))
                 .ForMember(dest => dest.StateCode, opt => opt.MapFrom(src => 0))
                 .ForMember(dest => dest.StatusCode, opt => opt.MapFrom(src => 1));
        }
    }

    public class InformationSourceConverter : IValueConverter<int?, string>
    {
        public string Convert(int? source, ResolutionContext context)
        {
            return Enumeration.GetAll<InformationSourceType>().FirstOrDefault(m => m.Value == source)?.Name;
        }
    }

    public class IssuedByTypeConverter : IValueConverter<string, int?>
    {
        public int? Convert(string source, ResolutionContext context)
        {
            InformationSourceType sourceType = Enumeration.GetAll<InformationSourceType>().FirstOrDefault(m => m.Name.Equals(source, StringComparison.OrdinalIgnoreCase));
            return (sourceType==null)? InformationSourceType.Other.Value: sourceType.Value;
        }
    }

    public class IdentifierTypeConverter : IValueConverter<int?, int?>
    {
        public int? Convert(int? source, ResolutionContext context)
        {
            if (source == IdentificationType.DriverLicense.Value) return (int)PersonalIdentifierType.DriverLicense;
            else if (source == IdentificationType.SocialInsuranceNumber.Value) return (int)PersonalIdentifierType.SocialInsuranceNumber;
            else if (source == IdentificationType.PersonalHealthNumber.Value) return (int)PersonalIdentifierType.PersonalHealthNumber;
            else if (source == IdentificationType.BirthCertificate.Value) return (int)PersonalIdentifierType.BirthCertificate;
            else if (source == IdentificationType.CorrectionsId.Value) return (int)PersonalIdentifierType.CorrectionsId;
            else if (source == IdentificationType.NativeStatusCard.Value) return (int)PersonalIdentifierType.NativeStatusCard;
            else if (source == IdentificationType.Passport.Value) return (int)PersonalIdentifierType.Passport;
            else if (source == IdentificationType.WcbClaim.Value) return (int)PersonalIdentifierType.WcbClaim;
            else if (source == IdentificationType.Other.Value) return (int)PersonalIdentifierType.Other;
            else if (source == IdentificationType.SecurityKeyword.Value) return (int)PersonalIdentifierType.SecurityKeyword;
            else return null;
        }
    }

    public class PersonalIdentifierTypeConverter : IValueConverter<PersonalIdentifierType, int?>
    {
        public int? Convert(PersonalIdentifierType source, ResolutionContext context)
        {
            return source.ToDynamicIdentifierType();
        }
    }

    public class ProvinceConverter : IValueConverter<string, int?>
    {
        public int? Convert(string source, ResolutionContext context)
        {
            return Enumeration.GetAll<CanadianProvinceType>().FirstOrDefault(m => m.Name.Equals(source, StringComparison.OrdinalIgnoreCase))?.Value;
        }
    }

    public class AddressTypeConverter : IValueConverter<string, int?>
    {
        public int? Convert(string source, ResolutionContext context)
        {
            return Enumeration.GetAll<LocationType>().FirstOrDefault(m => m.Name.Equals(source, StringComparison.OrdinalIgnoreCase))?.Value;
        }
    }

    public class CountryConverter : IValueConverter<string, SSG_Country?>
    {
        public SSG_Country Convert(string source, ResolutionContext context)
        {
            return new SSG_Country()
            {
                Name = source
            };
        }
    }

    public class FullTextResolver : IValueResolver<Address, SSG_Address, string>
    {
        public string Resolve(Address source, SSG_Address dest, string fullText, ResolutionContext context)
        {
            return $"{source.AddressLine1} {source.AddressLine2} {source.City} {source.Province} {source.Country} {source.PostalCode}";
        }
    }

    public static class SearchApiIdentifierType
    {
        public static int? ToDynamicIdentifierType(this PersonalIdentifierType value)
        {
            switch (value)
            {
                case PersonalIdentifierType.DriverLicense:
                    return IdentificationType.DriverLicense.Value;
                case PersonalIdentifierType.SocialInsuranceNumber:
                    return IdentificationType.SocialInsuranceNumber.Value;
                case PersonalIdentifierType.PersonalHealthNumber:
                    return IdentificationType.PersonalHealthNumber.Value;
                case PersonalIdentifierType.BirthCertificate:
                    return IdentificationType.BirthCertificate.Value;
                case PersonalIdentifierType.CorrectionsId:
                    return IdentificationType.CorrectionsId.Value;
                case PersonalIdentifierType.NativeStatusCard:
                    return IdentificationType.NativeStatusCard.Value;
                case PersonalIdentifierType.Passport:
                    return IdentificationType.Passport.Value;
                case PersonalIdentifierType.WcbClaim:
                    return IdentificationType.WcbClaim.Value;
                case PersonalIdentifierType.Other:
                    return IdentificationType.Other.Value;
                case PersonalIdentifierType.SecurityKeyword:
                    return IdentificationType.SecurityKeyword.Value;
                default:
                    return null;
            }
        }
    }

}
