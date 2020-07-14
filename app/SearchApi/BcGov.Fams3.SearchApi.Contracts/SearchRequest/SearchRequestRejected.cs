using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using System.Collections.Generic;

namespace BcGov.Fams3.SearchApi.Contracts.SearchRequest
{
    public interface SearchRequestRejected  :SearchRequestEvent 
    {
        IEnumerable<ValidationResult> Reasons { get; }
    }
}
