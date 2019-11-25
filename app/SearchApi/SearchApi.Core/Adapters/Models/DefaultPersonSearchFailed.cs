using System;
using System.Collections.Generic;
using SearchApi.Core.Adapters.Contracts;

namespace SearchApi.Core.Adapters.Models
{
    public class DefaultPersonSearchFailed : PersonSearchFailed
    {

        public DefaultPersonSearchFailed(Guid searchRequestId, ProviderProfile providerProfile, Exception cause)
        {
            SearchRequestId = searchRequestId;
            ProviderProfile = providerProfile;
            Cause = cause;
            this.TimeStamp = DateTime.Now;
        }

        public Guid SearchRequestId { get; }
        public DateTime TimeStamp { get; }
        public ProviderProfile ProviderProfile { get; }
        public Exception Cause { get; }
    }
}