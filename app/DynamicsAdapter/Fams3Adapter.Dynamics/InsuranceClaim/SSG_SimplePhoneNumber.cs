using Newtonsoft.Json;
using System;

namespace Fams3Adapter.Dynamics.InsuranceClaim
{
    public class SimplePhoneNumberEntity
    {
        [JsonProperty("ssg_phonenumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("ssg_phonenumber_original")]
        public string OriginalPhoneNumber { get; set; }

        [JsonProperty("ssg_phonenumbertype")]
        public string Type { get; set; }

        [JsonProperty("ssg_phoneextension")]
        public string Extension { get; set; }

        [JsonProperty("ssg_ICBCClaim")]
        public virtual SSG_Asset_ICBCClaim SSG_Asset_ICBCClaim { get; set; }
    }

    public class SSG_SimplePhoneNumber : SimplePhoneNumberEntity
    {
        [JsonProperty("ssg_simplephonenumberid")]
        public Guid SimplePhoneNumberId { get; set; }
    }
}
