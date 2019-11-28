using SearchApi.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SearchApi.Core.Adapters.Contracts
{
   public  interface PersonSearchCompleted  : PersonSearchAdapterEvent
    {
        Person Person { get; }

        IEnumerable<PersonId> PersonIds { get; }
    }
}
