using AutoMapper;
using DynamicsAdapter.Web.PersonSearch.Models;
using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.Name;
using Fams3Adapter.Dynamics.OptionSets.Models;
using Fams3Adapter.Dynamics.PhoneNumber;
using Fams3Adapter.Dynamics.SearchApiEvent;
using Fams3Adapter.Dynamics.SearchApiRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace  DynamicsAdapter.Web.Mapping
{

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SSG_Identifier, PersonalIdentifier>()
                 .ConstructUsing(m => new PersonalIdentifierActual() { })
                 .ForMember(dest => dest.SerialNumber, opt => opt.MapFrom(src => src.Identification))
                 .ForMember(dest => dest.EffectiveDate, opt => opt.MapFrom(src => src.IdentificationEffectiveDate))
                 .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => src.IdentificationExpirationDate))
                 .ForMember(dest => dest.Type, opt => opt.ConvertUsing(new IdentifierTypeConverter(), src => src.IdentifierType));
                

            CreateMap<PersonalIdentifier, SSG_Identifier>()
               .ForMember(dest => dest.Identification, opt => opt.MapFrom(src => src.SerialNumber))
               .ForMember(dest => dest.IdentificationEffectiveDate, opt => opt.ConvertUsing(new DateTimeOffsetConverter(), src => src.EffectiveDate))
               .ForMember(dest => dest.IdentificationExpirationDate, opt => opt.ConvertUsing(new DateTimeOffsetConverter(), src => src.ExpirationDate))
               .ForMember(dest => dest.IdentifierType, opt => opt.ConvertUsing(new PersonalIdentifierTypeConverter(), src => src.Type))
               .ForMember(dest => dest.IssuedBy, opt=>opt.ConvertUsing( new IssuedByConverter(), src => src.IssuedBy))
               .ForMember(dest => dest.StateCode, opt => opt.MapFrom(src => 0))
               .ForMember(dest => dest.StatusCode, opt => opt.MapFrom(src => 1));


            CreateMap<SSG_SearchApiRequest, PersonSearchRequest>()
                 .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.PersonGivenName))
                 .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.PersonSurname))
                 .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.PersonBirthDate))
                 .ForMember(dest => dest.Identifiers, opt => opt.MapFrom(src => src.Identifiers));       

            CreateMap<PersonSearchAccepted, SSG_SearchApiEvent>()
              .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
              .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(src => src.ProviderProfile.Name))
              .ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => src.TimeStamp))
              .ForMember(dest => dest.EventType, opt => opt.MapFrom(src => Keys.EVENT_ACCEPTED))
              .ForMember(dest => dest.Name, opt => opt.MapFrom(src => Keys.SEARCH_API_EVENT_NAME))
              .ForMember(dest => dest.Message, opt => opt.MapFrom(src => "Auto search has been accepted for processing"))
              .ReverseMap();

            CreateMap<PersonSearchRejected, SSG_SearchApiEvent>()
              .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
              .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(src => src.ProviderProfile.Name))
              .ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => src.TimeStamp))
              .ForMember(dest => dest.Name, opt => opt.MapFrom(src => Keys.SEARCH_API_EVENT_NAME))
              .ForMember(dest => dest.EventType, opt => opt.MapFrom(src => Keys.EVENT_REJECTED))
              .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Reasons == null ? "Auto search has been rejected." : "Auto search has been rejected. Reasons: " + string.Join(", ", src.Reasons.Select(x => $"{x.PropertyName} : {x.ErrorMessage}"))))
              .ReverseMap();

            CreateMap<PersonSearchFailed, SSG_SearchApiEvent>()
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
             .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(src => src.ProviderProfile.Name))
             .ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => src.TimeStamp))
              .ForMember(dest => dest.Name, opt => opt.MapFrom(src => Keys.SEARCH_API_EVENT_NAME))
             .ForMember(dest => dest.EventType, opt => opt.MapFrom(src => Keys.EVENT_FAILED))
             .ForMember(dest => dest.Message, opt => opt.MapFrom(src => "Auto search processing failed. Reason: " + src.Cause))
             .ReverseMap();

            CreateMap<PersonSearchCompleted, SSG_SearchApiEvent>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
               .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(src => src.ProviderProfile.Name))
               .ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => src.TimeStamp))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => Keys.SEARCH_API_EVENT_NAME))
               .ForMember(dest => dest.EventType, opt => opt.MapFrom(src => Keys.EVENT_COMPLETED))
               .ForMember(dest => dest.Message,
                          opt => opt.MapFrom(
                              src => $"Auto search processing completed successfully. {(src.MatchedPerson.Identifiers == null ? 0 : src.MatchedPerson.Identifiers.Count())} identifier(s) found.  {(src.MatchedPerson.Addresses == null ? 0 : src.MatchedPerson.Addresses.Count())} addresses found. {(src.MatchedPerson.PhoneNumbers == null ? 0 : src.MatchedPerson.PhoneNumbers.Count())} phone number(s) found."
                              )
                          )
               .ReverseMap();

            CreateMap<Address, SSG_Address>()
                 .ForMember(dest => dest.AddressLine1, opt => opt.MapFrom(src => src.AddressLine1))
                 .ForMember(dest => dest.AddressLine2, opt => opt.MapFrom(src => src.AddressLine2))
                 .ForMember(dest => dest.AddressLine3, opt => opt.MapFrom(src => src.AddressLine3))
                 .ForMember(dest => dest.Province, opt => opt.ConvertUsing(new ProvinceConverter(), src => src.StateProvince))
                 .ForMember(dest => dest.InformationSource, opt => opt.ConvertUsing(new SuppliedByValueConverter(), src => src.SuppliedBy))
                 .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                 .ForMember(dest => dest.Country, opt => opt.ConvertUsing(new CountryConverter(), src => src.CountryRegion))
                 .ForMember(dest => dest.Category, opt => opt.ConvertUsing(new AddressTypeConverter(), src => src.Type))
                 .ForMember(dest => dest.FullText, opt => opt.MapFrom<FullTextResolver>())
                 .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.ZipPostalCode))
                 .ForMember(dest => dest.EffectiveDate, opt => opt.ConvertUsing(new DateTimeOffsetConverter(),src => src.EffectiveDate))
                 .ForMember(dest => dest.EndDate, opt => opt.ConvertUsing(new DateTimeOffsetConverter(), src => src.EndDate))
                 .ForMember(dest => dest.EffectiveDateLabel, opt => opt.MapFrom(src => "Effective Date"))
                 .ForMember(dest => dest.EndDateLabel, opt => opt.MapFrom(src => "End Date"))
                 .ForMember(dest => dest.StateCode, opt => opt.MapFrom(src => 0))
                 .ForMember(dest => dest.StatusCode, opt => opt.MapFrom(src => 1));

            CreateMap<SSG_Address, Address>()
                 .ConstructUsing(m => new AddressActual() { })
                 .ForMember(dest => dest.AddressLine1, opt => opt.MapFrom(src => src.AddressLine1))
                 .ForMember(dest => dest.AddressLine2, opt => opt.MapFrom(src => src.AddressLine2))
                 .ForMember(dest => dest.StateProvince, opt => opt.ConvertUsing(new ProvinceValueConverter(), src => src.Province))
                 .ForMember(dest => dest.SuppliedBy, opt => opt.ConvertUsing(new SuppliedByIDConverter(), src => src.InformationSource))
                 .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                 .ForMember(dest => dest.CountryRegion, opt => opt.MapFrom(src => src.Country.Name))
                 .ForMember(dest => dest.Type, opt => opt.ConvertUsing(new AddressTypeValueConverter(), src => src.Category))
                 .ForMember(dest => dest.ZipPostalCode, opt => opt.MapFrom(src => src.PostalCode));


            CreateMap<PhoneNumber, SSG_PhoneNumber>()
                .ForMember(dest => dest.TelePhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber1))
                .ForMember(dest => dest.DateType, opt => opt.MapFrom(src => src.DateType))
                .ForMember(dest => dest.DateData, opt => opt.ConvertUsing(new DateTimeOffsetConverter(), src => src.Date))
                .ForMember(dest => dest.TelephoneNumberType, opt => opt.ConvertUsing(new TelephoneNumberIdConverter(), src => src.PhoneNumberType))
                .ForMember(dest => dest.InformationSource, opt => opt.ConvertUsing(new SuppliedByValueConverter(), src => src.SuppliedBy))
                .ForMember(dest => dest.StateCode, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.StatusCode, opt => opt.MapFrom(src => 1));

            CreateMap<SSG_PhoneNumber, PhoneNumber>()
                .ConstructUsing(m => new PhoneNumberActual() { })
                .ForMember(dest => dest.PhoneNumber1, opt => opt.MapFrom(src => src.TelePhoneNumber))
                .ForMember(dest => dest.DateType, opt => opt.MapFrom(src => src.DateType))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.DateData)) 
                .ForMember(dest => dest.PhoneNumberType, opt => opt.ConvertUsing(new TelephoneNumberValueConverter(), src => src.TelephoneNumberType))
                .ForMember(dest => dest.SuppliedBy, opt => opt.ConvertUsing(new SuppliedByIDConverter(), src => src.InformationSource));

            CreateMap<Name, SSG_Alias>()
                 .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                 //.ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                 //.ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.MiddleName))
                 //.ForMember(dest => dest.Type, opt => opt.ConvertUsing(new NameCategoryConverter(), src => src.Type))
                 //.ForMember(dest => dest.EffectiveDate, opt => opt.ConvertUsing(new DateTimeOffsetConverter(), src => src.EffectiveDate))
                 //.ForMember(dest => dest.EndDate, opt => opt.ConvertUsing(new DateTimeOffsetConverter(), src => src.EndDate))
                 //.ForMember(dest => dest.EffectiveDateLabel, opt => opt.MapFrom(src => "Effective Date"))
                 //.ForMember(dest => dest.EndDateLabel, opt => opt.MapFrom(src => "End Date"))
                 //.ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Description))
                 .ForMember(dest => dest.StateCode, opt => opt.MapFrom(src => 0))
                 .ForMember(dest => dest.StatusCode, opt => opt.MapFrom(src => 1));
        }
    }

}
