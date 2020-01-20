using System;
using System.Collections.Generic;
using BcGov.Fams3.SearchApi.Contracts.Person;

namespace SearchApi.Core.Test.Fake
{
    public class FakePerson : Person
    {
        public string FirstName { get; } = nameof(FirstName);
        public string LastName { get; } = nameof(LastName);
        public string SecondName { get; } = nameof(SecondName);
        public string ThirdName { get; } = nameof(ThirdName);
        public string HairColour { get; } = nameof(HairColour);
        public string EyeColour { get; } = nameof(EyeColour);
        public decimal Height { get; } = 1.2m;
        public decimal Weight { get; } = 2.3m;
        public DateTime? DateOfBirth { get; } = new DateTime(2001, 1, 1);
        public IEnumerable<PersonalIdentifier> Identifiers { get; set; }
        public IEnumerable<Address> Addresses { get; set; }
        public IEnumerable<PhoneNumber> PhoneNumbers { get; set; }
        public IEnumerable<Name> Names { get; set; }
    }
}
