using System;
using System.Collections.Generic;
using SearchApi.Core.Person.Contracts;
using SearchApi.Core.Person.Enums;

namespace SearchAdapter.Sample.SearchRequest.MatchFound
{
    public class PersonBuilder
    {
        private readonly IcbcPerson _icbcPerson = new IcbcPerson();

        private List<PersonalIdentifier> personalIdentifiers = new List<PersonalIdentifier>();
        public PersonBuilder WithFirstName(string firstName)
        {
            _icbcPerson.FirstName = firstName;
            return this;
        }

        public PersonBuilder WithLastName(string lastName)
        {
            _icbcPerson.LastName = lastName;
            return this;
        }

        public PersonBuilder WithDateOfBirth(DateTime? dateOfBirth)
        {
            if (dateOfBirth == null) return this;
            _icbcPerson.DateOfBirth = (DateTime)dateOfBirth;
            return this;
        }

        public PersonBuilder AddIdentifier(PersonalIdentifier personalIdentifier)
        {
            personalIdentifiers.Add(personalIdentifier);
            return this;
        }

        public IcbcPerson Build()
        {
            _icbcPerson.Identifiers = personalIdentifiers;
            return _icbcPerson;
        }
    }


    public sealed class IcbcPerson : Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public IEnumerable<PersonalIdentifier> Identifiers { get; set; }
    }


    public sealed class ICBCIdentifier : PersonalIdentifier
    {
        public string SerialNumber { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public PersonalIdentifierType Type { get; set; }
        public string IssuedBy { get; set; }
    }
}
