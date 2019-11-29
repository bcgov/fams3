using System;

namespace SearchApi.Core.Test.Fake
{
    public class FakePerson : Person.Contracts.Person
    {
        public string FirstName { get; } = nameof(FirstName);
        public string LastName { get; } = nameof(LastName);
        public DateTime? DateOfBirth { get; } = new DateTime(2001, 1, 1);
    }
}
