using Newtonsoft.Json;

namespace Fams3Adapter.Dynamics
{
    public abstract class DynamicsEntity
    {
        [JsonProperty("statecode")]
        public int StateCode { get; set; }

        [JsonProperty("statuscode")]
        public int StatusCode { get; set; }

        [JsonProperty("ssg_datadate")]
        public System.DateTime? Date1 { get; set; }

        [JsonProperty("ssg_datadatelabel")]
        public string Date1Label { get; set; }

        [JsonProperty("ssg_datadate2")]
        public System.DateTime? Date2 { get; set; }

        [JsonProperty("ssg_datadatelabel2")]
        public string Date2Label { get; set; }

    }
}
