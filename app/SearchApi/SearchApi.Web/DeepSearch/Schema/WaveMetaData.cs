using BcGov.Fams3.SearchApi.Contracts.Person;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchApi.Web.DeepSearch.Schema
{
    public class WaveMetaData
    {
        public string SearchRequestKey { get; set; }
        public int CurrentWave { get; set; }
        public string DataPartner { get; set; }
       
        public List<Person> AllParameters { get; set; }

        public List<Person> NewParameterss { get; set; }


    }



}
