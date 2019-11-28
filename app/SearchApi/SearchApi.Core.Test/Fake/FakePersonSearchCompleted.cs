using SearchApi.Core.Adapters.Contracts;
using SearchApi.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SearchApi.Core.Test.Fake
{
    public class FakePersonSearchCompleted : PersonSearchCompleted
    {
        public Person Person { get; set; }

        public IEnumerable<PersonId> PersonIds { get; set; }

        public Guid SearchRequestId { get; set; }

        public DateTime TimeStamp { get; set; }

        public ProviderProfile ProviderProfile { get; set; }
    }
}
