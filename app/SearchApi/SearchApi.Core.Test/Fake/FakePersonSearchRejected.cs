using BcGov.Fams3.SearchApi.Core.Adapters.Models;
using System;
using System.Collections.Generic;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;

namespace SearchApi.Core.Test.Fake
{
    public class FakePersonSearchRejected : PersonSearchRejected
    {
        public IEnumerable<ValidationResult> Reasons {get;set;}

        public Guid SearchRequestId { get; set; }
        public string SearchRequestKey { get; set; }

        public DateTime TimeStamp { get; set; }

        public ProviderProfile ProviderProfile { get; set; }
    }
}
