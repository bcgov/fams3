using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.SearchAgency.Models
{
    public class SearchRequestNotification : SearchRequestEvent
    {
        public NotificationType NotificationType { get; set; }

        public int? QueuePosition { get; set; }

        public DateTime? EstimatedCompletion { get; set; }

        public string Message { get; set; }

        public Person Person { get; set; }

        public string FSOName { get; set; }
    }
    public enum NotificationType
    {
        RequestSaved,
        RequestAssignedToFSO,
        ResponseReady
    }
}
