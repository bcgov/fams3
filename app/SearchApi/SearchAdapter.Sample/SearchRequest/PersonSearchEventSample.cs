using System;
using System.Collections.Generic;
using BcGov.Fams3.SearchApi.Core.Adapters.Contracts;
using BcGov.Fams3.SearchApi.Core.Adapters.Models;
using BcGov.Fams3.SearchApi.Core.Person.Contracts;
using BcGov.Fams3.SearchApi.Core.Person.Enums;

namespace SearchAdapter.Sample.SearchRequest
{
    public class PersonSearchCompletedSample : PersonSearchCompleted
    {
        public Guid SearchRequestId { get; set; }
        public DateTime TimeStamp { get; set; }
        public ProviderProfile ProviderProfile { get; set; }
        public Person MatchedPerson { get; set; }
    }

    public class PersonSample : Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public IEnumerable<PersonalIdentifier> Identifiers { get; set; }
        public IEnumerable<Address> Addresses { get; set; }
        public IEnumerable<PhoneNumber> PhoneNumbers { get; set; }
    }

    public class PersonalIdentifierSample : PersonalIdentifier
    {
        public string SerialNumber { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public PersonalIdentifierType Type { get; set; }
        public string IssuedBy { get; set; }
    }

    public class AddressSample : Address
    {
        public string Type { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string NonCanadianState { get; set; }
        public string SuppliedBy { get; set; }
    }

    public class PhoneNumberSample : PhoneNumber
    {

        public string PhoneNumber { get; set; }

        public string PhoneNumberType { get; set; }

        public string SuppliedBy { get; set; }

        public DateTime? Date { get; set; }

        public string DateType { get; set; }
    }

    public class PersonSearchRejectedEvent : PersonSearchRejected
    {

        private readonly List<ValidationResult> _validationResults = new List<ValidationResult>();

        public PersonSearchRejectedEvent(Guid searchRequestId, ProviderProfile providerProfile)
        {
            TimeStamp = DateTime.Now;
            SearchRequestId = searchRequestId;
            ProviderProfile = providerProfile;
        }

        public void AddValidationResult(ValidationResult validationResult)
        {
            _validationResults.Add(validationResult);
        }

        public Guid SearchRequestId { get; }
        public DateTime TimeStamp { get; }
        public ProviderProfile ProviderProfile { get; }
        public IEnumerable<ValidationResult> Reasons => _validationResults;
    }

}