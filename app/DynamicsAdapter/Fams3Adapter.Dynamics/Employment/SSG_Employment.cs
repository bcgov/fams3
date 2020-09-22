using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Types;
using Fams3Adapter.Dynamics.Update;
using Newtonsoft.Json;
using System;

namespace Fams3Adapter.Dynamics.Employment
{
    public class EmploymentEntity : DynamicsEntity
    {
        [JsonProperty("ssg_websiteurl")]
        public string Website { get; set; }

        [JsonProperty("ssg_PersonId")]
        public virtual SSG_Person Person { get; set; }

        [JsonProperty("ssg_occupation")]
        public string Occupation { get; set; }

        [JsonProperty("ssg_notes")]
        public string Notes { get; set; }

        [JsonProperty("ssg_locationpostalcode")]
        public string PostalCode { get; set; }

        [JsonProperty("ssg_LocationCountry")]
        public virtual SSG_Country Country { get; set; }

        [JsonProperty("ssg_locationcityname")]
        public string City { get; set; }

        [JsonProperty("ssg_suppliedby")]
        public int? InformationSource { get; set; }

        [JsonProperty("ssg_incomeassistancestatusoptionset")]
        public int? IncomeAssistanceStatusOption { get; set; }

        [JsonProperty("ssg_incomeassistancestatus")]
        public string IncomeAssistanceStatus { get; set; }

        [JsonProperty("ssg_recordtype")]
        [UpdateIgnore]
        public int EmploymentType { get; set; } = EmploymentRecordType.Employment.Value;

        [JsonProperty("ssg_employmentowner")]
        public string BusinessOwner { get; set; }

        [JsonProperty("ssg_employmentconfirmed")]
        public bool? EmploymentConfirmed { get; set; }

        [JsonProperty("ssg_countrytext")]
        public string CountryText { get; set; }

        [JsonProperty("ssg_CountrySubDivision")]
        public virtual SSG_CountrySubdivision CountrySubdivision { get; set; }

        [JsonProperty("ssg_countrysubdivision_text")]
        public string CountrySubdivisionText { get; set; }

        [JsonProperty("ssg_contactname")]
        public string ContactPerson { get; set; }

        [JsonProperty("ssg_employerlegalname")]
        public string BusinessName { get; set; }

        [JsonProperty("ssg_employerdbaname")]
        public string DBAName { get; set; }

        [JsonProperty("ssg_primaryphonenumber")]
        public string PrimaryPhoneNumber { get; set; }

        [JsonProperty("ssg_primaryphoneextension")]
        public string PrimaryPhoneExtension { get; set; }

        [JsonProperty("ssg_primaryfax")]
        public string PrimaryFax { get; set; }

        [JsonProperty("ssg_address")]
        public string AddressLine1 { get; set; }

        [JsonProperty("ssg_addresssecondaryunittext")]
        public string AddressLine2 { get; set; }

        [JsonProperty("ssg_addresslinethree")]
        public string AddressLine3 { get; set; }

        [JsonProperty("ssg_SearchRequest")]
        public virtual SSG_SearchRequest SearchRequest { get; set; }

        public virtual EmploymentContactEntity[] EmploymentContactEntities { get; set; }

        [JsonProperty("ssg_createdbyagency")]
        [UpdateIgnore]
        public bool IsCreatedByAgency { get; set; }

        [JsonProperty("ssg_employmentstatus")]
        [UpdateIgnore]
        public int? EmploymentStatus { get; set; }

        [JsonProperty("ssg_selfemploymentcompanyregistrationnumber")]
        public string SelfEmployComRegistrationNo { get; set; }

        [JsonProperty("ssg_selfemploymentcompanytype")]
        [UpdateIgnore]
        public int? SelfEmployComType { get; set; }

        [JsonProperty("selfemploymentcompanyrole")]
        [UpdateIgnore]
        public int? SelfEmployComRole { get; set; }

        [JsonProperty("ssg_selfemploymentpercentageofshares")]
        [UpdateIgnore]
        public int? SelfEmployPercentOfShare { get; set; }

        [JsonProperty("incomeassistanceclass")]
        [UpdateIgnore]
        public int? IncomeAssistanceCls { get; set; }

        [JsonProperty("ssg_incomeassistancedescription")]
        public string IncomeAssistanceDesc { get; set; }

        [JsonProperty("ssg_primarycontactphonenumber")]
        public string PrimaryContactPhone { get; set; }

        [JsonProperty("ssg_primarycontactphoneextension")]
        public string PrimaryContactPhoneExt { get; set; }
    }

    public class SSG_Employment : EmploymentEntity
    {
        [JsonProperty("ssg_employmentid")]
        public Guid EmploymentId { get; set; }

        [JsonProperty("ssg_ssg_employment_ssg_employmentcontact_EmploymentId")]
        public SSG_EmploymentContact[] SSG_EmploymentContacts { get; set; }

        public bool IsDuplicated { get; set; }
    }
}
