using BcGov.Fams3.SearchApi.Contracts.IA;
using BcGov.Fams3.SearchApi.Contracts.Person;
using System;

namespace BcGov.Fams3.SearchApi.Core.Adapters.Models
{
    public class DefaultIASearchFailed : IASearchFailed
    {

        public DefaultIASearchFailed(Guid searchRequestId, string searchRequestKey, Person person, string batchNo = "None")
        {
            SearchRequestId = searchRequestId;
            TimeStamp = DateTime.Now;
            SearchRequestKey = searchRequestKey;
            Person = person;
            BatchNo = batchNo;
        }

        public Guid SearchRequestId { get; }
        public string SearchRequestKey { get; }
        public DateTime TimeStamp { get; }

        public Person Person { get;  }

        public string BatchNo { get;}

        public bool Retry { get; }

        public string Cause { get; }
    }
    
}
