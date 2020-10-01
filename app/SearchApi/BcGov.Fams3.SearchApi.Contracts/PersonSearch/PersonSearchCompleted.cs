using System.Collections.Generic;

namespace BcGov.Fams3.SearchApi.Contracts.PersonSearch
{
   public  interface PersonSearchCompleted  : PersonSearchAdapterEvent
    {
        IEnumerable<Person.PersonFound> MatchedPersons { get; }
        string Message { get; }
         SearchStatus Status { get; }
    }
}
