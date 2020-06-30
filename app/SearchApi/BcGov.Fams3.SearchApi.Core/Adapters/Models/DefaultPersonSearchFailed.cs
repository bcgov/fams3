using System;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;

namespace BcGov.Fams3.SearchApi.Core.Adapters.Models
{
    public class DefaultPersonSearchFailed : PersonSearchFailed
    {

        public DefaultPersonSearchFailed(Guid searchRequestId, string searchRequestKey, ProviderProfile providerProfile, string cause)
        {
            SearchRequestId = searchRequestId;
            ProviderProfile = providerProfile;
            Cause = cause;
            TimeStamp = DateTime.Now;
            SearchRequestKey = searchRequestKey;
        }

        public Guid SearchRequestId { get; }
        public string SearchRequestKey { get;  }
        public DateTime TimeStamp { get; }
        public ProviderProfile ProviderProfile { get; }
        public string Cause { get; }
    }
}