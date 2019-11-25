using System;
using SearchApi.Core.Adapters.Contracts;

namespace SearchApi.Core.Adapters.Models
{
    public class DefaultPersonSearchAccepted : PersonSearchAccepted
    {
        public DefaultPersonSearchAccepted(Guid searchRequestId, ProviderProfile providerProfile)
        {
            SearchRequestId = searchRequestId;
            ProviderProfile = providerProfile;
        }

        public Guid SearchRequestId { get; }
        public ProviderProfile ProviderProfile { get; }
    }
}