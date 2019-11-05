using System;

namespace SearchApi.Core.Adapters.Contracts
{
    public interface MatchFound
    {
        string FirstName { get; }
        string LastName { get; }
        DateTime DateOfBirth { get; }
    }
}