using Fams3Adapter.Dynamics.SearchRequest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fams3Adapter.Dynamics.PhoneNumber
{
    public class SSG_PhoneNumber : DynamicsEntity
    {
        [JsonProperty("ssg_SearchRequest")]
        public virtual SSG_SearchRequest SSG_SearchRequest { get; set; }

        [JsonProperty("ssg_informationsourcetext")]
        public int? InformationSource { get; set; }

        [JsonProperty("ssg_datadatelabel")]
        public string DateType { get; set; }

        [JsonProperty("ssg_datadate")]
        public DateTime DataDate { get; set; }

        [JsonProperty("ssg_telephonenumber")]
        public string TelePhoneNumber { get; set; }

        [JsonProperty("ssg_telephonenumbercategorytext")]
        public int? TelephoneNumberType { get; set; }

    }
}
