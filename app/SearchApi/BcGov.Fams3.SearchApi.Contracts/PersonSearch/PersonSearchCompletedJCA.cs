using System;
using System.Collections.Generic;
using System.Text;

namespace BcGov.Fams3.SearchApi.Contracts.PersonSearch
{
    public interface PersonSearchCompletedJCA : PersonSearchAdapterEvent
    {
        IEnumerable<Person.PersonFound> MatchedPersons { get; }
        string Message { get; }
        SearchStatus Status { get; }
    }
}
