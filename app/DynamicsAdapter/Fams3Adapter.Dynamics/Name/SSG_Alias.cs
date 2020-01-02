using Fams3Adapter.Dynamics.SearchRequest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fams3Adapter.Dynamics.Name
{
    public class SSG_Alias : DynamicsEntity
    {
        [JsonProperty("ssg_informationsourcetext")]
        public int? InformationSource { get; set; }

        [JsonProperty("ssg_datadate")]
        public System.DateTime? EffectiveDate { get; set; }

        [JsonProperty("ssg_datadatelabel")]
        public string EffectiveDateLabel { get; set; }

        [JsonProperty("ssg_datadate2")]
        public System.DateTime? EndDate { get; set; }

        [JsonProperty("ssg_datadatelabel2")]
        public string EndDateLabel { get; set; }

        [JsonProperty("ssg_personfullname")]
        public string FullName { get; set; }

        [JsonProperty("ssg_persongivenname")]
        public string FirstName { get; set; }

        [JsonProperty("ssg_personmiddlename")]
        public string MiddleName { get; set; }

        [JsonProperty("ssg_PersonSurName")]
        public string LastName { get; set; }

        [JsonProperty("ssg_personthirdgivenname")]
        public string ThirdGivenName { get; set; }

        [JsonProperty("ssg_personnamecategorytext")]
        public int? Type { get; set; }

        [JsonProperty("ssg_SearchRequest")]
        public virtual SSG_SearchRequest SearchRequest { get; set; }

        [JsonProperty("ssg_responsecomment")]
        public string Comments { get; set; }
    }
}
