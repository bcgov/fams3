using System;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;

namespace BcGov.Fams3.SearchApi.Core.Adapters.Models
{
    public class DefaultPersonSearchFailed : PersonSearchFailed
    {

        public DefaultPersonSearchFailed(Guid searchRequestId, string SearchRequestKey, ProviderProfile providerProfile, string cause)
        {
            SearchRequestId = searchRequestId;
            ProviderProfile = providerProfile;
            Cause = cause;
            this.TimeStamp = DateTime.Now;
            this.SearchRequestKey = SearchRequestKey;
        }

        public Guid SearchRequestId { get; }
        public string SearchRequestKey { get;  }
        public DateTime TimeStamp { get; }
        public ProviderProfile ProviderProfile { get; }
        public string Cause { get; }
    }
}