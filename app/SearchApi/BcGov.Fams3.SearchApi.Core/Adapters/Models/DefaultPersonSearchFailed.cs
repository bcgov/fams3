using System;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;

namespace BcGov.Fams3.SearchApi.Core.Adapters.Models
{
    public class DefaultPersonSearchFailed : PersonSearchFailed
    {

        public DefaultPersonSearchFailed(Guid searchRequestId, string fileId, ProviderProfile providerProfile, string cause)
        {
            SearchRequestId = searchRequestId;
            ProviderProfile = providerProfile;
            Cause = cause;
            this.TimeStamp = DateTime.Now;
            this.FileId = fileId;
        }

        public Guid SearchRequestId { get; }
        public string FileId { get;  }
        public DateTime TimeStamp { get; }
        public ProviderProfile ProviderProfile { get; }
        public string Cause { get; }
    }
}