using System;
using System.Collections.Generic;
using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using BcGov.Fams3.SearchApi.Core.Adapters.Models;

namespace SearchAdapter.Sample.SearchRequest
{
    public class PersonSearchCompletedSample : PersonSearchCompleted
    {
        public Guid SearchRequestId { get; set; }
        public string FileId { get; set; }
        public DateTime TimeStamp { get; set; }
        public ProviderProfile ProviderProfile { get; set; }
        public IEnumerable<FoundPerson> MatchedPersons { get; set; }
    }

    public class PersonSearchRejectedEvent : PersonSearchRejected
    {

        private readonly List<DefaultValidationResult> _validationResults = new List<DefaultValidationResult>();

        public PersonSearchRejectedEvent(Guid searchRequestId, string fileId, ProviderProfile providerProfile)
        {
            TimeStamp = DateTime.Now;
            SearchRequestId = searchRequestId;
            ProviderProfile = providerProfile;
            FileId = fileId;
        }

        public void AddValidationResult(DefaultValidationResult validationResult)
        {
            _validationResults.Add(validationResult);
        }

        public Guid SearchRequestId { get; }
        public string FileId { get; }

        public DateTime TimeStamp { get; }
        public ProviderProfile ProviderProfile { get; }
        public IEnumerable<ValidationResult> Reasons => _validationResults;
    }

}