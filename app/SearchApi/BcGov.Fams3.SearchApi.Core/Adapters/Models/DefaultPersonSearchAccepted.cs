using System;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;

namespace BcGov.Fams3.SearchApi.Core.Adapters.Models
{
    public class DefaultPersonSearchAccepted : PersonSearchAccepted
    {
        public DefaultPersonSearchAccepted(Guid searchRequestId, ProviderProfile providerProfile, string SearchRequestKey)
        {
            TimeStamp = DateTime.Now;
            SearchRequestId = searchRequestId;
            ProviderProfile = providerProfile;
            SearchRequestKey = SearchRequestKey;
        }

        public Guid SearchRequestId { get; }
        public String SearchRequestKey { get; }
        public DateTime TimeStamp { get; }
        public ProviderProfile ProviderProfile { get; }
    }
}