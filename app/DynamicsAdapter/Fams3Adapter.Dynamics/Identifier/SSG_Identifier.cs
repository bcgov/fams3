using Newtonsoft.Json;
using System;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.SearchApiRequest;

namespace Fams3Adapter.Dynamics.Identifier
{
    public class SSG_Identifier : DynamicsEntity
    {
        [JsonProperty("ssg_identification")]
        public string Identification { get; set; }

        [JsonProperty("ssg_identificationeffectivedate")]
        public DateTime? IdentificationEffectiveDate { get; set; }

        [JsonProperty("ssg_identificationexpirationdate")]
        public DateTime? IdentificationExpirationDate { get; set; }

        [JsonProperty("ssg_SearchRequest")]
        public virtual SSG_SearchRequest SSG_SearchRequest { get; set; }

        [JsonProperty("ssg_identificationcategorytext")]
        public int? IdentifierType { get; set; }

        [JsonProperty("ssg_informationsourcetext")]
        public int? InformationSource { get; set; }

        [JsonProperty("ssg_issuedby")]
        public string IssuedBy { get; set; }


        }

}
