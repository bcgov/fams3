using System;
using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using BcGov.Fams3.SearchApi.Contracts.SearchRequest;

namespace BcGov.Fams3.SearchApi.Core.Adapters.Models
{
    public class DefaultSearchRequestFailed : SearchRequestFailed
    {

        public DefaultSearchRequestFailed(Guid searchRequestId, string requestId, string searchRequestKey, string cause, RequestAction action)
        {
            RequestId = requestId;
            SearchRequestId = searchRequestId;
            Action = action;
            Cause = cause;
            TimeStamp = DateTime.Now;
            SearchRequestKey = searchRequestKey;
        }

        public Guid SearchRequestId { get; }
        public string SearchRequestKey { get;  }
        public DateTime TimeStamp { get; }
        public ProviderProfile ProviderProfile { get; }
        public string Cause { get; }

        public string RequestId { get; }
        public RequestAction Action { get; set ; }
    }
}