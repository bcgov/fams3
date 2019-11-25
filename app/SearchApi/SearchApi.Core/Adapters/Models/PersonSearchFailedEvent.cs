using System;
using System.Collections.Generic;
using SearchApi.Core.Adapters.Contracts;

namespace SearchApi.Core.Adapters.Models
{
    public class PersonSearchFailedEvent : PersonSearchFailed
    {
        public Guid SearchRequestId { get; set; }
        public ProviderProfile ProviderProfile { get; set; }
        public Exception Cause { get; set; }
    }
}