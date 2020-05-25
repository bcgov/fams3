using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fams3Adapter.Dynamics.InsuranceClaim
{
    public class SSG_InvolvedParty
    {
        [JsonProperty("ssg_firstname")]
        public string FirstName { get; set; }

        [JsonProperty("ssg_lastname")]
        public string LastName { get; set; }

        [JsonProperty("ssg_middlename")]
        public string MiddleName { get; set; }

        [JsonProperty("ssg_thirdgivenname")]
        public string OtherName { get; set; }

        [JsonProperty("ssg_orgname")]
        public string OrganizationName { get; set; }

        [JsonProperty("ssg_partydescription")]
        public string PartyDescription { get; set; }

        [JsonProperty("ssg_partytypecode")]
        public string PartyTypeCode { get; set; }

        [JsonProperty("ssg_notes")]
        public string Notes { get; set; }

        [JsonProperty("ssg_ICBCClaim")]
        public virtual SSG_Asset_ICBCClaim SSG_Asset_ICBCClaim { get; set; }
    }
}
