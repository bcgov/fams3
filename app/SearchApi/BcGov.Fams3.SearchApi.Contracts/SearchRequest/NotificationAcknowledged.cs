using System;
using System.Collections.Generic;
using System.Text;

namespace BcGov.Fams3.SearchApi.Contracts.SearchRequest
{
    public interface NotificationAcknowledged
    {
        string RequestorReference { get; set; }
        NotificationType NotificationType { get; set; }
        string FileId { get; set; }
        NotificationStatusEnum Status { get; set; }
        string AgencyCode { get; set; }
        DateTime TimeStamp { get; }
    }

    public enum NotificationStatusEnum
    {
        SUCCESS,
        FAIL
    }
}
