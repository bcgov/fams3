using System;
using System.Collections.Generic;
using System.ComponentModel;
using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using Newtonsoft.Json;

namespace SearchApi.Web.Controllers
{
    /// <summary>
    /// The PersonSearchRequest represents the information known about a subject before executing a search.
    /// </summary>
    [Description("Represents a set of information to execute a search on a person")]
    public class PersonSearchRequest : Person
    {

        public IEnumerable<DataProvider> DataProviders { get; set; }
        /// <summary>
        /// This is the key to identify this search request, it is composed of fileId_SequenceNumber
        /// </summary>
        public string SearchRequestKey { get; set; }

        [JsonConstructor]
        public PersonSearchRequest(
            string firstName,
            string lastName,
            DateTime? dateOfBirth, 
            IEnumerable<PersonalIdentifier> identifiers,
            IEnumerable<Address> addresses,
            IEnumerable<Phone> phones,
            IEnumerable<Name> names, 
            IEnumerable<RelatedPerson> relatedPersons,
            IEnumerable<Employment> employments,
            IEnumerable<DataProvider> dataProviders,
            string searchRequestKey)
        {
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Identifiers = identifiers;
            Phones = phones;
            this.Names = names;
            this.Addresses = addresses;
            this.Employments = employments;         
            this.RelatedPersons = relatedPersons;
            this.DataProviders = dataProviders;
            this.SearchRequestKey = searchRequestKey;
        }

    }

    public class DataProvider : ProviderProfile
    {

        public DataProvider()
        {
            Completed = false;
        }
        public string Name { get; set; }
        public bool Completed { get; set; }

        public int NumberOfRetries { get; set; }

        public int TimeBetweenRetries { get; set; }
    }
}