using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.PersonSearch.Models
{
   

    public class ProviderSearchEventStatus
    {
        public Guid SearchRequestId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string ProviderName { get; set; }
        public string Message { get; set; }
        public string EventType { get; set; }
    }

}
