using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using BcGov.Fams3.SearchApi.Contracts.SearchRequest;
using System;

namespace SearchRequest.Adaptor.Test
{
    public class FakeSearchRequestEvent : SearchRequestEvent
    {
        public string RequestId { get; set; }

        public string SearchRequestKey { get; set; }

        public Guid SearchRequestId { get; set; }

        public DateTime TimeStamp { get; set; }

        public ProviderProfile ProviderProfile { get; set; }

        public RequestAction Action { get; set; }
        public int QueuePosition { get; set; }
        public DateTime EstimatedCompletion { get; set; }
    }

    public class FakeSearchRequestOrdered : SearchRequestOrdered
    {
        public string RequestId { get; set; }

        public string SearchRequestKey { get; set; }

        public Guid SearchRequestId { get; set; }

        public DateTime TimeStamp { get; set; }

        public ProviderProfile ProviderProfile { get; set; }

        public RequestAction Action { get; set; }

        public Person Person { get; set; }
        public int QueuePosition { get; set; }
        public DateTime EstimatedCompletion { get; set; }
    }

    public class FakeNotificationAcknowledged : NotificationAcknowledged
    {
        public string RequestId { get; set; }

        public string SearchRequestKey { get; set; }

        public Guid SearchRequestId { get; set; }

        public DateTime TimeStamp { get; set; }

        public ProviderProfile ProviderProfile { get; set; }

        public RequestAction Action { get; set; }
        public NotificationType NotificationType { get; set; }
        public NotificationStatusEnum Status { get; set; }
    }
}
