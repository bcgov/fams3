using System;
using System.Collections.Generic;
using System.Text;

namespace BcGov.Fams3.SearchApi.Contracts.PersonSearch
{
   public interface PersonSearchReceived : PersonSearchEvent
    {
        object ReceivedPayload { get; }
    }
}
