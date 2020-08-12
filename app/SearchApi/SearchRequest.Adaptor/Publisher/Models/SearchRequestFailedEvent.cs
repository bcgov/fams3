using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using BcGov.Fams3.SearchApi.Contracts.SearchRequest;
using System;

namespace SearchRequestAdaptor.Publisher.Models
{
    public class SearchRequestFailedEvent : SearchRequestFailed
    {
        public SearchRequestFailedEvent(SearchRequestEvent baseEvent)
        {
            this.RequestId = baseEvent.RequestId;
            this.SearchRequestId = baseEvent.SearchRequestId;
            this.TimeStamp = DateTime.Now;
            this.SearchRequestKey = baseEvent.SearchRequestKey;
            this.Action = baseEvent.Action;
            this.ProviderProfile = baseEvent.ProviderProfile;
        }

        public string Cause { get; set; }

        public string RequestId { get; set; }

        public string SearchRequestKey { get; set; }

        public Guid SearchRequestId { get; set; }

        public DateTime TimeStamp { get; set; }

        public ProviderProfile ProviderProfile { get; set; }

        public RequestAction Action { get; set; }
        public int? QueuePosition { get; set; }
        public DateTime? EstimatedCompletion { get; set; }
    }
}
