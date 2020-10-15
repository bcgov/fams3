using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Types;
using Fams3Adapter.Dynamics.Update;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Fams3Adapter.Dynamics.Employment
{
    public class EmploymentEntity : DynamicsEntity
    {
        [JsonProperty("ssg_websiteurl")]
        public string Website { get; set; }

        [JsonProperty("ssg_PersonId")]
        public virtual SSG_Person Person { get; set; }

        [JsonProperty("ssg_occupation")]
        [DisplayName("Occupation/Description")]
        public string Occupation { get; set; }

        [JsonProperty("ssg_notes")]
        [DisplayName("Notes")]
        public string Notes { get; set; }

        [JsonProperty("ssg_locationpostalcode")]
        [DisplayName("Postal Code")]
        public string PostalCode { get; set; }

        [JsonProperty("ssg_LocationCountry")]
        [DisplayName("Country")]
        public virtual SSG_Country Country { get; set; }

        [JsonProperty("ssg_locationcityname")]
        [DisplayName("City")]
        public string City { get; set; }

        [JsonProperty("ssg_suppliedby")]
        public int? InformationSource { get; set; }

        [JsonProperty("ssg_incomeassistancestatusoptionset")]
        public int? IncomeAssistanceStatusOption { get; set; }

        [JsonProperty("ssg_incomeassistancestatus")]
        [DisplayName("Income Assistance Status Text")]
        public string IncomeAssistanceStatus { get; set; }

        [JsonProperty("ssg_recordtype")]
        [DisplayName("Record Type")]
        [UpdateIgnore]
        public int EmploymentType { get; set; } = EmploymentRecordType.Employment.Value;

        [JsonProperty("ssg_employmentowner")]
        [DisplayName("Employment Owner")]
        public string BusinessOwner { get; set; }

        [JsonProperty("ssg_employmentconfirmed")]
        [DisplayName("Employment Confirmed")]
        public bool? EmploymentConfirmed { get; set; }

        [JsonProperty("ssg_countrytext")]
        [DisplayName("Country Text")]
        public string CountryText { get; set; }

        [JsonProperty("ssg_CountrySubDivision")]
        [DisplayName("Province/State")]
        public virtual SSG_CountrySubdivision CountrySubdivision { get; set; }

        [JsonProperty("ssg_countrysubdivision_text")]
        [DisplayName("Province/State Text")]
        public string CountrySubdivisionText { get; set; }

        [JsonProperty("ssg_contactname")]
        [DisplayName("Contact Name")]
        public string ContactPerson { get; set; }

        [JsonProperty("ssg_employerlegalname")]
        [DisplayName("Legal Name")]
        public string BusinessName { get; set; }

        [JsonProperty("ssg_employerdbaname")]
        public string DBAName { get; set; }

        [JsonProperty("ssg_primaryphonenumber")]
        [DisplayName("Primary Phone")]
        public string PrimaryPhoneNumber { get; set; }

        [JsonProperty("ssg_primaryphoneextension")]
        [DisplayName("Primary Phone Ext")]
        public string PrimaryPhoneExtension { get; set; }

        [JsonProperty("ssg_primaryfax")]
        [DisplayName("Primary Fax")]
        public string PrimaryFax { get; set; }

        [JsonProperty("ssg_address")]
        [DisplayName("Address Line 1")]
        public string AddressLine1 { get; set; }

        [JsonProperty("ssg_addresssecondaryunittext")]
        [DisplayName("Address Line 2")]
        public string AddressLine2 { get; set; }

        [JsonProperty("ssg_addresslinethree")]
        [DisplayName("Address Line 3")]
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

        [JsonProperty("ssg_agencyupdatedescription")]
        public string UpdateDetails { get; set; }

        [JsonProperty("ssg_couldnotlocate")]
        public bool CouldNotLocate { get; set; }
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
