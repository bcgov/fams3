using System;
using System.Collections.Generic;
using SearchApi.Core.Adapters.Contracts;
using SearchApi.Core.Contracts;

namespace SearchApi.Core.Test.Fake
{
    public class FakePersonFound : PersonFound
    {
        public Guid SearchRequestId { get; } = Guid.NewGuid();

        public Person Person { get; } = new FakePerson();
        public IEnumerable<PersonId> PersonIds { get; } = new List<PersonId>() { new FakePersonId() };
    }
}