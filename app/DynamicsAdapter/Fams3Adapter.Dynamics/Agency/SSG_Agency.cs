using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fams3Adapter.Dynamics.Agency
{
    public class SSG_Agency : DynamicsEntity
    {
        [JsonProperty("ssg_agencyid")]
        public Guid AgencyId { get; set; }

        [JsonProperty("ssg_organizationabbreviationtext")]
        public string AgencyCode { get; set; }
    }

}
