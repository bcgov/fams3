using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SearchApi.Core.Adapters.Contracts;
using SearchApi.Core.Person.Contracts;

namespace SearchAdapter.ICBC.SearchRequest
{

    public class IcbcMatchFoundBuilder
    {

        private readonly IcbcMatchFound _matchFound;

        public IcbcMatchFoundBuilder(Guid searchRequestId)
        {
            _matchFound = new IcbcMatchFound(searchRequestId);
        }

        public IcbcMatchFoundBuilder WithPerson(Person person)
        {
            this._matchFound.MatchedPerson = person;
            return this;
        }

        public SearchApi.Core.Adapters.Contracts.PersonSearchCompleted Build()
        {
            return this._matchFound;
        }

    }

   
    public sealed class IcbcMatchFound : PersonSearchCompleted
    {

        public IcbcMatchFound(Guid searchRequestId)
        {
            SearchRequestId = searchRequestId;
            TimeStamp = DateTime.Now;
        }

        public Guid SearchRequestId { get; }
        public DateTime TimeStamp { get; }
        public Person MatchedPerson { get; set; }
        public ProviderProfile ProviderProfile { get; }
    }
}
