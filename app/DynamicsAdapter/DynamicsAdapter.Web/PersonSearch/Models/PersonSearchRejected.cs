using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.PersonSearch.Models
{
    public class PersonSearchRejected : BcGov.Fams3.SearchApi.Contracts.PersonSearch.PersonSearchRejected
    {
        public IEnumerable<ValidationResult> Reasons { get; set; }
        public Guid SearchRequestId { get; set; }

        public DateTime TimeStamp { get; set; }

        public ProviderProfile ProviderProfile { get; set; }

        BcGov.Fams3.SearchApi.Contracts.PersonSearch.ProviderProfile AdapterEvent.ProviderProfile => ProviderProfile;
        IEnumerable<BcGov.Fams3.SearchApi.Contracts.PersonSearch.ValidationResult> BcGov.Fams3.SearchApi.Contracts.PersonSearch.PersonSearchRejected.Reasons => Reasons;
    }

    public class ValidationResult : BcGov.Fams3.SearchApi.Contracts.PersonSearch.ValidationResult
    {
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }
    }
}
