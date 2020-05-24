using Newtonsoft.Json;

namespace Fams3Adapter.Dynamics.InsuranceClaim
{
    public class SSG_PhoneNumberForAssets
    {
        [JsonProperty("ssg_name")]
        public string PhoneNumber { get; set; }

        [JsonProperty("ssg_type")]
        public string Type { get; set; }

        [JsonProperty("ssg_extension")]
        public string Extension { get; set; }

        [JsonProperty("ssg_ICBCClaim")]
        public virtual SSG_Asset_ICBCClaim SSG_Asset_ICBCClaim { get; set; }
    }
}
