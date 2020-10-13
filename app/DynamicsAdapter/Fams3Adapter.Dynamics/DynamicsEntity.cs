using Fams3Adapter.Dynamics.Update;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Fams3Adapter.Dynamics
{
    public abstract class DynamicsEntity : IUpdatableObject
    {
        [JsonProperty("statecode")]
        public int StateCode { get; set; }

        [JsonProperty("statuscode")]
        public int StatusCode { get; set; }

        [JsonProperty("ssg_datadate")]
        [DisplayName("Date")]
        public System.DateTime? Date1 { get; set; }

        [JsonProperty("ssg_datadatelabel")]
        [DisplayName("Date Type")]
        public string Date1Label { get; set; }

        [JsonProperty("ssg_datadate2")]
        [DisplayName("Date 2")]
        public System.DateTime? Date2 { get; set; }

        [JsonProperty("ssg_datadatelabel2")]
        [DisplayName("Date 2 Type")]
        public string Date2Label { get; set; }

        [JsonProperty("ssg_responsecomment")]
        public string ResponseComments { get; set; }

        public bool Existed { get; set; }
    }
}
