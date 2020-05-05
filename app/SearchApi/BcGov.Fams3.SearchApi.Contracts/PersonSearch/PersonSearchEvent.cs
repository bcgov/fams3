using System;

namespace BcGov.Fams3.SearchApi.Contracts.PersonSearch
{
    public interface PersonSearchEvent
    {
        Guid SearchRequestId { get; }
        string FileId { get; }
        DateTime TimeStamp { get; }
    }
}