using BcGov.Fams3.SearchApi.Core.Adapters.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using BcGov.Fams3.SearchApi.Core.Person.Contracts;

namespace SearchApi.Core.Test.Fake
{
    public class FakePersonSearchCompleted : PersonSearchCompleted
    {
        public Person MatchedPerson { get; set; }

        public Guid SearchRequestId { get; set; }

        public DateTime TimeStamp { get; set; }

        public ProviderProfile ProviderProfile { get; set; }
    }
}
