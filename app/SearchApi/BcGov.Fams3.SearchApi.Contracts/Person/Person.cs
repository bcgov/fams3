using System;
using System.Collections.Generic;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string OtherName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime? DateOfDeath { get; set; }

        public string Gender { get; set; }
        public bool? DateDeathConfirmed { get; set; }
        public string Incacerated { get; set; }

        public IEnumerable<PersonalIdentifier> Identifiers { get; set; }
        public IEnumerable<Address> Addresses { get; set; }
        public IEnumerable<Phone> Phones { get; set; }
        public IEnumerable<Name> Names { get; set; }
        public IEnumerable<RelatedPerson> RelatedPersons { get; set; }

        public IEnumerable<Employment> Employments { get; set; }

        public string Notes { get; set; }
    }
}