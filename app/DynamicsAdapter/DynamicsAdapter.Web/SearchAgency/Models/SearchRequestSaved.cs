using System;

namespace DynamicsAdapter.Web.SearchAgency.Models
{
    public class SearchRequestSaved : SearchRequestEvent
    {
        public string Message { get; set; }
        public int? QueuePosition { get; set; }
        public DateTime? EstimatedCompletion { get; set; }
    }
}
