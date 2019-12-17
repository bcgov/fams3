using System;
using System.Collections.Generic;
using BcGov.Fams3.SearchApi.Core.Person.Contracts;

namespace SearchApi.Core.Test.Fake
{
    public class FakePerson : Person
    {
        public string FirstName { get; } = nameof(FirstName);
        public string LastName { get; } = nameof(LastName);
        public DateTime? DateOfBirth { get; } = new DateTime(2001, 1, 1);
        public IEnumerable<PersonalIdentifier> Identifiers { get; set; }
        public IEnumerable<Address> Addresses { get; set; }
        public IEnumerable<PhoneNumber> PhoneNumbers { get; set; }
    }
}
