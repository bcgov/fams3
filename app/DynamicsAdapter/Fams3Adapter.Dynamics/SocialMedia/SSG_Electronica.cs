using Newtonsoft.Json;
using System.ComponentModel;

namespace Fams3Adapter.Dynamics.SocialMedia
{
    public class SSG_Electronica : DynamicsEntity
    {
        [JsonProperty("ssg_electronicaddresscategorytext")]
        public int? SocialMediaAccountType { get; set; }

        [JsonProperty("ssg_electronicaddress")]
        public string SocialMediaAddress { get; set; }

    }
}
