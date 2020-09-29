using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.SearchRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchRequest.Adaptor.Publisher.Models
{
    public class SearchRequestNotificationEvent
    {

        public SearchRequestNotificationEvent(SearchRequestEvent baseEvent)
        {

            this.RequestId = baseEvent.RequestId;
            this.SearchRequestId = baseEvent.SearchRequestId;
            this.TimeStamp = DateTime.Now;
            this.SearchRequestKey = baseEvent.SearchRequestKey;

        }

        public SearchRequestNotificationEvent()
        {

        }
        public string RequestId { get; set; }

        public string SearchRequestKey { get; set; }

        public Guid SearchRequestId { get; set; }

        public DateTime TimeStamp { get; set; }

        public ProviderProfile ProviderProfile { get; set; }

        public int? QueuePosition { get; set; }
        public DateTime? EstimatedCompletion { get; set; }

        public NotificationType NotificationType { get; set; }

        public string Message { get; set; }

        public string FSOName { get; set; }

        public Person Person { get; set; }
    }
}
