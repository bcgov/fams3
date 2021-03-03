using System;
using System.Collections.Generic;
using System.Text;

namespace BcGov.Fams3.SearchApi.Contracts.SearchRequest
{
    public interface NotificationAcknowledged : SearchRequestEvent
    {
        NotificationType NotificationType { get; set; }
        NotificationStatusEnum Status { get; set; }
    }

    public enum NotificationStatusEnum
    {
        SUCCESS,
        FAIL
    }
}
