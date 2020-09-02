using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Update;
using Newtonsoft.Json;
using System;

namespace Fams3Adapter.Dynamics.RelatedPerson
{
    public class RelatedPersonEntity : DynamicsEntity
    {
        [JsonProperty("ssg_informationsourcetext")]
        public int? InformationSource { get; set; }

        [JsonProperty("ssg_personbirthdate")]
        public DateTime? DateOfBirth { get; set; }

        [JsonProperty("ssg_persongivenname")]
        public string FirstName { get; set; }

        [JsonProperty("ssg_personmiddlename")]
        public string MiddleName { get; set; }

        [JsonProperty("ssg_PersonId")]
        public virtual SSG_Person Person { get; set; }

        [JsonProperty("ssg_personsurname")]
        public string LastName { get; set; }

        [JsonProperty("ssg_personthirdgivenname")]
        public string ThirdGivenName { get; set; }

        [JsonProperty("ssg_personrelationshipcategorytext")]
        public int? Type { get; set; }

        [JsonProperty("ssg_personcategorytext")]
        public int? PersonType { get; set; }

        [JsonProperty("ssg_personsextext")]
        public int? Gender { get; set; }

        [JsonProperty("ssg_supplierrelationtype")]
        public string SupplierRelationType { get; set; }

        [JsonProperty("ssg_SearchRequest")]
        public virtual SSG_SearchRequest SearchRequest { get; set; }

        [JsonProperty("ssg_description")]
        public string Description { get; set; }

        [JsonProperty("ssg_notes")]
        public string Notes { get; set; }

        [JsonProperty("ssg_createdbyagency")]
        [UpdateIgnore]
        public bool IsCreatedByAgency { get; set; }
    }

    public class SSG_Identity : RelatedPersonEntity
    {
        [JsonProperty("ssg_identityid")]
        public Guid RelatedPersonId { get; set; }
    }
}
