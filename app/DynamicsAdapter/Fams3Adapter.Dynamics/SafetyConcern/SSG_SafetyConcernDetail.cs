using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Newtonsoft.Json;
using System;

namespace Fams3Adapter.Dynamics.SafetyConcern
{
    public class SafetyConcernEntity : DynamicsEntity
    {
        [JsonProperty("ssg_type")]
        public int? Type { get; set; }

        [JsonProperty("ssg_name")]
        public string Detail { get; set; }

        [JsonProperty("ssg_createdbyagency")]
        public bool IsCreatedByAgency { get; set; }

        [JsonProperty("ssg_PersonId")]
        public virtual SSG_Person Person { get; set; }

        [JsonProperty("ssg_SearchRequest")]
        public virtual SSG_SearchRequest SearchRequest { get; set; }

        [JsonProperty("ssg_suppliedby")]
        public int? InformationSource { get; set; }
    }

    public class SSG_SafetyConcernDetail : SafetyConcernEntity
    {
        [JsonProperty("ssg_safetyconcerndetailid")]
        public Guid SafetyConcernDetailId { get; set; }
    }
}
