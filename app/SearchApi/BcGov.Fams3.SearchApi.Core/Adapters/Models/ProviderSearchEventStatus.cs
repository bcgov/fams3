using System;

namespace BcGov.Fams3.SearchApi.Core.Adapters.Models
{

    /// <summary>
    /// Represents a search status from a provider which could be accepted, rejected, person not found, person found or failed
    /// </summary>
    public class ProviderSearchEventStatus
    {
        public Guid SearchRequestId { get; set; }
        public DateTime TimeStamp { get; set; } 
        public string ProviderName { get; set; }
        public string Message { get; set; }
        public string EventType { get; set; }
    }
}
