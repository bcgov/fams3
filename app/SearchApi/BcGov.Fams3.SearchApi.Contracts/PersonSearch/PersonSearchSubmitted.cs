using System;
using System.Collections.Generic;
using System.Text;

namespace BcGov.Fams3.SearchApi.Contracts.PersonSearch
{
    public interface PersonSearchSubmitted : PersonSearchAdapterEvent
    {
            string Message { get; }
       SearchStatus Status { get; set; }
    }
}
