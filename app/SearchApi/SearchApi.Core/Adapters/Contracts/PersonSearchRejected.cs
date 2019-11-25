using System;
using System.Collections.Generic;
using SearchApi.Core.Adapters.Models;

namespace SearchApi.Core.Adapters.Contracts
{
    public interface PersonSearchRejected
    {
        Guid RequestId { get; }
        ProviderProfile ProviderProfile { get; }
        IEnumerable<ValidationResult> Reasons { get; }
    }
}