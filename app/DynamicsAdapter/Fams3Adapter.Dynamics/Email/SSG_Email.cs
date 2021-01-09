using Newtonsoft.Json;
using System.ComponentModel;

namespace Fams3Adapter.Dynamics.Email
{
    public class SSG_Email : DynamicsEntity
    {
        [JsonProperty("ssg_emailcategorytext")]
        public int? EmailType { get; set; }

        [JsonProperty("ssg_email")]
        public string Email { get; set; }

    }
}
