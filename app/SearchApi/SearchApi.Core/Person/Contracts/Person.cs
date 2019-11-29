using System;

namespace SearchApi.Core.Person.Contracts
{
    public interface Person
    {
        string FirstName { get; }
        string LastName { get; }
        DateTime? DateOfBirth { get; }
    }
}