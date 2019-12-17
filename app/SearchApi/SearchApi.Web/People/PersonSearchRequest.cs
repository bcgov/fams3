using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using NSwag;
using BcGov.Fams3.SearchApi.Core.Adapters.Models;
using BcGov.Fams3.SearchApi.Core.Person.Contracts;
using BcGov.Fams3.SearchApi.Core.Person.Enums;

namespace SearchApi.Web.Controllers
{
    /// <summary>
    /// The PersonSearchRequest represents the information known about a subject before executing a search.
    /// </summary>
    [Description("Represents a set of information to execute a search on a person")]
    public class PersonSearchRequest : Person
    {

        [JsonConstructor]
        public PersonSearchRequest(
            string firstName, 
            string lastName, 
            DateTime? dateOfBirth, 
            IEnumerable<SearchApiPersonalIdentifier> identifiers,
            IEnumerable<SearchApiAddress> addresses,
            IEnumerable<SearchApiPhoneNumber> phoneNumbers)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.DateOfBirth = dateOfBirth;
            this.Identifiers = identifiers;
            this.PhoneNumbers = phoneNumbers;
        }

        [Description("The first name of the subject")]
        public string FirstName { get; }
        [Description("The last name of the subject")]
        public string LastName { get; }
        [Description("The date of birth of the subject")]
        public DateTime? DateOfBirth { get; }
        [Description("A collection of Personal Identifiers")]
        public IEnumerable<PersonalIdentifier> Identifiers { get; set; }
        [Description("A collection of addresses")]
        public IEnumerable<Address> Addresses { get; set; }
        [Description("A collection of phone numbers")]
        public IEnumerable<PhoneNumber> PhoneNumbers { get; set; }


    }


    public class SearchApiPersonalIdentifier : PersonalIdentifier
    {
        [Description("The serial number of the identifier")]
        public string SerialNumber { get; set; }
        [Description("The effective date of the identifier")]
        public DateTime? EffectiveDate { get; set; }
        [Description("The expiration date of the identifier")]
        public DateTime? ExpirationDate { get; set; }
        [Description("The type of the identifier")]
        public PersonalIdentifierType Type { get; set; }
        [Description("The issuer of the identifier")]
        public string IssuedBy { get; set; }
    }


    public class SearchApiAddress : Address
    {
        [Description("The type of address")]
        public string Type { get; }
        [Description("The Address Line 1")]
        public string AddressLine1 { get; }
        [Description("The Address Line 2")]
        public string AddressLine2 { get; }
        [Description("The Address Province or state")]
        public string Province { get; }
        [Description("The Address City")]
        public string City { get; }
        [Description("The Address Country")]
        public string Country { get; }
        [Description("The Address Zip or Postal Code")]
        public string PostalCode { get; }
        public string NonCanadianState { get; }
        public string SuppliedBy { get; }
    }


    public class SearchApiPhoneNumber : PhoneNumber
    {
        public string SuppliedBy { get; }
        [Description("A Date")]
        public DateTime? Date { get; }
        [Description("The Date type of the supplied Date")]
        public string DateType { get; }
        [Description("The Phone number")]
        public string PhoneNumber { get; }
        [Description("The phone number type")]
        public string PhoneNumberType { get; }
    }
}