using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using NSwag;
using SearchApi.Core.Adapters.Models;
using SearchApi.Core.Person.Contracts;
using SearchApi.Core.Person.Enums;

namespace SearchApi.Web.Controllers
{
    /// <summary>
    /// The PersonSearchRequest represents the information known about a subject before executing a search.
    /// </summary>
    [Description("Represents a set of information to execute a search on a person")]
    public class PersonSearchRequest : Person
    {

        [JsonConstructor]
        public PersonSearchRequest(string firstName, string lastName, DateTime? dateOfBirth, IEnumerable<SearchApiPersonalIdentifier> identifiers)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.DateOfBirth = dateOfBirth;
            this.Identifiers = identifiers;
        }

        [Description("The first name of the subject.")]
        public string FirstName { get; }
        [Description("The last name of the subject.")]
        public string LastName { get; }
        [Description("The date of birth of the subject.")]
        public DateTime? DateOfBirth { get; }

        public IEnumerable<PersonalIdentifier> Identifiers { get; set; }
        public IEnumerable<PersonalAddress> Addresses { get; set; }

        public IEnumerable<PersonalPhoneNumber> PhoneNumbers { get; set; }


    }


    public class SearchApiPersonalIdentifier : PersonalIdentifier
    {
        [Description("The serial number of the identifier.")]
        public string SerialNumber { get; set; }
        [Description("The effective date of the identifier.")]
        public DateTime? EffectiveDate { get; set; }
        [Description("The expiration date of the identifier.")]
        public DateTime? ExpirationDate { get; set; }
        [Description("The type of the identifier.")]
        public PersonalIdentifierType Type { get; set; }
        [Description("The issuer of the identifier.")]
        public string IssuedBy { get; set; }
    }
}