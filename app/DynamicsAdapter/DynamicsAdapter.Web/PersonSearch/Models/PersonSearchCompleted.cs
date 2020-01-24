using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.PersonSearch.Models
{
    [ContractClassFor(typeof(BcGov.Fams3.SearchApi.Contracts.PersonSearch.PersonSearchCompleted))]
    public class PersonSearchCompleted : BcGov.Fams3.SearchApi.Contracts.PersonSearch.PersonSearchCompleted
    {
        private ProviderProfile _providerProfile;

        //public PersonSearchCompleted(ProviderProfile pr)
        //{
        //    this._providerProfile = pr;
        //}
        public BcGov.Fams3.SearchApi.Contracts.Person.Person MatchedPerson { get;  set; }

        public Guid SearchRequestId { get; set; }

        public DateTime TimeStamp { get; set; }
        
        public BcGov.Fams3.SearchApi.Contracts.PersonSearch.ProviderProfile ProviderProfile
        {
            get { return _providerProfile; }
            set { _providerProfile = (ProviderProfile)value; }
        }

        //public ProviderProfile ProviderProfile
        //{
        //    get;set;
        //}

        //public void SetProviderProfile(BcGov.Fams3.SearchApi.Contracts.PersonSearch.ProviderProfile pf)
        //{
        //    _providerProfile = pf;
        //}
    }

}
