using Newtonsoft.Json;
using System;

namespace Fams3Adapter.Dynamics.SearchRequest
{
    public class SSG_SearchRequestReason
    {
        [JsonProperty("ssg_searchrequestreasonid")]
        public Guid ReasonId { get; set; }

        [JsonProperty("ssg_searchreasoncode")]
        public string ReasonCode { get; set; }
    }
}
