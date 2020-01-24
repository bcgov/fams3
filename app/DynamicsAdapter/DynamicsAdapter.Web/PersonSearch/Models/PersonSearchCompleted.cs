using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.PersonSearch.Models
{
    public class PersonSearchCompleted : BcGov.Fams3.SearchApi.Contracts.PersonSearch.PersonSearchCompleted
    {      
        public BcGov.Fams3.SearchApi.Contracts.Person.Person MatchedPerson { get;  set; }

        public Guid SearchRequestId { get; set; }

        public DateTime TimeStamp { get; set; }

        public BcGov.Fams3.SearchApi.Contracts.PersonSearch.ProviderProfile ProviderProfile { get; set; }
    }
}
