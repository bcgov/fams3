using AutoMapper;
using DynamicsAdapter.Web.PersonSearch.Models;
using DynamicsAdapter.Web.SearchAgency.Models;
using Fams3Adapter.Dynamics;
using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.AssetOwner;
using Fams3Adapter.Dynamics.BankInfo;
using Fams3Adapter.Dynamics.CompensationClaim;
using Fams3Adapter.Dynamics.DataProvider;
using Fams3Adapter.Dynamics.Employment;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.InsuranceClaim;
using Fams3Adapter.Dynamics.Investment;
using Fams3Adapter.Dynamics.Name;
using Fams3Adapter.Dynamics.Notes;
using Fams3Adapter.Dynamics.OtherAsset;
using Fams3Adapter.Dynamics.Pension;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.PhoneNumber;
using Fams3Adapter.Dynamics.RealEstate;
using Fams3Adapter.Dynamics.RelatedPerson;
using Fams3Adapter.Dynamics.SafetyConcern;
using Fams3Adapter.Dynamics.SearchApiEvent;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.SearchResponse;
using Fams3Adapter.Dynamics.Types;
using Fams3Adapter.Dynamics.Vehicle;
using System;
using System.Linq;

namespace DynamicsAdapter.Web.Mapping
{

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<IdentifierEntity, PersonalIdentifier>()
                 .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Identification))
                 .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                 .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                 .ForMember(dest => dest.TypeCode, opt => opt.MapFrom(src => src.SupplierTypeCode))
                 .ForMember(dest => dest.IssuedBy, opt => opt.MapFrom(src => src.IssuedBy))
                 .ForMember(dest => dest.ReferenceDates, opt => opt.MapFrom<PersonalIdentifier_ReferenceDateResolver>())
                 .ForMember(dest => dest.Type, opt => opt.ConvertUsing(new IdentifierTypeConverter(), src => src.IdentifierType))
                 .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => OwnerType.PersonSought));

            CreateMap<PersonalInfo, DynamicsEntity>()
               .ForMember(dest => dest.Date1, opt => opt.MapFrom<Date1Resolver>())
               .ForMember(dest => dest.Date2, opt => opt.MapFrom<Date2Resolver>())
               .ForMember(dest => dest.Date1Label, opt => opt.MapFrom<Date1LabelResolver>())
               .ForMember(dest => dest.Date2Label, opt => opt.MapFrom<Date2LabelResolver>())
               .ForMember(dest => dest.StateCode, opt => opt.MapFrom(src => 0))
               .ForMember(dest => dest.StatusCode, opt => opt.MapFrom(src => 1));

            CreateMap<PersonalIdentifier, IdentifierEntity>()
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
                 .ForMember(dest => dest.Identifiers, opt => opt.MapFrom(src => src.Identifiers))
                 .ForMember(dest => dest.Names, opt => opt.MapFrom<NamesResolver>())
                 .ForMember(dest => dest.Agency, opt => opt.MapFrom<AgencyResolver>())
                 .ForMember(dest => dest.JcaPerson, opt => opt.MapFrom(src => src))
                 .ForMember(dest => dest.IsPreScreenSearch, opt => opt.MapFrom(src => src.IsPrescreenSearch))
                 .ForMember(dest => dest.SearchRequestKey, opt => opt.MapFrom(src => src.SearchRequest == null ? "0" : $"{src.SearchRequest.FileId}_{src.SequenceNumber}"))
                 .ForMember(dest => dest.DataProviders, opt => opt.MapFrom(src => src.DataProviders));

            CreateMap<SSG_SearchApiRequest, JCAPerson>()
                 .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.JCAFirstName))
                 .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.JCALastName))
                 .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.JCAMiddleName))
                 .ForMember(dest => dest.MotherMaidName, opt => opt.MapFrom(src => src.JCAMotherBirthSurname))
                 .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.JCANotes))
                 .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.JCAPersonBirthDate))
                 .ForMember(dest => dest.Gender, opt => opt.ConvertUsing(new PersonGenderTypeConverter(), src => src.JCAGender));

            CreateMap<PersonSearchAccepted, SSG_SearchApiEvent>()
              .ForMember(dest => dest.EventType, opt => opt.MapFrom(src => Keys.EVENT_ACCEPTED))
              .ForMember(dest => dest.Message, opt => opt.MapFrom(src => "Auto search has been accepted for processing"))
              .IncludeBase<PersonSearchStatus, SSG_SearchApiEvent>();

            CreateMap<PersonSearchRejected, SSG_SearchApiEvent>()
              .ForMember(dest => dest.EventType, opt => opt.MapFrom(src => Keys.EVENT_REJECTED))
              .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Reasons == null ? "Auto search has been rejected." : "Auto search has been rejected. Reasons: " + string.Join(", ", src.Reasons.Select(x => $"{x.PropertyName} : {x.ErrorMessage}"))))
              .IncludeBase<PersonSearchStatus, SSG_SearchApiEvent>();

            CreateMap<PersonSearchFailed, SSG_SearchApiEvent>()
             .ForMember(dest => dest.EventType, opt => opt.MapFrom(src => Keys.EVENT_FAILED))
             .ForMember(dest => dest.Message, opt => opt.MapFrom(src => "Auto search processing failed. Reason: " + src.Cause))
             .IncludeBase<PersonSearchStatus, SSG_SearchApiEvent>();

            CreateMap<PersonSearchCompleted, SSG_SearchApiEvent>()
               .ForMember(dest => dest.EventType, opt => opt.MapFrom(src => Keys.EVENT_COMPLETED))
               .ForMember(dest => dest.Message, opt => opt.ConvertUsing(new PersonSearchCompletedMessageConvertor(), src => src))
               .IncludeBase<PersonSearchStatus, SSG_SearchApiEvent>();

            CreateMap<PersonSearchSubmitted, SSG_SearchApiEvent>()
                .ForMember(dest => dest.EventType, opt => opt.MapFrom(src => Keys.EVENT_SUBMITTED))
                .IncludeBase<PersonSearchStatus, SSG_SearchApiEvent>();

            CreateMap<PersonSearchStatus, SSG_SearchApiEvent>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
               .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(src => src.ProviderProfile.Name))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => Keys.SEARCH_API_EVENT_NAME));


            CreateMap<SearchRequestOrdered, SearchRequestEntity>()
               .ConstructUsing(m => Contructors.ConstructSearchRequestEntity(m))
               .ForMember(dest => dest.OriginalRequestorReference, opt => opt.MapFrom(src => src.Person.Agency.RequestId))
               .ForMember(dest => dest.RequestDate, opt => opt.MapFrom(src => src.Person.Agency.RequestDate.DateTime))
               .ForMember(dest => dest.SearchReasonCode, opt => opt.MapFrom(src => src.Person.Agency.ReasonCode.ToString()))
               .ForMember(dest => dest.AgencyOfficeLocationText, opt => opt.MapFrom(src => src.Person.Agency.LocationCode))
               .ForMember(dest => dest.AgencyCode, opt => opt.MapFrom(src => src.Person.Agency.Code))
               .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Person.Agency.Notes))
               .ForMember(dest => dest.PersonSoughtDateOfBirth, opt => opt.MapFrom(src => src.Person.DateOfBirth == null ? (DateTime?)null : ((DateTimeOffset)(src.Person.DateOfBirth)).DateTime))
               .ForMember(dest => dest.PersonSoughtEyeColor, opt => opt.MapFrom(src => src.Person.EyeColour))
               .ForMember(dest => dest.PersonSoughtHairColor, opt => opt.MapFrom(src => src.Person.HairColour))
               .ForMember(dest => dest.PersonSoughtFirstName, opt => opt.MapFrom(src => src.Person.FirstName))
               .ForMember(dest => dest.PersonSoughtLastName, opt => opt.MapFrom(src => src.Person.LastName))
               .ForMember(dest => dest.PersonSoughtGender, opt => opt.ConvertUsing(new PersonGenderConverter(), src => src.Person.Gender))
               .ForMember(dest => dest.RequestPriority, opt => opt.ConvertUsing(new RequestPriorityConverter(), src => src.Person.Agency.RequestPriority))
               ;

            CreateMap<Address, AddressEntity>()
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
                 .ForMember(dest => dest.IncarcerationStatus, opt => opt.MapFrom(src => src.IncarcerationStatus))
                 .IncludeBase<PersonalInfo, DynamicsEntity>()
                 .ReverseMap()
                    .ForMember(dest => dest.AddressLine1, opt => opt.MapFrom(src => src.CouldNotLocate ? "Could Not Locate" : src.AddressLine1))
                    .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => OwnerType.PersonSought))
                    .ForMember(dest => dest.Type, opt => opt.ConvertUsing(new AddressTypeResponseConverter(), src => src.Category));

            CreateMap<Employment, EmploymentEntity>()
              .ForMember(dest => dest.AddressLine1, opt => opt.MapFrom(src => (src.Employer == null) ? string.Empty : (src.Employer.Address == null) ? string.Empty : src.Employer.Address.AddressLine1))
              .ForMember(dest => dest.AddressLine2, opt => opt.MapFrom(src => (src.Employer == null) ? string.Empty : (src.Employer.Address == null) ? string.Empty : src.Employer.Address.AddressLine2))
              .ForMember(dest => dest.AddressLine3, opt => opt.MapFrom(src => (src.Employer == null) ? string.Empty : (src.Employer.Address == null) ? string.Empty : src.Employer.Address.AddressLine3))
              .ForMember(dest => dest.BusinessName, opt => opt.MapFrom(src => (src.Employer == null) ? string.Empty : src.Employer.Name))
              .ForMember(dest => dest.Occupation, opt => opt.MapFrom(src => src.Occupation))
              .ForMember(dest => dest.Notes, opt => opt.MapFrom<EmploymentEntityNotesResolver>())
              .ForMember(dest => dest.Website, opt => opt.MapFrom(src => src.Website))
              .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => (src.Employer == null) ? string.Empty : (src.Employer.Address == null) ? string.Empty : src.Employer.Address.ZipPostalCode))
              .ForMember(dest => dest.City, opt => opt.MapFrom(src => (src.Employer == null) ? string.Empty : (src.Employer.Address == null) ? string.Empty : src.Employer.Address.City))
              .ForMember(dest => dest.IncomeAssistanceStatus, opt => opt.MapFrom(src => src.IncomeAssistanceStatus))
              .ForMember(dest => dest.IncomeAssistanceStatusOption, opt => opt.ConvertUsing(new IncomeAssistanceStatusConvertor(), src => src.IncomeAssistanceStatus))
              .ForMember(dest => dest.EmploymentType, opt => opt.ConvertUsing(new IncomeAssistanceConvertor(), src => src.IncomeAssistanceStatus))
              .ForMember(dest => dest.CountrySubdivisionText, opt => opt.MapFrom(src => (src.Employer == null) ? string.Empty : (src.Employer.Address == null) ? string.Empty : src.Employer.Address.StateProvince))
              .ForMember(dest => dest.CountryText, opt => opt.MapFrom(src => (src.Employer == null) ? string.Empty : (src.Employer.Address == null) ? string.Empty : src.Employer.Address.CountryRegion))
              .ForMember(dest => dest.BusinessOwner, opt => opt.MapFrom(src => (src.Employer == null) ? string.Empty : src.Employer.OwnerName))
              .ForMember(dest => dest.EmploymentConfirmed, opt => opt.MapFrom(src => src.EmploymentConfirmed))
              .ForMember(dest => dest.PrimaryPhoneNumber, opt => opt.ConvertUsing(new PrimaryPhoneNumberConvertor(), src => src.Employer == null ? null : src.Employer.Phones))
              .ForMember(dest => dest.PrimaryPhoneExtension, opt => opt.ConvertUsing(new PrimaryPhoneExtConvertor(), src => src.Employer == null ? null : src.Employer.Phones))
              .ForMember(dest => dest.PrimaryContactPhone, opt => opt.ConvertUsing(new PrimaryContactPhoneNumberConvertor(), src => src.Employer == null ? null : src.Employer.Phones))
              .ForMember(dest => dest.PrimaryContactPhoneExt, opt => opt.ConvertUsing(new PrimaryContactPhoneExtConvertor(), src => src.Employer == null ? null : src.Employer.Phones))
              .ForMember(dest => dest.ContactPerson, opt => opt.MapFrom(src => (src.Employer == null) ? string.Empty : src.Employer.ContactPerson))
              .IncludeBase<PersonalInfo, DynamicsEntity>();


            CreateMap<Phone, EmploymentContactEntity>()
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Type.Equals("Phone", StringComparison.InvariantCultureIgnoreCase) ? src.PhoneNumber : null))
                .ForMember(dest => dest.OriginalPhoneNumber, opt => opt.MapFrom(src => src.Type.Equals("Phone", StringComparison.InvariantCultureIgnoreCase) ? src.PhoneNumber : null))
                .ForMember(dest => dest.PhoneExtension, opt => opt.MapFrom(src => src.Extension))
                .ForMember(dest => dest.FaxNumber, opt => opt.MapFrom(src => src.Type.Equals("fax", StringComparison.InvariantCultureIgnoreCase) ? src.PhoneNumber : null))
                .ForMember(dest => dest.OriginalFaxNumber, opt => opt.MapFrom(src => src.Type.Equals("fax", StringComparison.InvariantCultureIgnoreCase) ? src.PhoneNumber : null))
                .ForMember(dest => dest.SupplierTypeCode, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.PhoneType, opt => opt.ConvertUsing(new PhoneTypeConverter(), src => src.Type))
                .ForMember(dest => dest.StateCode, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.StatusCode, opt => opt.MapFrom(src => 1))
                .ReverseMap();

            CreateMap<EmploymentEntity, Phone>()
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PrimaryPhoneNumber))
                .ForMember(dest => dest.Extension, opt => opt.MapFrom(src => src.PrimaryPhoneExtension))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "primary phone"));


            CreateMap<Phone, PhoneNumberEntity>()
                .ForMember(dest => dest.TelePhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                 .ForMember(dest => dest.PhoneExtension, opt => opt.MapFrom(src => src.Extension))
                 .ForMember(dest => dest.SupplierTypeCode, opt => opt.MapFrom(src => src.Type))
                 .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                 .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                 .ForMember(dest => dest.TelephoneNumberType, opt => opt.ConvertUsing(new PhoneTypeConverter(), src => src.Type))
               .IncludeBase<PersonalInfo, DynamicsEntity>()
               .ReverseMap()
                    .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                    .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => OwnerType.PersonSought))
                    .ForMember(dest => dest.Type, opt => opt.ConvertUsing(new PhoneTypeResponseConverter(), src => src.TelephoneNumberType));

            CreateMap<Name, AliasEntity>()
                 .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                 .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                 .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.MiddleName))
                 .ForMember(dest => dest.ThirdGivenName, opt => opt.MapFrom(src => src.OtherName))
                 .ForMember(dest => dest.Type, opt => opt.ConvertUsing(new NameCategoryConverter(), src => src.Type))
                 .ForMember(dest => dest.SupplierTypeCode, opt => opt.MapFrom(src => src.Type))
                 .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => $"{src.Notes} {src.Description}"))
                 .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                 .IncludeBase<PersonalInfo, DynamicsEntity>()
                 .ReverseMap()
                    .ForMember(dest => dest.Owner, opt => opt.Ignore());

            CreateMap<RelatedPerson, RelatedPersonEntity>()
                 .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                 .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                 .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.MiddleName))
                 .ForMember(dest => dest.ThirdGivenName, opt => opt.MapFrom(src => src.OtherName))
                 .ForMember(dest => dest.Type, opt => opt.ConvertUsing(new RelatedPersonCategoryConverter(), src => src.Type))
                 .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth == null ? (DateTime?)null : src.DateOfBirth.Value.DateTime))
                 .ForMember(dest => dest.SupplierRelationType, opt => opt.MapFrom(src => src.Type))
                 .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                 .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                 .ForMember(dest => dest.Gender, opt => opt.ConvertUsing(new PersonGenderConverter(), src => src.Gender))
                 .ForMember(dest => dest.PersonType, opt => opt.MapFrom(src => RelatedPersonPersonType.Relation.Value))
                 .IncludeBase<PersonalInfo, DynamicsEntity>()
                 .ReverseMap()
                    .ForMember(dest => dest.Gender, opt => opt.ConvertUsing(new PersonGenderTypeConverter(), src => src.Gender))
                    .ForMember(dest => dest.Type, opt => opt.ConvertUsing(new RelatedPersonCategoryResponseConverter(), src => src.Type))
                    .ForMember(dest => dest.PersonType, opt => opt.ConvertUsing(new RelatedPersonTypeResponseConverter(), src => src.PersonType))
                    .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth == null ? (DateTimeOffset?)null : new DateTimeOffset((DateTime)src.DateOfBirth)))
                    .ForMember(dest => dest.DateOfDeath, opt => opt.MapFrom(src => src.DateOfDeath == null ? (DateTimeOffset?)null : new DateTimeOffset((DateTime)src.DateOfDeath)))
                    ;

            CreateMap<SSG_SearchapiRequestDataProvider, DataProvider>()
                 .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.AdaptorName.Replace(" ", String.Empty).ToUpperInvariant()))
                   .ForMember(dest => dest.NumberOfRetries, opt => opt.MapFrom(src => src.NumberOfRetries))
                   .ForMember(dest => dest.TimeBetweenRetries, opt => opt.MapFrom(src => src.TimeBetweenRetries))
              .ForMember(dest => dest.SearchSpeedType, opt => opt.MapFrom(src => (src.SearchSpeed == AutoSearchSpeedType.Fast.Value) ? SearchSpeedType.Fast : SearchSpeedType.Slow));

            CreateMap<SSG_Notese, ResponseNote>()
                    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<Person, PersonEntity>()
               .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
               .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
               .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.MiddleName))
               .ForMember(dest => dest.ThirdGivenName, opt => opt.MapFrom(src => src.OtherName))
               .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => (src.DateOfBirth != null) ? src.DateOfBirth.Value.DateTime : (DateTime?)null))
               .ForMember(dest => dest.DateOfDeath, opt => opt.MapFrom(src => (src.DateOfDeath != null) ? src.DateOfDeath.Value.DateTime : (DateTime?)null))
               .ForMember(dest => dest.DateOfDeathConfirmed, opt => opt.MapFrom(src => src.DateDeathConfirmed))
               .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
               .ForMember(dest => dest.GenderOptionSet, opt => opt.ConvertUsing(new PersonGenderConverter(), src => src.Gender))
               .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
               .ForMember(dest => dest.StateCode, opt => opt.MapFrom(src => 0))
               .ForMember(dest => dest.StatusCode, opt => opt.MapFrom(src => 1))
               .ForMember(dest => dest.Incacerated, opt => opt.ConvertUsing(new IncaceratedConverter(), src => src.Incacerated))
               .ForMember(dest => dest.Complexion, opt => opt.MapFrom(src => src.Complexion))
               .ForMember(dest => dest.EyeColor, opt => opt.MapFrom(src => src.EyeColour))
               .ForMember(dest => dest.HairColor, opt => opt.MapFrom(src => src.HairColour))
               .ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.Height))
               .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
               .ForMember(dest => dest.WearGlasses, opt => opt.MapFrom(src => src.WearGlasses))
               .ForMember(dest => dest.DistinguishingFeatures, opt => opt.MapFrom(src => src.DistinguishingFeatures));

            CreateMap<BankInfo, BankingInformationEntity>()
                 .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.AccountNumber))
                 .ForMember(dest => dest.Branch, opt => opt.MapFrom(src => src.Branch))
                 .ForMember(dest => dest.BankName, opt => opt.MapFrom(src => src.BankName))
                 .ForMember(dest => dest.TransitNumber, opt => opt.MapFrom(src => src.TransitNumber))
                 .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => $"{src.Notes} {src.Description}"))
                 .IncludeBase<PersonalInfo, DynamicsEntity>()
                 .ReverseMap()
                    .ForMember(dest => dest.BranchNumber, opt => opt.MapFrom(src => src.BranchNumber))
                    .ForMember(dest => dest.AccountType, opt => opt.ConvertUsing(new AccountTypeResponseConverter(), src => src.AccountType))
                    ;

            CreateMap<Employer, EmploymentEntity>()
                  .ForMember(dest => dest.AddressLine1, opt => opt.MapFrom(src => (src.Address == null) ? string.Empty : src.Address.AddressLine1))
                  .ForMember(dest => dest.AddressLine2, opt => opt.MapFrom(src => (src.Address == null) ? string.Empty : src.Address.AddressLine2))
                  .ForMember(dest => dest.AddressLine3, opt => opt.MapFrom(src => (src.Address == null) ? string.Empty : src.Address.AddressLine3))
                  .ForMember(dest => dest.BusinessName, opt => opt.MapFrom(src => src.Name))
                  .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => (src.Address == null) ? string.Empty : src.Address.ZipPostalCode))
                  .ForMember(dest => dest.City, opt => opt.MapFrom(src => (src.Address == null) ? string.Empty : src.Address.City))
                  .ForMember(dest => dest.CountrySubdivisionText, opt => opt.MapFrom(src => (src.Address == null) ? string.Empty : src.Address.StateProvince))
                  .ForMember(dest => dest.CountryText, opt => opt.MapFrom(src => (src.Address == null) ? string.Empty : src.Address.CountryRegion))
                  .ForMember(dest => dest.BusinessOwner, opt => opt.MapFrom(src => src.OwnerName))
                  .ForMember(dest => dest.ContactPerson, opt => opt.MapFrom(src => src.ContactPerson))
                  .ForMember(dest => dest.StateCode, opt => opt.MapFrom(src => 0))
                  .ForMember(dest => dest.StatusCode, opt => opt.MapFrom(src => 1));

            CreateMap<SSG_Employment, Employer>()
                  .ForMember(dest => dest.DbaName, opt => opt.MapFrom(src => src.DBAName))
                  .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CouldNotLocate ? "Could Not Locate" : src.BusinessName))
                  .ForMember(dest => dest.Phones, opt => opt.MapFrom<EmployerPhoneResponseResolver>())
                  .ForMember(dest => dest.ContactPerson, opt => opt.MapFrom(src => src.ContactPerson))
                  .ForMember(dest => dest.Address, opt => opt.MapFrom<EmployerAddressResponseResolver>());

            CreateMap<SSG_Employment, Employment>()
                    .ForMember(dest => dest.IncomeAssistance, opt => opt.ConvertUsing(new IncomeAssistanceResponseConvertor(), src => src.EmploymentType))
                    .ForMember(dest => dest.Employer, opt => opt.MapFrom(src => src))
                    .ForMember(dest => dest.Website, opt => opt.MapFrom(src => src.Website))
                    .ForMember(dest => dest.EmploymentStatus, opt => opt.ConvertUsing(new EmploymentStatusResponseConverter(), src => src.EmploymentStatus))
                    .ForMember(dest => dest.SelfEmployComRegistrationNo, opt => opt.MapFrom(src => src.SelfEmployComRegistrationNo))
                    .ForMember(dest => dest.SelfEmployComType, opt => opt.ConvertUsing(new SelfEmployComTypeResponseConverter(), src => src.SelfEmployComType))
                    .ForMember(dest => dest.Occupation, opt => opt.MapFrom(src => src.Occupation))
                    .ForMember(dest => dest.SelfEmployComRole, opt => opt.ConvertUsing(new SelfEmployComRoleResponseConverter(), src => src.SelfEmployComType))
                    .ForMember(dest => dest.SelfEmployPercentOfShare, opt => opt.MapFrom(src => src.SelfEmployPercentOfShare))
                    .ForMember(dest => dest.IncomeAssistanceStatus, opt => opt.ConvertUsing(new IncomeAssistanceStatusResponseConverter(), src => src.IncomeAssistanceStatusOption))
                    .ForMember(dest => dest.IncomeAssistanceDesc, opt => opt.MapFrom(src => src.IncomeAssistanceDesc))
            ;

            CreateMap<Vehicle, VehicleEntity>()
                 .ForMember(dest => dest.OwnershipType, opt => opt.MapFrom(src => src.OwnershipType))
                 .ForMember(dest => dest.PlateNumber, opt => opt.MapFrom(src => src.PlateNumber))
                 .ForMember(dest => dest.Discription, opt => opt.MapFrom(src => src.Description))
                 .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                 .ForMember(dest => dest.Vin, opt => opt.MapFrom(src => src.Vin))
                 .IncludeBase<PersonalInfo, DynamicsEntity>()
                 .ReverseMap()
                    .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Year))
                    .ForMember(dest => dest.Make, opt => opt.MapFrom(src => src.Make))
                    .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color))
                    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                    .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Model))
                    .ForMember(dest => dest.Owners, opt => opt.MapFrom<VehicleOwnersRespnseResolver>())
                 ;

            CreateMap<VehicleEntity, InvolvedParty>()
             .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Lessee))
             .ForMember(dest => dest.Organization, opt => opt.MapFrom(src => src.LeasingCom))
             .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.LeasingComAddr))
             .ForMember(dest => dest.Type, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Lessee) ? null : "Leased"))
             ;

            CreateMap<InvolvedParty, AssetOwnerEntity>()
                 .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                 .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Name.FirstName))
                 .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Name.LastName))
                 .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                 .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.Name.MiddleName))
                 .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                 .ForMember(dest => dest.OtherName, opt => opt.MapFrom(src => src.Name.OtherName))
                 .ForMember(dest => dest.OrganizationName, opt => opt.MapFrom(src => src.Organization));

            CreateMap<OtherAsset, AssetOtherEntity>()
                  .ForMember(dest => dest.TypeDescription, opt => opt.MapFrom(src => src.TypeDescription))
                  .ForMember(dest => dest.Description, opt => opt.MapFrom(src => $"{src.ReferenceDescription} {src.ReferenceValue}"))
                  .ForMember(dest => dest.AssetDescription, opt => opt.MapFrom(src => src.Description))
                  .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                  .IncludeBase<PersonalInfo, DynamicsEntity>()
                  .ReverseMap();

            CreateMap<CompensationClaim, CompensationClaimEntity>()
                  .ForMember(dest => dest.ClaimType, opt => opt.MapFrom(src => src.ClaimType))
                  .ForMember(dest => dest.ClaimantNumber, opt => opt.MapFrom(src => src.ClaimantNumber))
                  .ForMember(dest => dest.ClaimNumber, opt => opt.MapFrom(src => src.ClaimNumber))
                  .ForMember(dest => dest.ClaimStatus, opt => opt.MapFrom(src => src.ClaimStatus))
                  .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                  .IncludeBase<PersonalInfo, DynamicsEntity>()
                  .ReverseMap()
                    .ForMember(dest => dest.ClaimAmount, opt => opt.MapFrom(src => src.ClaimAmount));

            CreateMap<Person, SafetyConcernEntity>()
                    .ForMember(dest => dest.Detail, opt => opt.MapFrom(src => $"{src.CautionFlag} {src.CautionReason} {src.CautionNotes}"))
                    .ForMember(dest => dest.Type, opt => opt.ConvertUsing(new SafetyConcernTypeConverter(), src => src.CautionReason))
                    .IncludeBase<PersonalInfo, DynamicsEntity>();

            CreateMap<InsuranceClaim, ICBCClaimEntity>()
                  .ForMember(dest => dest.ClaimType, opt => opt.MapFrom(src => src.ClaimType))
                  .ForMember(dest => dest.ClaimNumber, opt => opt.MapFrom(src => src.ClaimNumber))
                  .ForMember(dest => dest.ClaimStatus, opt => opt.MapFrom(src => src.ClaimStatus))
                  .ForMember(dest => dest.AdjusterFirstName, opt => opt.MapFrom(src => src.Adjustor == null ? null : src.Adjustor.FirstName))
                  .ForMember(dest => dest.AdjusterLastName, opt => opt.MapFrom(src => src.Adjustor == null ? null : src.Adjustor.LastName))
                  .ForMember(dest => dest.AdjusterMiddleName, opt => opt.MapFrom(src => src.Adjustor == null ? null : src.Adjustor.MiddleName))
                  .ForMember(dest => dest.AdjusterOtherName, opt => opt.MapFrom(src => src.Adjustor == null ? null : src.Adjustor.OtherName))
                  .ForMember(dest => dest.AdjusterPhoneNumber, opt => opt.MapFrom(src => src.AdjustorPhone == null ? null : src.AdjustorPhone.PhoneNumber))
                  .ForMember(dest => dest.OriginalAdjusterPhoneNumber, opt => opt.MapFrom(src => src.AdjustorPhone == null ? null : src.AdjustorPhone.PhoneNumber))
                  .ForMember(dest => dest.AdjusterPhoneNumberExt, opt => opt.MapFrom(src => src.AdjustorPhone == null ? null : src.AdjustorPhone.Extension))
                  .ForMember(dest => dest.PHNNumber, opt => opt.MapFrom(src => src.Identifiers == null ? null : src.Identifiers.FirstOrDefault<PersonalIdentifier>(m => m.Type == PersonalIdentifierType.PersonalHealthNumber).Value))
                  .ForMember(dest => dest.BCDLNumber, opt => opt.MapFrom(src => src.Identifiers == null ? null : src.Identifiers.FirstOrDefault<PersonalIdentifier>(m => m.Type == PersonalIdentifierType.BCDriverLicense).Value))
                  .ForMember(dest => dest.BCDLStatus, opt => opt.MapFrom(src => src.Identifiers == null ? null : src.Identifiers.FirstOrDefault<PersonalIdentifier>(m => m.Type == PersonalIdentifierType.BCDriverLicense).Description))
                  .ForMember(dest => dest.BCDLExpiryDate, opt => opt.MapFrom(src => src.Identifiers == null ? null : src.Identifiers.FirstOrDefault<PersonalIdentifier>(m => m.Type == PersonalIdentifierType.BCDriverLicense).ReferenceDates.ElementAt(0).Value.ToString()))
                  .ForMember(dest => dest.ClaimCenterLocationCode, opt => opt.MapFrom(src => src.ClaimCentre == null ? null : src.ClaimCentre.Location))
                  .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                  .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                  .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.ClaimCentre == null ? null : src.ClaimCentre.ContactAddress == null ? null : src.ClaimCentre.ContactAddress.ZipPostalCode))
                  .ForMember(dest => dest.AddressLine1, opt => opt.MapFrom(src => src.ClaimCentre == null ? null : src.ClaimCentre.ContactAddress == null ? null : src.ClaimCentre.ContactAddress.AddressLine1))
                  .ForMember(dest => dest.AddressLine2, opt => opt.MapFrom(src => src.ClaimCentre == null ? null : src.ClaimCentre.ContactAddress == null ? null : src.ClaimCentre.ContactAddress.AddressLine2))
                  .ForMember(dest => dest.AddressLine3, opt => opt.MapFrom(src => src.ClaimCentre == null ? null : src.ClaimCentre.ContactAddress == null ? null : src.ClaimCentre.ContactAddress.AddressLine3))
                  .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.ClaimCentre == null ? null : src.ClaimCentre.ContactAddress == null ? null : src.ClaimCentre.ContactAddress.City))
                  .ForMember(dest => dest.SupplierCountryCode, opt => opt.MapFrom(src => src.ClaimCentre == null ? null : src.ClaimCentre.ContactAddress == null ? null : src.ClaimCentre.ContactAddress.CountryRegion))
                  .ForMember(dest => dest.SupplierCountrySubdivisionCode, opt => opt.MapFrom(src => src.ClaimCentre == null ? null : src.ClaimCentre.ContactAddress == null ? null : src.ClaimCentre.ContactAddress.StateProvince))
                  .IncludeBase<PersonalInfo, DynamicsEntity>()
                  .ReverseMap()
                    .ForMember(dest => dest.ClaimAmount, opt => opt.MapFrom(src => src.ClaimAmount))
                    .ForMember(dest => dest.Adjustor, opt => opt.MapFrom(src =>
                        (string.IsNullOrEmpty(src.AdjusterFirstName) && string.IsNullOrEmpty(src.AdjusterLastName)) ? null : new Name { FirstName = src.AdjusterFirstName, LastName = src.AdjusterLastName }))
                    .ForMember(dest => dest.AdjustorPhone, opt => opt.MapFrom(src =>
                        string.IsNullOrEmpty(src.AdjusterPhoneNumber) ? null : new Phone { PhoneNumber = src.AdjusterPhoneNumber, Extension = src.AdjusterPhoneNumberExt }))
                    .ForMember(dest => dest.ClaimCentre, opt => opt.MapFrom(src =>
                        string.IsNullOrEmpty(src.ClaimCenterLocationCode) ? null : new ClaimCentre { Location = src.ClaimCenterLocationCode }))
                  ;

            CreateMap<Phone, SimplePhoneNumberEntity>()
                  .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                  .ForMember(dest => dest.OriginalPhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                  .ForMember(dest => dest.Extension, opt => opt.MapFrom(src => src.Extension))
                  .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type));

            CreateMap<InvolvedParty, InvolvedPartyEntity>()
                  .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Name == null ? null : src.Name.FirstName))
                  .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Name == null ? null : src.Name.LastName))
                  .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.Name == null ? null : src.Name.MiddleName))
                  .ForMember(dest => dest.OtherName, opt => opt.MapFrom(src => src.Name == null ? null : src.Name.OtherName))
                  .ForMember(dest => dest.OrganizationName, opt => opt.MapFrom(src => src.Organization))
                  .ForMember(dest => dest.PartyTypeCode, opt => opt.MapFrom(src => src.Type))
                  .ForMember(dest => dest.PartyDescription, opt => opt.MapFrom(src => src.TypeDescription))
                  .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes));

            CreateMap<DynamicsEntity, PersonalInfo>()
               .ForMember(dest => dest.ReferenceDates, opt => opt.MapFrom<ReferenceDatesResolver>());


            CreateMap<SSG_SearchRequest, Agency>()
                   .ForMember(dest => dest.Agent, opt => opt.MapFrom(src => src))
                   .ForMember(dest => dest.RequestDate, opt => opt.MapFrom(src => src.RequestDate))
                   .ForMember(dest => dest.ReasonCode, opt => opt.ConvertUsing(new SearchReasonCodeConverter(), src => src.SearchReason))
                   .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Agency.AgencyCode))
                   .ForMember(dest => dest.DaysOpen, opt => opt.ConvertUsing(new MinsToDaysConverter(), src => src.MinsOpen))
                   .ForMember(dest => dest.RequestId, opt => opt.MapFrom(src => src.OriginalRequestorReference))
                   .ForMember(dest => dest.RequestPriority, opt => opt.ConvertUsing(new RequestPriorityTypeConverter(), src => src.RequestPriority));

            CreateMap<SSG_SearchRequest, Name>()
                   .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.AgentFirstName))
                   .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.AgentLastName));

            CreateMap<SSG_Asset_Investment, Investment>()
                .ForMember(dest => dest.MaturityDate, opt => opt.MapFrom(src => src.MaturityDate == null ? (DateTimeOffset?)null : new DateTimeOffset((DateTime)src.MaturityDate)))
                .IncludeBase<DynamicsEntity, PersonalInfo>();

            CreateMap<SSG_SafetyConcernDetail, SafetyConcern>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Detail))
                .ForMember(dest => dest.Type, opt => opt.ConvertUsing(new SafetyConcernTypeResponseConverter(), src => src.Type))
                .IncludeBase<DynamicsEntity, PersonalInfo>();

            CreateMap<SSG_Asset_PensionDisablility, Pension>()
                .ForMember(dest => dest.Provider, opt => opt.MapFrom(src => src.Provider))
                .ForMember(dest => dest.ProviderPhone, opt => opt.MapFrom(src => src.ProviderPhone))
                .ForMember(dest => dest.BalanceAmount_base, opt => opt.MapFrom(src => src.BalanceAmount_base.ToString()))
                .ForMember(dest => dest.BalanceAmount, opt => opt.MapFrom(src => src.BalanceAmount.ToString()))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency))
                .ForMember(dest => dest.ExchangeRate, opt => opt.MapFrom(src => src.ExchangeRate.ToString()))
                .ForMember(dest => dest.ProviderAddress, opt => opt.MapFrom<ProviderAddressResponseResolver>())
                .IncludeBase<DynamicsEntity, PersonalInfo>();

            CreateMap<SSG_Asset_RealEstateProperty, RealEstateProperty>()
                .ForMember(dest => dest.Pid, opt => opt.MapFrom(src => src.PID))
                .ForMember(dest => dest.TitleNumber, opt => opt.MapFrom(src => src.TitleNumber))
                .ForMember(dest => dest.LandTitleDistrict, opt => opt.MapFrom(src => src.LandTitleDistrict))
                .ForMember(dest => dest.NumberOfOwners, opt => opt.MapFrom(src => src.NumberOfOwners))
                .ForMember(dest => dest.LegalDescription, opt => opt.MapFrom(src => src.LegalDescription))
                .ForMember(dest => dest.PropertyAddress, opt => opt.MapFrom<PropertyAddressResponseResolver>())
                .IncludeBase<DynamicsEntity, PersonalInfo>();

            CreateMap<SSG_Person, ResponsePerson>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.MiddleName))
                .ForMember(dest => dest.OtherName, opt => opt.MapFrom(src => src.ThirdGivenName))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.DateOfDeath, opt => opt.MapFrom(src => src.DateOfDeath))
                .ForMember(dest => dest.Gender, opt => opt.ConvertUsing(new PersonGenderTypeConverter(), src => src.GenderOptionSet))
                .ForMember(dest => dest.Complexion, opt => opt.MapFrom(src => src.Complexion))
                .ForMember(dest => dest.EyeColour, opt => opt.MapFrom(src => src.EyeColor))
                .ForMember(dest => dest.HairColour, opt => opt.MapFrom(src => src.HairColor))
                .ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.Height))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
                .ForMember(dest => dest.WearGlasses, opt => opt.MapFrom(src => src.WearGlasses))
                .ForMember(dest => dest.DistinguishingFeatures, opt => opt.MapFrom(src => src.DistinguishingFeatures))
                .IncludeBase<DynamicsEntity, PersonalInfo>()
                .ForMember(dest => dest.ResponseComments, opt => opt.MapFrom(src => src.ResponseComments));

            CreateMap<SSG_SearchRequestResponse, Person>()
               .ForMember(dest => dest.Names, opt => opt.MapFrom(src => src.SSG_Aliases))
               .ForMember(dest => dest.Identifiers, opt => opt.MapFrom(src => src.SSG_Identifiers))
               .ForMember(dest => dest.Addresses, opt => opt.MapFrom(src => src.SSG_Addresses))
               .ForMember(dest => dest.Phones, opt => opt.MapFrom(src => src.SSG_PhoneNumbers))
               .ForMember(dest => dest.Employments, opt => opt.MapFrom(src => src.SSG_Employments))
               .ForMember(dest => dest.Vehicles, opt => opt.MapFrom(src => src.SSG_Asset_Vehicles))
               .ForMember(dest => dest.InsuranceClaims, opt => opt.MapFrom(src => src.SSG_Asset_ICBCClaims))
               .ForMember(dest => dest.CompensationClaims, opt => opt.MapFrom(src => src.SSG_Asset_WorkSafeBcClaims))
               .ForMember(dest => dest.BankInfos, opt => opt.MapFrom(src => src.SSG_BankInfos))
               .ForMember(dest => dest.RelatedPersons, opt => opt.MapFrom(src => src.SSG_Identities))
               .ForMember(dest => dest.ResponseNotes, opt => opt.MapFrom(src => src.SSG_Noteses))
               .ForMember(dest => dest.Investments, opt => opt.MapFrom(src => src.SSG_Asset_Investments))
               .ForMember(dest => dest.OtherAssets, opt => opt.MapFrom(src => src.SSG_Asset_Others))
               .ForMember(dest => dest.SafetyConcerns, opt => opt.MapFrom(src => src.SSG_SafetyConcernDetails))
               .ForMember(dest => dest.Pensions, opt => opt.MapFrom(src => src.SSG_Asset_PensionDisablilitys))
               .ForMember(dest => dest.RealEstateProperties, opt => opt.MapFrom(src => src.SSG_Asset_RealEstatePropertys))
               .ForMember(dest => dest.ResponsePersons, opt => opt.MapFrom(src => src.SSG_Persons))
               .ForMember(dest => dest.Type, opt => opt.ConvertUsing(new PersonSoughtRoleConverter(), src => src.SSG_SearchRequests[0].PersonSoughtRole))
               .ForMember(dest => dest.Agency, opt => opt.MapFrom(src => src.SSG_SearchRequests[0]))
               .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.SSG_SearchRequests[0].PersonSoughtFirstName))
               .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.SSG_SearchRequests[0].PersonSoughtLastName))
               .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.SSG_SearchRequests[0].PersonSoughtMiddleName))
               .ForMember(dest => dest.OtherName, opt => opt.MapFrom(src => src.SSG_SearchRequests[0].PersonSoughtThirdGiveName))
               .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.SSG_SearchRequests[0].PersonSoughtDateOfBirth))
               .ForMember(dest => dest.Gender, opt => opt.ConvertUsing(new PersonGenderTypeConverter(), src => src.SSG_SearchRequests[0].PersonSoughtGender))
               ;
        }
    }

}
