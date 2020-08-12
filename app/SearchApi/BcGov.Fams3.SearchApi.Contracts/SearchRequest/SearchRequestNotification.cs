using System;
using System.Collections.Generic;
using System.Text;

namespace BcGov.Fams3.SearchApi.Contracts.SearchRequest
{
   public interface SearchRequestNotification : SearchRequestEvent
    {
        NotificationType Notification { get; }

        public int? QueuePosition { get; set; }

        public DateTime? EstimatedCompletion { get; set; }

        public string Message { get; set; }
    }

    public enum NotificationType
    {
        RequestSaved,
        RequestAssignedToFSO

    }
}
