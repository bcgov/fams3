using SearchApi.Core.Adapters.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using SearchApi.Core.Person.Contracts;

namespace SearchApi.Core.Test.Fake
{
    public class FakePersonSearchCompleted : PersonSearchCompleted
    {
        public Person.Contracts.Person Person { get; set; }

        public IEnumerable<PersonId> PersonIds { get; set; }

        public Guid SearchRequestId { get; set; }

        public DateTime TimeStamp { get; set; }

        public ProviderProfile ProviderProfile { get; set; }
    }
}
