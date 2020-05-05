using System;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;

namespace SearchApi.Core.Test.Fake
{
    public class FakePersonSearchAccepted : PersonSearchAccepted
    {
      
            public Guid SearchRequestId { get; set; }

            public string FileId { get; set; }

            public DateTime TimeStamp { get; set; }

            public ProviderProfile ProviderProfile { get; set; }
        
    }
}
