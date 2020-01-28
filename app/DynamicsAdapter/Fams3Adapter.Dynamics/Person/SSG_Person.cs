using Fams3Adapter.Dynamics.SearchRequest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fams3Adapter.Dynamics.Person
{
    public class SSG_Person : DynamicsEntity
    {
        [JsonProperty("ssg_searchrequestid")]
        public virtual SSG_SearchRequest SearchRequest { get; set; }

        [JsonProperty("ssg_informationsourcetext")]
        public int? InformationSource { get; set; }

        [JsonProperty("ssg_dateofbirth")]
        public string DateOfBirth { get; set; }

        [JsonProperty("ssg_dateofdeath")]
        public string DateOfDirth { get; set; }

        [JsonProperty("ssg_dateofdeathconfirmed")]
        public string DateOfDeathConfirmed { get; set; }

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
        public string ThirdGiveName { get; set; }

    }
}
