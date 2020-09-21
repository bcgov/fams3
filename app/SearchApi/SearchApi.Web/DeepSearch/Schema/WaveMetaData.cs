using BcGov.Fams3.SearchApi.Contracts.Person;
using System.Collections.Generic;

namespace SearchApi.Web.DeepSearch.Schema
{
    public class WaveMetaData
    {
        public string SearchRequestKey { get; set; }
        public int CurrentWave { get; set; }
        public string DataPartner { get; set; }
       
        public List<Person> AllParameter { get; set; }

        public List<Person> NewParameter { get; set; }

        public int NumberOfRetries { get; set; }

        public int TimeBetweenRetries { get; set; }




    }



}
