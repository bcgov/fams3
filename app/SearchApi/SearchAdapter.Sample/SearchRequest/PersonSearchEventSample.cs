using System;
using System.Collections.Generic;
using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using BcGov.Fams3.SearchApi.Core.Adapters.Models;

namespace SearchAdapter.Sample.SearchRequest
{
    public class PersonSearchCompletedSample : PersonSearchCompleted
    {
        public Guid SearchRequestId { get; set; }
        public DateTime TimeStamp { get; set; }
        public ProviderProfile ProviderProfile { get; set; }
        public PersonFound MatchedPerson { get; set; }
    }

    public class PersonSample : PersonFound
    {
        public string SecondName { get; set; }
        public string ThirdName { get; set; }
        public string HairColour { get; set; }
        public string EyeColour { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public IEnumerable<PersonalIdentifier> Identifiers { get; set; }
        public IEnumerable<Address> Addresses { get; set; }
        public IEnumerable<PhoneNumber> PhoneNumbers { get; set; }
        public IEnumerable<Name> Names { get; set; }
    }

    public class PersonalIdentifierSample : PersonalIdentifier
    {
        public string Value { get; set; }
        public PersonalIdentifierType Type { get; set; }
        public string TypeCode { get; set; }
        public string IssuedBy { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public IEnumerable<ReferenceDate> ReferenceDates { get; set; }
    }

    public class ReferenceDateSample : ReferenceDate
    {
        public int Index { get; set; }
        public string Key { get; set; }
        public DateTime Value { get; set; }
    }

    public class AddressSample : Address
    {
        public string TypeCode { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string StateProvince { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string CountryRegion { get; set; }
        public string ZipPostalCode { get; set; }
        public string SuppliedBy { get; set; }
        public IEnumerable<ReferenceDate> ReferenceDates { get; set; }
        public AddressType Type { get; set; }
        public string Description { get; set; }
    }

    public class PhoneNumberSample : PhoneNumber
    {

        public string PhoneNumber { get; set; }

        public PhoneTypeCode Type { get; set; }

        public string Extension { get; set; }

        public string SuppliedBy { get; set; }

        public IEnumerable<ReferenceDate> ReferenceDates { get; set; }

        public string Description { get; set; }

        public string TypeCode { get; set; }
    }

    public class NameSample : Name
    {
        public NameType Type { get; set; }

        public IEnumerable<ReferenceDate> ReferenceDates { get; set; }
        public string FirstName { get; set; }

        public string LastName {get; set; }

        public string SecondName {get; set; }
      
        public string ThirdName { get; set; }

        public string TypeCode { get; set; }
        public string Description { get; set; }
    }

    public class PersonSearchRejectedEvent : PersonSearchRejected
    {

        private readonly List<DefaultValidationResult> _validationResults = new List<DefaultValidationResult>();

        public PersonSearchRejectedEvent(Guid searchRequestId, ProviderProfile providerProfile)
        {
            TimeStamp = DateTime.Now;
            SearchRequestId = searchRequestId;
            ProviderProfile = providerProfile;
        }

        public void AddValidationResult(DefaultValidationResult validationResult)
        {
            _validationResults.Add(validationResult);
        }

        public Guid SearchRequestId { get; }
        public DateTime TimeStamp { get; }
        public ProviderProfile ProviderProfile { get; }
        public IEnumerable<ValidationResult> Reasons => _validationResults;
    }

}