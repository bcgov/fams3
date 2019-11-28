using System;
using System.Collections.Generic;
using SearchApi.Core.Contracts;

namespace SearchApi.Core.Adapters.Contracts
{
    /// <summary>
    /// Represents a match in a provider data set.
    /// </summary>
    public interface PersonFound
    {
        Guid SearchRequestId { get; }
        
        Person Person { get; }

        IEnumerable<PersonId> PersonIds { get; }

    }
}