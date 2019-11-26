using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SearchApi.Core.Adapters.Contracts;
using SearchApi.Core.Adapters.Models;

namespace SearchAdapter.ICBC.SearchRequest
{
    public class PersonSearchRejectedEvent : PersonSearchRejected
    {

        private readonly List<ValidationResult> _validationResults = new List<ValidationResult>();

        public PersonSearchRejectedEvent(Guid searchRequestId, ProviderProfile providerProfile)
        {
            TimeStamp = DateTime.Now;
            SearchRequestId = searchRequestId;
            ProviderProfile = providerProfile;
        }

        public void AddValidationResult(ValidationResult validationResult)
        {
            _validationResults.Add(validationResult);
        }

        public Guid SearchRequestId { get; }
        public DateTime TimeStamp { get; }
        public ProviderProfile ProviderProfile { get; }
        public IEnumerable<ValidationResult> Reasons => _validationResults;
    }
}
