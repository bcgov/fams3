using System;
using System.Collections.Generic;
using System.Text;

namespace BcGov.Fams3.SearchApi.Contracts.SearchRequest
{
   public interface SearchRequestNotification : SearchRequestEvent
    {
        NotificationType Notification { get; }
    }

    public enum NotificationType
    {
        RequestSubmitted,
        RequestAssignedToFSO,
        RequestCancelled,
        RequestCompleted

    }
}
