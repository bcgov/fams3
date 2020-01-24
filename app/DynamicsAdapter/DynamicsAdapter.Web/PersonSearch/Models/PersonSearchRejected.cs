using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.PersonSearch.Models
{
    public class PersonSearchRejected : BcGov.Fams3.SearchApi.Contracts.PersonSearch.PersonSearchRejected
    {
        public IEnumerable<BcGov.Fams3.SearchApi.Contracts.PersonSearch.ValidationResult> Reasons { get; set; }

        public Guid SearchRequestId { get; set; }

        public DateTime TimeStamp { get; set; }

        public BcGov.Fams3.SearchApi.Contracts.PersonSearch.ProviderProfile ProviderProfile { get; set; }
    }

    public class ValidationResult : BcGov.Fams3.SearchApi.Contracts.PersonSearch.ValidationResult
    {
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }
    }
}
