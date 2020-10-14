using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Update;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Fams3Adapter.Dynamics.PhoneNumber
{
    public class PhoneNumberEntity : DynamicsEntity
    {
        [JsonProperty("ssg_PersonId")]
        public virtual SSG_Person Person { get; set; }

        [JsonProperty("ssg_SearchRequest")]
        public virtual SSG_SearchRequest SearchRequest { get; set; }

        [JsonProperty("ssg_informationsourcetext")]
        public int? InformationSource { get; set; }

        [JsonProperty("ssg_originalphonenumber")]
        [DisplayName("Phone Number")]
        public string TelePhoneNumber { get; set; }

        [JsonProperty("ssg_telephonenumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("ssg_suppliertypecode")]
        public string SupplierTypeCode { get; set; }

        [JsonProperty("ssg_phoneextension")]
        [DisplayName("Phone Extension")]
        public string PhoneExtension { get; set; }

        [JsonProperty("ssg_telephonenumbercategorytext")]
        [DisplayName("Type")]
        public int? TelephoneNumberType { get; set; }

        [JsonProperty("ssg_description")]
        public string Description { get; set; }

        [JsonProperty("ssg_notes")]
        [DisplayName("Notes")]
        public string Notes { get; set; }

        [JsonProperty("ssg_createdbyagency")]
        [UpdateIgnore]
        public bool IsCreatedByAgency { get; set; }

        [JsonProperty("ssg_agencyupdatedescription")]
        public string UpdateDetails { get; set; }

    }

    public class SSG_PhoneNumber : PhoneNumberEntity
    {
        [JsonProperty("ssg_phonenumberid")]
        public Guid PhoneNumberId { get; set; }
    }
}
