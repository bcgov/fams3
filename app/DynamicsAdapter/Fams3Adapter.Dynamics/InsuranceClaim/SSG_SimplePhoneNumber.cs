using Newtonsoft.Json;

namespace Fams3Adapter.Dynamics.InsuranceClaim
{
    public class SSG_SimplePhoneNumber
    {
        [JsonProperty("ssg_phonenumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("ssg_phonenumbertype")]
        public string Type { get; set; }

        [JsonProperty("ssg_phoneextension")]
        public string Extension { get; set; }

        [JsonProperty("ssg_ICBCClaim")]
        public virtual SSG_Asset_ICBCClaim SSG_Asset_ICBCClaim { get; set; }
    }
}
