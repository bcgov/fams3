using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using System;
using System.Collections.Generic;

namespace SearchApi.Core.Test.Fake
{

    public class FakePersonSearchCompletedJCA : PersonSearchCompletedJCA
    {
        public IEnumerable<PersonFound> MatchedPersons { get; set; }

        public Guid SearchRequestId { get; set; }
        public string SearchRequestKey { get; set; }

        public DateTime TimeStamp { get; set; }

        public ProviderProfile ProviderProfile { get; set; }

        public SearchStatus Status { get; set; }
        public string Message { get; set; }

    }
}
