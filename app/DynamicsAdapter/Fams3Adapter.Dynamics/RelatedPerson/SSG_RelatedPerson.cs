using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Update;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Fams3Adapter.Dynamics.RelatedPerson
{
    public class RelatedPersonEntity : DynamicsEntity
    {
        [JsonProperty("ssg_informationsourcetext")]
        public int? InformationSource { get; set; }

        [JsonProperty("ssg_personbirthdate")]
        [DisplayName("Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [JsonProperty("ssg_persongivenname")]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [JsonProperty("ssg_personmiddlename")]
        [DisplayName("Middle Name")]
        public string MiddleName { get; set; }

        [JsonProperty("ssg_PersonId")]
        public virtual SSG_Person Person { get; set; }

        [JsonProperty("ssg_personsurname")]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [JsonProperty("ssg_personthirdgivenname")]
        [DisplayName("Middle Name 2")]
        public string ThirdGivenName { get; set; }

        [JsonProperty("ssg_personrelationshipcategorytext")]
        [DisplayName("Relationship")]
        public int? Type { get; set; }

        [JsonProperty("ssg_personcategorytext")]
        [DisplayName("Type")]
        public int? PersonType { get; set; }

        [JsonProperty("ssg_personsextext")]
        [DisplayName("Gender")]
        public int? Gender { get; set; }

        [JsonProperty("ssg_supplierrelationtype")]
        public string SupplierRelationType { get; set; }

        [JsonProperty("ssg_SearchRequest")]
        public virtual SSG_SearchRequest SearchRequest { get; set; }

        [JsonProperty("ssg_description")]
        [DisplayName("Description")]
        public string Description { get; set; }

        [JsonProperty("ssg_notes")]
        [DisplayName("Notes")]
        public string Notes { get; set; }

        [JsonProperty("ssg_createdbyagency")]
        [UpdateIgnore]
        public bool IsCreatedByAgency { get; set; }

        [JsonProperty("ssg_agencyupdatedescription")]
        public string UpdateDetails { get; set; }

        [JsonProperty("ssg_persondeathdate")]
        [DisplayName("Date of Death")]
        public DateTime? DateOfDeath { get; set; }
    }

    public class SSG_Identity : RelatedPersonEntity
    {
        [JsonProperty("ssg_identityid")]
        public Guid RelatedPersonId { get; set; }
    }
}
