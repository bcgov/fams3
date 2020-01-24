using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.PersonSearch.Models
{
    public class PersonSearchFailed : BcGov.Fams3.SearchApi.Contracts.PersonSearch.PersonSearchFailed
    {
        public string Cause { get; set; }

        public Guid SearchRequestId { get; set; }

        public DateTime TimeStamp { get; set; }
        public ProviderProfile ProviderProfile { get; set; }

        BcGov.Fams3.SearchApi.Contracts.PersonSearch.ProviderProfile AdapterEvent.ProviderProfile => ProviderProfile;
       
    }
}
