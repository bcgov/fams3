using System;
using System.Collections.Generic;
using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;

namespace SearchApi.Core.Test.Fake
{
    public class FakePersonSearchCompleted : PersonSearchCompleted
    {
        public IEnumerable<Person> MatchedPersons { get; set; }

        public Guid SearchRequestId { get; set; }
        public string FileId { get; set; }

        public DateTime TimeStamp { get; set; }

        public ProviderProfile ProviderProfile { get; set; }
    }
}
