using System;

namespace BcGov.Fams3.SearchApi.Contracts.Rfi
{
    public interface RequestForInformation
    {		
        Guid Id { get; }
		string Recipient {get;}
		string DocumentBody{get;}
    }
}