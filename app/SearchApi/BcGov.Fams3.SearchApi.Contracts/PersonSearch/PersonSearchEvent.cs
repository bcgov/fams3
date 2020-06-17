using System;

namespace BcGov.Fams3.SearchApi.Contracts.PersonSearch
{
    public interface PersonSearchEvent
    {
        /// <summary>
        /// This is GUID of SearchApiRequestId
        /// </summary>
        Guid SearchRequestId { get; }
        /// <summary>
        /// This is the Key to identify the searchRequest, it is made of FileId_SequenceNumber
        /// </summary>
        string SearchRequestKey { get; }
        DateTime TimeStamp { get; }
    }
}