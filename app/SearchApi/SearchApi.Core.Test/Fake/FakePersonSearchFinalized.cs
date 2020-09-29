using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using System;

namespace SearchApi.Core.Test.Fake
{
    public class FakePersonSearchFinalized : PersonSearchFinalized
    {
        public Guid SearchRequestId { get; set; }
        public string SearchRequestKey { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Message { get; set; }

        public ProviderProfile ProviderProfile { get; set; }
        public SearchStatus Status { get; set; }
    }
}
