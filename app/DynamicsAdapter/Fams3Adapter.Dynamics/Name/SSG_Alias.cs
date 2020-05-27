using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Newtonsoft.Json;

namespace Fams3Adapter.Dynamics.Name
{
    //We have to use SSG_Aliase 
    //if we use SSG_Alias, as Simple.OData.Client will use url "ssg_alias". If we use SSG_Aliase, Simple.OData.Client will visit url ssg_aliases.
    //while Dynamics needs url /api/data/v9.0/ssg_aliases
    public class SSG_Aliase : DynamicsEntity
    {
        [JsonProperty("ssg_informationsourcetext")]
        public int? InformationSource { get; set; }

        [JsonProperty("ssg_persongivenname")]
        public string FirstName { get; set; }

        [JsonProperty("ssg_personmiddlename")]
        public string MiddleName { get; set; }

        [JsonProperty("ssg_PersonSurName")]
        public string LastName { get; set; }

        [JsonProperty("ssg_personthirdgivenname")]
        public string ThirdGivenName { get; set; }

        [JsonProperty("ssg_personnamecategorytext")]
        public int? Type { get; set; }

        [JsonProperty("ssg_suppliertypecode")]
        public string SupplierTypeCode { get; set; }

        [JsonProperty("ssg_SearchRequest")]
        public virtual SSG_SearchRequest SearchRequest { get; set; }

        [JsonProperty("ssg_PersonId")]
        public virtual SSG_Person Person { get; set; }

        [JsonProperty("ssg_responsecomment")]
        public string Comments { get; set; }

        [JsonProperty("ssg_notes")]
        public string Notes { get; set; }
    }
}
