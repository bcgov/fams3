using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.PersonSearch.Models
{
    public class PersonSearchAccepted : BcGov.Fams3.SearchApi.Contracts.PersonSearch.PersonSearchAccepted
    {
        public Guid SearchRequestId { get; set; }

        public DateTime TimeStamp { get; set; }

        public BcGov.Fams3.SearchApi.Contracts.PersonSearch.ProviderProfile ProviderProfile { get; set; }
    }
}
