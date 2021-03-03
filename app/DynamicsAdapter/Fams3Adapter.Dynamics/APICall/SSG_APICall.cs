using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fams3Adapter.Dynamics.APICall
{
    public class SSG_APICall
    {
        [JsonProperty("ssg_apicallid")]
        public Guid Id { get; set; }

        [JsonProperty("ssg_notes")]
        public string Notes { get; set; }

        [JsonProperty("statuscode")]
        public int StatusCode { get; set; }

        [JsonProperty("statecode")]
        public int StateCode { get; set; }
    }


}
