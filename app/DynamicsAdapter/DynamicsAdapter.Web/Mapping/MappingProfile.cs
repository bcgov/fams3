using AutoMapper;
using DynamicsAdapter.Web.PersonSearch.Models;
using Fams3Adapter.Dynamics;
using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.AssetOwner;
using Fams3Adapter.Dynamics.BankInfo;
using Fams3Adapter.Dynamics.CompensationClaim;
using Fams3Adapter.Dynamics.DataProvider;
using Fams3Adapter.Dynamics.Employment;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.InsuranceClaim;
using Fams3Adapter.Dynamics.Name;
using Fams3Adapter.Dynamics.OtherAsset;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.PhoneNumber;
using Fams3Adapter.Dynamics.RelatedPerson;
using Fams3Adapter.Dynamics.SearchApiEvent;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Vehicle;
using System;
using System.Linq;
using DynamicsAdapter.Web.SearchAgency.Models;

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
                 .ForMember(dest => dest.Type, opt => opt.ConvertUsing(new IdentifierTypeConverter(), src => src.IdentifierType));

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
                 .ForMember(dest => dest.SearchRequestKey, opt => opt.MapFrom(src => src.SearchRequest == null ? "0": $"{src.SearchRequest.FileId}_{src.SequenceNumber}"))
                 .ForMember(dest => dest.DataProviders, opt => opt.MapFrom(src => src.DataProviders));

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
               .ForMember(dest => dest.Message, opt => opt.ConvertUsing(new PersonSearchCompletedMessageConvertor(), src => src.MatchedPersons))
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
               .ForMember(dest => dest.OriginalRequestorReference, opt => opt.MapFrom( src => src.Person.Agency.RequestId))
               .ForMember(dest => dest.RequestDate, opt => opt.MapFrom(src => src.Person.Agency.RequestDate.DateTime))
               .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Person.Agency.Notes))
               .ForMember(dest => dest.PersonSoughtDateOfBirth, opt => opt.MapFrom(src => src.Person.DateOfBirth))
               .ForMember(dest => dest.PersonSoughtEyeColor, opt => opt.MapFrom(src => src.Person.EyeColour))
               .ForMember(dest => dest.PersonSoughtHairColor, opt => opt.MapFrom(src => src.Person.HairColour))
               .ForMember(dest => dest.PersonSoughtFirstName, opt => opt.MapFrom(src => src.Person.FirstName))
               .ForMember(dest => dest.PersonSoughtLastName, opt => opt.MapFrom(src => src.Person.LastName))
                //.ForMember(dest => dest.AgentLastName, opt => opt.MapFrom(src => src.Person == null ? null : src.Person.Agency == null ? null : src.Person.Agency.Agent == null ? null : src.Person.Agency.Agent.LastName))
                //.ForMember(dest => dest.AgentPhoneNumber, opt => opt.MapFrom<SearchRequestEntity_AgentPhoneNumberResolver>())
                //.ForMember(dest => dest.AgentPhoneExtension, opt => opt.MapFrom<SearchRequestEntity_AgentPhoneExtResolver>())
                //.ForMember(dest => dest.AgentFax, opt => opt.MapFrom<SearchRequestEntity_AgentFaxResolver>())
                //.ForMember(dest => dest.ApplicantAddressLine1, opt => opt.MapFrom<SearchRequestEntity_ApplicantAddress1Resolver>())
                //.ForMember(dest => dest.ApplicantAddressLine2, opt => opt.MapFrom<SearchRequestEntity_ApplicantAddress2Resolver>())
                //.ForMember(dest => dest.ApplicantCity, opt => opt.MapFrom<SearchRequestEntity_ApplicantCityResolver>())
                //.ForMember(dest => dest.ApplicantFirstName, opt => opt.MapFrom<SearchRequestEntity_ApplicantFirstNameResolver>())
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
                 .IncludeBase<PersonalInfo, DynamicsEntity>();

            CreateMap<Employment, EmploymentEntity>()
              .ForMember(dest => dest.AddressLine1, opt => opt.MapFrom(src => (src.Employer == null) ? string.Empty : (src.Employer.Address == null) ? string.Empty : src.Employer.Address.AddressLine1))
              .ForMember(dest => dest.AddressLine2, opt => opt.MapFrom(src => (src.Employer == null) ? string.Empty : (src.Employer.Address == null) ? string.Empty : src.Employer.Address.AddressLine2))
              .ForMember(dest => dest.AddressLine3, opt => opt.MapFrom(src => (src.Employer == null) ? string.Empty : (src.Employer.Address == null) ? string.Empty : src.Employer.Address.AddressLine3))
              .ForMember(dest => dest.BusinessName, opt => opt.MapFrom(src => (src.Employer == null) ? string.Empty : src.Employer.Name))
              .ForMember(dest => dest.Occupation, opt => opt.MapFrom(src => src.Occupation))
              .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
              .ForMember(dest => dest.Website, opt => opt.MapFrom(src => src.Website))
              .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => (src.Employer == null) ? string.Empty : (src.Employer.Address == null) ? string.Empty : src.Employer.Address.ZipPostalCode))
              .ForMember(dest => dest.City, opt => opt.MapFrom(src => (src.Employer == null) ? string.Empty : (src.Employer.Address == null) ? string.Empty : src.Employer.Address.City))
              .ForMember(dest => dest.IncomeAssistanceStatus, opt => opt.MapFrom(src => src.IncomeAssistanceStatus))
              .ForMember(dest => dest.IncomeAssistanceStatusOption, opt => opt.ConvertUsing(new IncomeAssistanceStatusConvertor(), src => src.IncomeAssistanceStatus))
              .ForMember(dest => dest.EmploymentType, opt => opt.ConvertUsing(new IncomeAssistanceConvertor(), src => src.IncomeAssistance))
              .ForMember(dest => dest.CountrySubdivisionText, opt => opt.MapFrom(src => (src.Employer == null) ? string.Empty : (src.Employer.Address == null) ? string.Empty : src.Employer.Address.StateProvince))
              .ForMember(dest => dest.CountryText, opt => opt.MapFrom(src => (src.Employer == null) ? string.Empty : (src.Employer.Address == null) ? string.Empty : src.Employer.Address.CountryRegion))
              .ForMember(dest => dest.BusinessOwner, opt => opt.MapFrom(src => (src.Employer == null) ? string.Empty : src.Employer.OwnerName))
              .ForMember(dest => dest.EmploymentConfirmed, opt => opt.MapFrom(src => src.EmploymentConfirmed))
              .ForMember(dest => dest.ContactPerson, opt => opt.MapFrom(src => (src.Employer == null) ? string.Empty : src.Employer.ContactPerson))
              .IncludeBase<PersonalInfo, DynamicsEntity>();

            CreateMap<Phone, EmploymentContactEntity>()
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Type.Equals("Phone", StringComparison.InvariantCultureIgnoreCase) ? src.PhoneNumber : null))
                .ForMember(dest => dest.PhoneExtension, opt => opt.MapFrom(src => src.Extension))
                .ForMember(dest => dest.FaxNumber, opt => opt.MapFrom(src => src.Type.Equals("fax", StringComparison.InvariantCultureIgnoreCase) ? src.PhoneNumber : null))
                .ForMember(dest => dest.SupplierTypeCode, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.PhoneType, opt => opt.ConvertUsing(new PhoneTypeConverter(), src => src.Type))
                .ForMember(dest => dest.StateCode, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.StatusCode, opt => opt.MapFrom(src => 1));

            CreateMap<Phone, PhoneNumberEntity>()
                .ForMember(dest => dest.TelePhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                 .ForMember(dest => dest.PhoneExtension, opt => opt.MapFrom(src => src.Extension))
                 .ForMember(dest => dest.SupplierTypeCode, opt => opt.MapFrom(src => src.Type))
                 .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                 .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                 .ForMember(dest => dest.TelephoneNumberType, opt => opt.ConvertUsing(new PhoneTypeConverter(), src => src.Type))
               .IncludeBase<PersonalInfo, DynamicsEntity>();

            CreateMap<Name, AliasEntity>()
                 .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                 .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                 .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.MiddleName))
                 .ForMember(dest => dest.ThirdGivenName, opt => opt.MapFrom(src => src.OtherName))
                 .ForMember(dest => dest.Type, opt => opt.ConvertUsing(new NameCategoryConverter(), src => src.Type))
                 .ForMember(dest => dest.SupplierTypeCode, opt => opt.MapFrom(src => src.Type))
                 .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Description))
                 .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                 .IncludeBase<PersonalInfo, DynamicsEntity>();

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
                 .IncludeBase<PersonalInfo, DynamicsEntity>();

            CreateMap<SSG_SearchapiRequestDataProvider, DataProvider>()
                 .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.AdaptorName.Replace(" ", String.Empty).ToUpperInvariant()));

            CreateMap<Person, PersonEntity>()
               .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
               .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
               .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.MiddleName))
               .ForMember(dest => dest.ThirdGivenName, opt => opt.MapFrom(src => src.OtherName))
               .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => (src.DateOfBirth != null) ? src.DateOfBirth.Value.DateTime : (DateTime?)null))
               .ForMember(dest => dest.DateOfDeath, opt => opt.MapFrom(src => (src.DateOfDeath != null) ? src.DateOfDeath.Value.DateTime : (DateTime?)null))
               .ForMember(dest => dest.DateOfDeathConfirmed, opt => opt.MapFrom(src => src.DateDeathConfirmed))
               .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
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
                 .IncludeBase<PersonalInfo, DynamicsEntity>();

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

            CreateMap<Vehicle, VehicleEntity>()
                 .ForMember(dest => dest.OwnershipType, opt => opt.MapFrom(src => src.OwnershipType))
                 .ForMember(dest => dest.PlateNumber, opt => opt.MapFrom(src => src.PlateNumber))
                 .ForMember(dest => dest.Discription, opt => opt.MapFrom(src => src.Description))
                 .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                 .ForMember(dest => dest.Vin, opt => opt.MapFrom(src => src.Vin))
                 .IncludeBase<PersonalInfo, DynamicsEntity>();

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
                  .IncludeBase<PersonalInfo, DynamicsEntity>();

            CreateMap<CompensationClaim, CompensationClaimEntity>()
                  .ForMember(dest => dest.ClaimType, opt => opt.MapFrom(src => src.ClaimType))
                  .ForMember(dest => dest.ClaimantNumber, opt => opt.MapFrom(src => src.ClaimantNumber))
                  .ForMember(dest => dest.ClaimNumber, opt => opt.MapFrom(src => src.ClaimNumber))
                  .ForMember(dest => dest.ClaimStatus, opt => opt.MapFrom(src => src.ClaimStatus))
                  .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                  .IncludeBase<PersonalInfo, DynamicsEntity>();

            CreateMap<InsuranceClaim, ICBCClaimEntity>()
                  .ForMember(dest => dest.ClaimType, opt => opt.MapFrom(src => src.ClaimType))
                  .ForMember(dest => dest.ClaimNumber, opt => opt.MapFrom(src => src.ClaimNumber))
                  .ForMember(dest => dest.ClaimStatus, opt => opt.MapFrom(src => src.ClaimStatus))
                  .ForMember(dest => dest.AdjusterFirstName, opt => opt.MapFrom(src => src.Adjustor == null ? null : src.Adjustor.FirstName))
                  .ForMember(dest => dest.AdjusterLastName, opt => opt.MapFrom(src => src.Adjustor == null ? null : src.Adjustor.LastName))
                  .ForMember(dest => dest.AdjusterMiddleName, opt => opt.MapFrom(src => src.Adjustor == null ? null : src.Adjustor.MiddleName))
                  .ForMember(dest => dest.AdjusterOtherName, opt => opt.MapFrom(src => src.Adjustor == null ? null : src.Adjustor.OtherName))
                  .ForMember(dest => dest.AdjusterPhoneNumber, opt => opt.MapFrom(src => src.AdjustorPhone==null?null:src.AdjustorPhone.PhoneNumber))
                  .ForMember(dest => dest.AdjusterPhoneNumberExt, opt => opt.MapFrom(src => src.AdjustorPhone == null ? null : src.AdjustorPhone.Extension))
                  .ForMember(dest => dest.PHNNumber, opt=>opt.MapFrom(src=>src.Identifiers==null?null:src.Identifiers.FirstOrDefault<PersonalIdentifier>(m=>m.Type==PersonalIdentifierType.PersonalHealthNumber).Value))
                  .ForMember(dest => dest.BCDLNumber, opt => opt.MapFrom(src => src.Identifiers == null ? null : src.Identifiers.FirstOrDefault<PersonalIdentifier>(m => m.Type == PersonalIdentifierType.BCDriverLicense).Value))
                  .ForMember(dest => dest.BCDLStatus, opt => opt.MapFrom(src => src.Identifiers == null ? null : src.Identifiers.FirstOrDefault<PersonalIdentifier>(m => m.Type == PersonalIdentifierType.BCDriverLicense).Description))
                  .ForMember(dest => dest.BCDLExpiryDate, opt => opt.MapFrom(src => src.Identifiers == null ? null : src.Identifiers.FirstOrDefault<PersonalIdentifier>(m=>m.Type== PersonalIdentifierType.BCDriverLicense).ReferenceDates.ElementAt(0).Value.ToString()))
                  .ForMember(dest => dest.ClaimCenterLocationCode, opt => opt.MapFrom(src => src.ClaimCentre==null? null:src.ClaimCentre.Location))
                  .ForMember(dest => dest.Description, opt=>opt.MapFrom(src=>src.Description))
                  .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                  .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.ClaimCentre == null ? null : src.ClaimCentre.ContactAddress==null?null: src.ClaimCentre.ContactAddress.ZipPostalCode))
                  .ForMember(dest => dest.AddressLine1, opt => opt.MapFrom(src => src.ClaimCentre == null ? null : src.ClaimCentre.ContactAddress == null ? null : src.ClaimCentre.ContactAddress.AddressLine1))
                  .ForMember(dest => dest.AddressLine2, opt => opt.MapFrom(src => src.ClaimCentre == null ? null : src.ClaimCentre.ContactAddress == null ? null : src.ClaimCentre.ContactAddress.AddressLine2))
                  .ForMember(dest => dest.AddressLine3, opt => opt.MapFrom(src => src.ClaimCentre == null ? null : src.ClaimCentre.ContactAddress == null ? null : src.ClaimCentre.ContactAddress.AddressLine3))
                  .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.ClaimCentre == null ? null : src.ClaimCentre.ContactAddress == null ? null : src.ClaimCentre.ContactAddress.City))
                  .ForMember(dest => dest.SupplierCountryCode, opt => opt.MapFrom(src => src.ClaimCentre == null ? null : src.ClaimCentre.ContactAddress == null ? null : src.ClaimCentre.ContactAddress.CountryRegion))
                  .ForMember(dest => dest.SupplierCountrySubdivisionCode, opt => opt.MapFrom(src => src.ClaimCentre == null ? null : src.ClaimCentre.ContactAddress == null ? null : src.ClaimCentre.ContactAddress.StateProvince))
                  .IncludeBase<PersonalInfo, DynamicsEntity>();

            CreateMap<Phone, SimplePhoneNumberEntity>()
                  .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                  .ForMember(dest => dest.Extension, opt => opt.MapFrom(src => src.Extension))
                  .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type));

            CreateMap<InvolvedParty, InvolvedPartyEntity>()
                  .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Name==null? null: src.Name.FirstName))
                  .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Name == null ? null : src.Name.LastName))
                  .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.Name == null ? null : src.Name.MiddleName))
                  .ForMember(dest => dest.OtherName, opt => opt.MapFrom(src => src.Name == null ? null : src.Name.OtherName))
                  .ForMember(dest => dest.OrganizationName, opt => opt.MapFrom(src => src.Organization))
                  .ForMember(dest => dest.PartyTypeCode, opt => opt.MapFrom(src => src.Type))
                  .ForMember(dest => dest.PartyDescription, opt => opt.MapFrom(src => src.TypeDescription))
                  .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes));

        }
    }

}
