using Newtonsoft.Json;
using System;

namespace Fams3Adapter.Dynamics.SearchRequest
{
    public class SSG_SearchRequest
    {
        [JsonProperty("ssg_searchrequestid")]
        public Guid SearchRequestId { get; set; }

        [JsonProperty("ssg_name")]
        public string FileId { get; set; }
    }
}
