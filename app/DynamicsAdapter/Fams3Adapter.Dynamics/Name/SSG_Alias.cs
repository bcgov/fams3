using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Update;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Fams3Adapter.Dynamics.Name
{
    //We have to use AliasEntity 
    //if we use SSG_Alias, as Simple.OData.Client will use url "ssg_alias". If we use AliasEntity, Simple.OData.Client will visit url ssg_aliases.
    //while Dynamics needs url /api/data/v9.0/ssg_aliases
    public class AliasEntity : DynamicsEntity
    {
        [JsonProperty("ssg_informationsourcetext")]
        public int? InformationSource { get; set; }

        [JsonProperty("ssg_persongivenname")]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Middle Name")]
        [JsonProperty("ssg_personmiddlename")]
        public string MiddleName { get; set; }

        [DisplayName("Last Name")]
        [JsonProperty("ssg_Personsurname")]
        public string LastName { get; set; }

        [DisplayName("Middle Name 2")]
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

        [JsonProperty("ssg_notes")]
        [DisplayName("Notes")]
        public string Notes { get; set; }

        [JsonProperty("ssg_personbirthdate")]
        public DateTime? DateOfBirth { get; set; }

        [JsonProperty("ssg_createdbyagency")]
        [UpdateIgnore]
        public bool IsCreatedByAgency { get; set; }

        [JsonProperty("ssg_agencyupdatedescription")]
        public string UpdateDetails { get; set; }
    }

    public class SSG_Aliase : AliasEntity
    {
        [JsonProperty("ssg_aliasid")]
        public Guid AliasId { get; set; }
    }
}
