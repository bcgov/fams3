using System;

namespace SearchApi.Core.Adapters.Contracts
{
    public interface PersonSearchEvent
    {
        Guid SearchRequestId { get; }
        DateTime TimeStamp { get; }
        ProviderProfile ProviderProfile { get; }
    }
}