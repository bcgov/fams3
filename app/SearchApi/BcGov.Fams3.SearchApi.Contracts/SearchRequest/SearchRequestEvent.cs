using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using System;

namespace BcGov.Fams3.SearchApi.Contracts.SearchRequest
{

    public interface SearchRequestEvent : AdapterEvent

    {
        /// <summary>
        /// This is Data Partner Id for the SearchRequest
        /// </summary>
        string RequestId { get; }
        /// <summary>
        /// This is the Key to identify the searchRequest in dynamics it is made of FileId_SequenceNumber
        /// </summary>
        string SearchRequestKey { get; }
        /// <summary>
        /// This is Guid of the search request
        /// </summary>
        Guid SearchRequestId { get; }
        DateTime TimeStamp { get; }
        ////
        /// <summary>
        ///   action requested by agency, new, update, cancel, notify"
        /// </summary>
        public RequestAction Action { get; set; }

    }
}
