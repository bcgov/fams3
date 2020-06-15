using System;

namespace DynamicsAdapter.Web.PersonSearch.Models
{
    public class PersonSearchFinalized
    {
        public Guid SearchRequestId { get; set; }
        public string SearchRequestKey{ get; set; }
        public DateTime TimeStamp { get; set; }
        public string Message { get; set; }
    }
}
