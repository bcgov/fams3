using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using System;
using System.Collections.Generic;
using System.Text;

namespace BcGov.Fams3.Redis.Model
{
    public class SearchRequest
    {
        public Guid SearchRequestId { get; set; }
        public string SearchRequestKey { get; set; }
        public Person Person { get; set; }
        public bool IsPreScreenSearch { get; set; }
        public IEnumerable<DataPartner> DataPartners { get; set; }

    }

    public class DataPartner
    {
        public string Name { get; set; }
        public bool Completed { get; set; }

        public int NumberOfRetries { get; set; }

        public int TimeBetweenRetries { get; set; }

        public SearchSpeedType SearchSpeed { get; set; }
    }

}
