using System;

namespace BcGov.Fams3.SearchApi.Core.Adapters.Contracts
{
    public interface PersonSearchEvent
    {
        Guid SearchRequestId { get; }
        DateTime TimeStamp { get; }
    }
}