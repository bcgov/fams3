﻿using System.Collections.Generic;

namespace BcGov.Fams3.SearchApi.Contracts.PersonSearch
{
   public  interface PersonSearchCompleted  : PersonSearchAdapterEvent
    {
        IEnumerable<Person.FoundPerson> MatchedPersons { get; }
    }
}
