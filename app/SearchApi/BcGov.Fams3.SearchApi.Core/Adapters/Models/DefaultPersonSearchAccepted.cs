using System;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;

namespace BcGov.Fams3.SearchApi.Core.Adapters.Models
{
    public class DefaultPersonSearchAccepted : PersonSearchAccepted
    {
        public DefaultPersonSearchAccepted(Guid searchRequestId, ProviderProfile providerProfile, string searchRequestKey)
        {
            TimeStamp = DateTime.Now;
            SearchRequestId = searchRequestId;
            ProviderProfile = providerProfile;
            this.SearchRequestKey = searchRequestKey;
        }

        public Guid SearchRequestId { get; }
        public String SearchRequestKey { get; }
        public DateTime TimeStamp { get; }
        public ProviderProfile ProviderProfile { get; }
        public SearchStatus Status { get; }

        public string Message { get; }
    }
}