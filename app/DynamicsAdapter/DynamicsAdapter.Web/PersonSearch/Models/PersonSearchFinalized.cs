using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.PersonSearch.Models
{
    public class PersonSearchFinalized 
    {
        public Guid SearchRequestId { get; set; }
        public string FileId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Message { get; set; }
    }
}
