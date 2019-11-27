using System;
using System.Collections.Generic;
using SearchApi.Core.Adapters.Models;

namespace SearchApi.Core.Adapters.Contracts
{
    public interface PersonSearchRejected : PersonSearchEvent, AdapterEvent
    {
        IEnumerable<ValidationResult> Reasons { get; }
    }
}