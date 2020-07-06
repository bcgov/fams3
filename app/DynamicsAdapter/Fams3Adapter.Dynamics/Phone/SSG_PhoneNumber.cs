﻿using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Newtonsoft.Json;
using System;

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

        public override string ToResultName()
        {
            return $"Duplicate | PhoneNumber | {TelePhoneNumber} {PhoneExtension}";
        }
    }

    public class SSG_PhoneNumber : PhoneNumberEntity
    {
        [JsonProperty("ssg_phonenumberid")]
        public Guid PhoneNumberId { get; set; }
    }
}
