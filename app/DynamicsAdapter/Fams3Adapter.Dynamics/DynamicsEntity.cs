using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fams3Adapter.Dynamics
{
    public abstract class DynamicsEntity
    {
        [JsonProperty("statecode")]
        public int StateCode { get; set; }

        [JsonProperty("statuscode")]
        public int StatusCode { get; set; }

        [JsonProperty("ssg_datadate")]
        public System.DateTime? EffectiveDate { get; set; }

        [JsonProperty("ssg_datadatelabel")]
        public string EffectiveDateLabel { get; set; }

        [JsonProperty("ssg_datadate2")]
        public System.DateTime? EndDate { get; set; }

        [JsonProperty("ssg_datadatelabel2")]
        public string EndDateLabel { get; set; }
    }
}
