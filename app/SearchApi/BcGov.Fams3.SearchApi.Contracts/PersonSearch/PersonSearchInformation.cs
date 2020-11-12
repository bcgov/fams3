using System;
using System.Collections.Generic;
using System.Text;

namespace BcGov.Fams3.SearchApi.Contracts.PersonSearch
{
   public interface PersonSearchInformation : PersonSearchAdapterEvent
    
    {
        string Message { get; set; }
    }
}
