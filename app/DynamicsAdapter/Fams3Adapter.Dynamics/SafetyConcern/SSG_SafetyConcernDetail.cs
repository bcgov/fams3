using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Newtonsoft.Json;
using System;

namespace Fams3Adapter.Dynamics.SafetyConcern
{
    public class SafetyConcernEntity : DynamicsEntity
    {
        [JsonProperty("ssg_type")]
        public string Type { get; set; }

        [JsonProperty("ssg_name")]
        public string Detail { get; set; }
    }

    public class SSG_SafetyConcernDetail : SafetyConcernEntity
    {
        [JsonProperty("ssg_safetyconcerndetailid")]
        public Guid SafetyConcernDetailId { get; set; }
    }
}
