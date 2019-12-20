using System.Collections.Generic;

namespace BcGov.Fams3.SearchApi.Contracts.PersonSearch
{
    public interface PersonSearchRejected : PersonSearchAdapterEvent
    {
        IEnumerable<ValidationResult> Reasons { get; }
    }
}