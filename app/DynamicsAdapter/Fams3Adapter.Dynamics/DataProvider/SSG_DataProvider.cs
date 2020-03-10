using Fams3Adapter.Dynamics.SearchApiRequest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fams3Adapter.Dynamics.DataProvider
{

    public class SSG_SearchapiRequestDataProvider 
    {
        [JsonProperty("ssg_name")]
        public string Name { get; set; }

        [JsonProperty("ssg_suppliedbyvalue")]
        public int? SuppliedByValue { get; set; }

        [JsonProperty("ssg_SearchAPIRequestId")]
        public virtual SSG_SearchApiRequest SearchApiRequest { get; set; }

    }
}
