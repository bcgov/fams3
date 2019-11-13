using System;

namespace SearchApi.Core.Adapters.Contracts
{
    /// <summary>
    /// Represents a match in a provider data set.
    /// </summary>
    public interface MatchFound
    {
        Guid searchRequestId { get; }
        string FirstName { get; }
        string LastName { get; }
        DateTime DateOfBirth { get; }
    }
}