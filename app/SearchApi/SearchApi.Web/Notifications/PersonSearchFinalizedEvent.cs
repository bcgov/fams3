using BcGov.Fams3.SearchApi.Contracts.Person;
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

        public string SearchRequestKey { get; set; }

        public DateTime TimeStamp { get; set; }

        public ProviderProfile ProviderProfile { get; set; }

        public SearchStatus Status { get; set; }
    }

    public class ProviderProfileDetails : ProviderProfile
    {
        public string Name {get;set;}
        public SearchSpeedType SearchSpeedType { get; set; } = SearchSpeedType.Fast;
    }
}
