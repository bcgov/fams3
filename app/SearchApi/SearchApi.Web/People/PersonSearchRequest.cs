using System;
using System.Collections.Generic;
using System.ComponentModel;
using BcGov.Fams3.SearchApi.Contracts.Person;
using Newtonsoft.Json;

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
            IEnumerable<PersonalIdentifier> identifiers,
            IEnumerable<Address> addresses,
            IEnumerable<SearchApiPhoneNumber> phoneNumbers,
            IEnumerable<SearchApiName> names )
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.DateOfBirth = dateOfBirth;
            this.Identifiers = identifiers;
            this.PhoneNumbers = phoneNumbers;
            this.Names = names;
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
        [Description("A collection of names")]
        public IEnumerable<Name> Names { get; set; }
    }

     public class SearchApiPhoneNumber : PhoneNumber
    {
        public string SuppliedBy { get; }
        public DateTime? Date { get; }
        public string DateType { get; }
        public string PhoneNumber { get; }
        public string PhoneNumberType { get; }
    }

    public class SearchApiName : Name
    {
        public string FirstName { get; }

        public string LastName { get; }

        public string MiddleName { get; }

        public string Type { get; }

        public DateTime? EffectiveDate { get; }

        public DateTime? EndDate { get; }

        public string Description { get; }
    }
}