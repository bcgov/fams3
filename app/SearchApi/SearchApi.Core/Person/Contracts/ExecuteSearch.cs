using System;
using System.Collections.Generic;
using SearchApi.Core.Adapters.Models.Contracts;

namespace SearchApi.Core.Contracts
{
    public interface ExecuteSearch
    {
        string FirstName { get; }
        string LastName { get; }
        DateTime? DateOfBirth { get; }

        IEnumerable<PersonalIdentifier> Identifiers { get; }
    }
}