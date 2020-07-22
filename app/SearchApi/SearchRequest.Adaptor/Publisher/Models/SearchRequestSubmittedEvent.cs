using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using BcGov.Fams3.SearchApi.Contracts.SearchRequest;
using System;

namespace SearchRequestAdaptor.Publisher.Models
{
    public class SearchRequestSubmittedEvent : SearchRequestSubmitted
    {
        public SearchRequestSubmittedEvent(SearchRequestEvent baseEvent)
        {
            this.RequestId = baseEvent.RequestId;
            this.SearchRequestId = baseEvent.SearchRequestId;
            this.TimeStamp = DateTime.Now;
            this.SearchRequestKey = baseEvent.SearchRequestKey;
            this.Action = baseEvent.Action;
        }

        public string RequestId { get; set; }

        public string SearchRequestKey { get; set; }

        public Guid SearchRequestId { get; set; }

        public DateTime TimeStamp { get; set; }

        public ProviderProfile ProviderProfile { get; set; }

        public String Message { get; set; }

        public RequestAction Action { get; set; }
    }
}
