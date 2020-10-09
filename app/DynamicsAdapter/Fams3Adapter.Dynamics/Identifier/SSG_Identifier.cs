using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Update;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Fams3Adapter.Dynamics.Identifier
{
    public class IdentifierEntity : DynamicsEntity
    {
        [JsonProperty("ssg_identification")]
        [DisplayName("ID")]
        public string Identification { get; set; }

        [JsonProperty("ssg_suppliertypecode")]
        public string SupplierTypeCode { get; set; }

        [JsonProperty("ssg_identificationcategorytext")]
        [DisplayName("Type")]
        public int? IdentifierType { get; set; }

        [JsonProperty("ssg_notes")]
        [DisplayName("Notes")]
        public string Notes { get; set; }

        [JsonProperty("ssg_description")]
        [DisplayName("Description")]
        public string Description { get; set; }

        [JsonProperty("ssg_issuedby")]
        public string IssuedBy { get; set; }

        [JsonProperty("ssg_SearchRequest")]
        public virtual SSG_SearchRequest SearchRequest { get; set; }

        [JsonProperty("ssg_PersonId")]
        public virtual SSG_Person Person { get; set; }

        [JsonProperty("ssg_informationsourcetext")]
        public int? InformationSource { get; set; }

        [JsonProperty("ssg_createdbyagency")]
        [UpdateIgnore]
        public bool IsCreatedByAgency { get; set; }
    }

    public class SSG_Identifier : IdentifierEntity
    {
        [JsonProperty("ssg_identifierid")]
        public Guid IdentifierId { get; set; }
    }

}
