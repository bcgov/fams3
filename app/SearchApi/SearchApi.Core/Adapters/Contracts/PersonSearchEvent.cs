using System;

namespace SearchApi.Core.Adapters.Contracts
{
    public interface PersonSearchEvent
    {
        Guid SearchRequestId { get; }

        ProviderProfile ProviderProfile { get; }
    }
}