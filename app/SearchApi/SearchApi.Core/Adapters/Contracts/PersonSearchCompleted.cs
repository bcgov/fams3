using System;
using System.Collections.Generic;
using System.Text;
using SearchApi.Core.Person.Contracts;

namespace SearchApi.Core.Adapters.Contracts
{
   public  interface PersonSearchCompleted  : PersonSearchAdapterEvent
    {
        Person.Contracts.Person Person { get; }

        IEnumerable<PersonId> PersonIds { get; }
    }
}
