using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fams3Adapter.Dynamics.SearchRequest
{
    public class SSG_SearchRequest
    {
        [JsonProperty("ssg_searchrequestid")]
        public Guid SearchRequestId { get; set; }
    }
}
