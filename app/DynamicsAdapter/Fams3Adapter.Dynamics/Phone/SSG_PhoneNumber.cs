using Fams3Adapter.Dynamics.Employment;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fams3Adapter.Dynamics.PhoneNumber
{
    public class SSG_PhoneNumber : DynamicsEntity
    {
        [JsonProperty("ssg_PersonId")]
        public virtual SSG_Person Person { get; set; }

        [JsonProperty("ssg_SearchRequest")]
        public virtual SSG_SearchRequest SearchRequest { get; set; }

        [JsonProperty("ssg_informationsourcetext")]
        public int? InformationSource { get; set; }

        [JsonProperty("ssg_LinkedEmployment")]
        public EmploymentEntity LinkedEmployment { get; set; }

        [JsonProperty("ssg_telephonenumber")]
        public string TelePhoneNumber { get; set; }

        [JsonProperty("ssg_suppliertypecode")]
        public string SupplierTypeCode { get; set; }

        [JsonProperty("ssg_phoneextension")]
        public string PhoneExtension { get; set; }

        [JsonProperty("ssg_telephonenumbercategorytext")]
        public int? TelephoneNumberType { get; set; }

        [JsonProperty("ssg_description")]
        public string Description { get; set; }

        [JsonProperty("ssg_notes")]
        public string Notes { get; set; }



    }
}
