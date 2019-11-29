using System;
using System.Collections.Generic;

namespace SearchApi.Core.Person.Contracts
{
    public interface ExecuteSearch
    {
        string FirstName { get; }
        string LastName { get; }
        DateTime? DateOfBirth { get; }

        IEnumerable<PersonalIdentifier> Identifiers { get; }
    }
}