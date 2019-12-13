using System.Collections.Generic;
using BcGov.Fams3.SearchApi.Core.Adapters.Models;

namespace BcGov.Fams3.SearchApi.Core.Adapters.Contracts
{
    public interface PersonSearchRejected : PersonSearchAdapterEvent
    {
        IEnumerable<ValidationResult> Reasons { get; }
    }
}