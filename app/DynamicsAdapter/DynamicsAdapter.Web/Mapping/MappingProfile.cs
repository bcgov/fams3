using AutoMapper;
using DynamicsAdapter.Web.PersonSearch.Models;
using Fams3Adapter.Dynamics;
using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.Name;
using Fams3Adapter.Dynamics.OptionSets.Models;
using Fams3Adapter.Dynamics.Person;
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
                 .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Identification))
                 .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                 .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                 .ForMember(dest => dest.TypeCode, opt => opt.MapFrom(src => src.SupplierTypeCode))
                 .ForMember(dest => dest.IssuedBy, opt => opt.MapFrom(src => src.IssuedBy))
                 .ForMember(dest => dest.ReferenceDates, opt => opt.MapFrom<PersonalIdentifier_ReferenceDateResolver>())
                 .ForMember(dest => dest.Type, opt => opt.ConvertUsing(new IdentifierTypeConverter(), src => src.IdentifierType));

            CreateMap<PersonalInfo, DynamicsEntity>()
               .ForMember(dest => dest.Date1, opt => opt.MapFrom<Date1Resolver>())
               .ForMember(dest => dest.Date2, opt => opt.MapFrom<Date2Resolver>())
               .ForMember(dest => dest.Date1Label, opt => opt.MapFrom<Date1LabelResolver>())
               .ForMember(dest => dest.Date2Label, opt => opt.MapFrom<Date2LabelResolver>())
               .ForMember(dest => dest.StateCode, opt => opt.MapFrom(src => 0))
               .ForMember(dest => dest.StatusCode, opt => opt.MapFrom(src => 1));

            CreateMap<PersonalIdentifier, SSG_Identifier>()
               .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
               .ForMember(dest => dest.Identification, opt => opt.MapFrom(src => src.Value))
               .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
               .ForMember(dest => dest.SupplierTypeCode, opt => opt.MapFrom(src => src.TypeCode))
               .ForMember(dest => dest.IdentifierType, opt => opt.ConvertUsing(new PersonalIdentifierTypeConverter(), src => src.Type))
               .ForMember(dest => dest.IssuedBy, opt => opt.MapFrom(src => src.IssuedBy))
               .IncludeBase<PersonalInfo, DynamicsEntity>();

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
                              src => $"Auto search processing completed successfully. {(src.MatchedPerson.Identifiers == null ? 0 : src.MatchedPerson.Identifiers.Count())} identifier(s) found.  {(src.MatchedPerson.Addresses == null ? 0 : src.MatchedPerson.Addresses.Count())} addresses found. {(src.MatchedPerson.Phones == null ? 0 : src.MatchedPerson.Phones.Count())} phone number(s) found."
                              )
                          )
               .ReverseMap();

            CreateMap<Address, SSG_Address>()
                 .ForMember(dest => dest.AddressLine1, opt => opt.MapFrom(src => src.AddressLine1))
                 .ForMember(dest => dest.AddressLine2, opt => opt.MapFrom(src => src.AddressLine2))
                 .ForMember(dest => dest.AddressLine3, opt => opt.MapFrom(src => src.AddressLine3))
                 .ForMember(dest => dest.SupplierTypeCode, opt => opt.MapFrom(src => src.Type))
                 .ForMember(dest => dest.CountrySubdivisionText, opt => opt.MapFrom(src => src.StateProvince))
                 .ForMember(dest => dest.CountryText, opt => opt.MapFrom(src => src.CountryRegion))
                 .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                 .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                 .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                 .ForMember(dest => dest.Category, opt => opt.ConvertUsing(new AddressTypeConverter(), src => src.Type))
                 .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.ZipPostalCode))
                 .IncludeBase<PersonalInfo, DynamicsEntity>();

            CreateMap<Phone, SSG_PhoneNumber>()
                .ForMember(dest => dest.TelePhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                 .ForMember(dest => dest.PhoneExtension, opt => opt.MapFrom(src => src.Extension))
                 .ForMember(dest => dest.SupplierTypeCode, opt => opt.MapFrom(src => src.Type))
                 .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                 .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                 .ForMember(dest => dest.TelephoneNumberType, opt => opt.ConvertUsing(new PhoneTypeConverter(), src => src.Type))
               .IncludeBase<PersonalInfo, DynamicsEntity>();

            CreateMap<Name, SSG_Aliase>()
                 .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                 .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                 .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.MiddleName))
                 .ForMember(dest => dest.ThirdGivenName, opt => opt.MapFrom(src => src.OtherName))
                 .ForMember(dest => dest.Type, opt => opt.ConvertUsing(new NameCategoryConverter(), src => src.Type))
                 .ForMember(dest => dest.ProviderTypeCode, opt => opt.MapFrom(src => src.Type))
                 .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Description))
                 .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                 .IncludeBase<PersonalInfo, DynamicsEntity>();

            CreateMap<Person, PersonEntity>()
               .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
               .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
               .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.MiddleName))
               .ForMember(dest => dest.ThirdGivenName, opt => opt.MapFrom(src => src.OtherName))
               .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => (src.DateOfBirth != null) ? src.DateOfBirth.Value.DateTime : (DateTime?)null))
                .ForMember(dest => dest.DateOfDeath, opt => opt.MapFrom(src => (src.DateOfDeath != null) ? src.DateOfDeath.Value.DateTime : (DateTime?)null))
                 .ForMember(dest => dest.DateOfDeathConfirmed, opt => opt.MapFrom( src => src.DateDeathConfirmed))
               .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
               .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                  .ForMember(dest => dest.StateCode, opt => opt.MapFrom(src => 0))
               .ForMember(dest => dest.StatusCode, opt => opt.MapFrom(src => 1))
               .ForMember(dest => dest.Incacerated, opt => opt.ConvertUsing(new IncaceratedConverter(), src => src.Incacerated));
              
        }
    }

}
