using System;

namespace DynamicsAdapter.Web.SearchAgency.Models
{
    public class SearchRequestSubmitted : SearchRequestEvent
    {
        public string Message { get; set; }
        public int QueuePosition { get; set; }
        public DateTime EstimatedCompletion { get; set; }
    }
}
