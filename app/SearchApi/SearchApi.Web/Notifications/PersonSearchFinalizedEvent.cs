using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchApi.Web.Notifications
{
    public class PersonSearchFinalizedEvent : PersonSearchFinalized
    {
        public string Message { get; set; }

        public Guid SearchRequestId { get; set; }

        public string FileId { get; set; }

        public DateTime TimeStamp { get; set; }

    }
}
