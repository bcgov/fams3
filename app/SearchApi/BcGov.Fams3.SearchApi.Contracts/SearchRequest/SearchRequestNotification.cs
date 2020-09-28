using System;
using System.Collections.Generic;
using System.Text;

namespace BcGov.Fams3.SearchApi.Contracts.SearchRequest
{
    public interface SearchRequestNotification : SearchRequestEvent
    {
        NotificationType NotificationType { get; }

        public int? QueuePosition { get; set; }

        public DateTime? EstimatedCompletion { get; set; }

        public string Message { get; set; }

        public Person.Person Person { get; set; }
    }

    public enum NotificationType
    {
        RequestSaved,
        RequestAssignedToFSO,
        RequestClosed
    }
}
