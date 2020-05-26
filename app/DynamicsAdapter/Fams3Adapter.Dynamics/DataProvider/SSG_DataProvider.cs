using Fams3Adapter.Dynamics.SearchApiRequest;
using Newtonsoft.Json;

namespace Fams3Adapter.Dynamics.DataProvider
{

    public class SSG_SearchapiRequestDataProvider
    {
        [JsonProperty("ssg_adaptorname")]
        public string AdaptorName { get; set; }

        [JsonProperty("ssg_SearchAPIRequestId")]
        public virtual SSG_SearchApiRequest SearchApiRequest { get; set; }

    }
}
