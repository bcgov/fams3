using System;

namespace BcGov.Fams3.SearchApi.Contracts.PersonSearch
{
    public interface PersonSearchEvent
    {
        Guid SearchRequestId { get; }
        DateTime TimeStamp { get; }
    }
}