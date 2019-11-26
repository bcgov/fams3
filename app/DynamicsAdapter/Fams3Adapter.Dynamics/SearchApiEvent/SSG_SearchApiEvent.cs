using System;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Fams3Adapter.Dynamics.SearchRequest;
using Newtonsoft.Json;

namespace Fams3Adapter.Dynamics.SearchApiEvent
{
    /// <summary>
    /// Represents FAMS3 Dynamics SearchApiEvent
    /// </summary>
    public class SSG_SearchApiEvent
    {
        [JsonProperty("ssg_searchapieventid")]
        public Guid Id { get; set; }
        [JsonProperty("ssg_name")] 
        public string Name { get; set; }
        [JsonProperty("ssg_eventtype")]
        public string Type { get; set; }
        [JsonProperty("ssg_eventmessage")]
        public string Message { get; set; }
        [JsonProperty("ssg_providername")]
        public string ProviderName { get; set; }
        [JsonProperty("ssg_timestamp")]
        public DateTime TimeStamp { get; set; }
        [JsonProperty("ssg_SearchAPIRequest")]
        public virtual SSG_SearchApiRequest SearchApiRequest { get; set; }
    }
}