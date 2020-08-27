using Newtonsoft.Json;
using System;

namespace Fams3Adapter.Dynamics.Agency
{
    public class SSG_AgencyLocation : DynamicsEntity
    {
        [JsonProperty("ssg_agencylocationid")]
        public Guid AgencyLocationId { get; set; }

        [JsonProperty("ssg_locationcityname")]
        public string City { get; set; }

        [JsonProperty("ssg_agencycode")]
        public string AgencyCode { get; set; }

        [JsonProperty("ssg_locationcode")]
        public string LocationCode { get; set; }
    }
}
