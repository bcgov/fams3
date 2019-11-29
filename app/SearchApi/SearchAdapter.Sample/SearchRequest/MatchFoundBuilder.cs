using System;
using SearchApi.Core.Adapters.Contracts;
using SearchApi.Core.Person.Contracts;

namespace SearchAdapter.Sample.SearchRequest.MatchFound
{

    public class MatchFoundBuilder
    {

        private readonly IcbcMatchFound _matchFound;

        public MatchFoundBuilder(Guid searchRequestId)
        {
            _matchFound = new IcbcMatchFound(searchRequestId);
        }

        public MatchFoundBuilder WithPerson(Person person)
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
