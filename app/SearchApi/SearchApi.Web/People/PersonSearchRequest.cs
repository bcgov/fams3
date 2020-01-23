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
            IEnumerable<Phone> phones,
            IEnumerable<Name> names)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.DateOfBirth = dateOfBirth;
            this.Identifiers = identifiers;
            this.Phones = phones;
            this.Names = names;
        }
    }

}