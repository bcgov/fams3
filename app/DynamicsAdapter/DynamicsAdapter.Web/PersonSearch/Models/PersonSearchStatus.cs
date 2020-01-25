using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.PersonSearch.Models
{
    public abstract class PersonSearchStatus : BcGov.Fams3.SearchApi.Contracts.PersonSearch.PersonSearchEvent
    {
        public Guid SearchRequestId { get; set; }
        public DateTime TimeStamp { get; set; }
        public ProviderProfile ProviderProfile { get; set; }

    }
}
