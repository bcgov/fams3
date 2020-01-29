using Fams3Adapter.Dynamics.SearchRequest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fams3Adapter.Dynamics.Person
{
    public class SSG_Person 
    {

        [JsonProperty("statecode")]
        public int StateCode { get; set; }

        [JsonProperty("statuscode")]
        public int StatusCode { get; set; }
        [JsonProperty("ssg_SearchRequestId")]
        public virtual SSG_SearchRequest SearchRequest { get; set; }

        [JsonProperty("ssg_informationsourcetext")]
        public int? InformationSource { get; set; }

        [JsonProperty("ssg_dateofbirth")]
        public DateTime? DateOfBirth { get; set; }

        [JsonProperty("ssg_dateofdeath")]
        public DateTime? DateOfDeath { get; set; }

        [JsonProperty("ssg_dateofdeathconfirmed")]
        public bool? DateOfDeathConfirmed { get; set; }

        [JsonProperty("ssg_firstname")]
        public string FirstName { get; set; }

        [JsonProperty("ssg_gender")]
        public string Gender { get; set; }

        [JsonProperty("ssg_incarcerated")]
        public int? Incacerated { get; set; }

        [JsonProperty("ssg_lastname")]
        public string LastName { get; set; }

        [JsonProperty("ssg_middlename")]
        public string MiddleName { get; set; }

        [JsonProperty("ssg_notes")]
        public string Notes { get; set; }

        [JsonProperty("ssg_thirdgivenname")]
        public string ThirdGivenName { get; set; }

    }
}
