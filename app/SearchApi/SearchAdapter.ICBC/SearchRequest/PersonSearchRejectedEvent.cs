using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SearchApi.Core.Adapters.Contracts;
using SearchApi.Core.Adapters.Models;

namespace SearchAdapter.ICBC.SearchRequest
{
    public class PersonSearchRejectedEvent : PersonSearchRejected
    {
        public Guid RequestId { get; set; }
        public ProviderProfile ProviderProfile { get; set; }
        public IEnumerable<ValidationResult> Reasons { get; set; }
    }
}
