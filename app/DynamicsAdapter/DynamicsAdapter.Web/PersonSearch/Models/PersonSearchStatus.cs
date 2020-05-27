using System;

namespace DynamicsAdapter.Web.PersonSearch.Models
{
    public abstract class PersonSearchStatus
    {
        public Guid SearchRequestId { get; set; }
        public string FileId { get; set; }
        public DateTime TimeStamp { get; set; }
        public ProviderProfile ProviderProfile { get; set; }
    }
}
