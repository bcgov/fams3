using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Update;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Fams3Adapter.Dynamics.TaxIncomeInformation
{
    public class TaxIncomeInformationEntity : DynamicsEntity
    {
        [JsonProperty("ssg_name")]
        [Description("Tax Year")]
        public string TaxYear { get; set; }

        [JsonProperty("ssg_taxyear")]
        [Description("Tax Year of Result")]
        public string TaxYearResult { get; set; }

        [JsonProperty("ssg_commissionincomet4amount")]
        [Description("Commission Income T4 Amount")]
        public string CommissionIncomeT4Amount { get; set; }

        [JsonProperty("ssg_emergencyvolunteerexemptincomeamount")]
        [Description("Emergency Volunteer Exempt Income Amount")]
        public string EmergencyVolunteerExemptIncomeAmount { get; set; }

        [JsonProperty("ssg_employmentincomet4amount")]
        [Description("Employment Income T4 Amount")]
        public string EmploymentIncomeT4Amount { get; set; }

        [JsonProperty("ssg_jcacode")]
        [Description("JCA Code")]
        public string JCACode { get; set; }

        [JsonProperty("ssg_notes")] // This is saved to Data Provider - Notes
        [Description("Text for the Trace Status Code")]
        public string TaxTraceStatusText { get; set; }

        [JsonProperty("ssg_suppliedby")]
        public int? InformationSource { get; set; }

        [UpdateIgnore]
        [JsonProperty("ssg_createdbyagency")]
        public bool IsCreatedByAgency { get; set; }

        [JsonProperty("ssg_agencyupdatedescription")]
        public string UpdateDetails { get; set; }

        [JsonProperty("ssg_searchrequest")]
        public virtual SSG_SearchRequest SearchRequest { get; set; }

        [JsonProperty("ssg_Personid")]
        public virtual SSG_Person Person { get; set; }

        [JsonProperty("ssg_personname")]
        public string FullName { get; set; }

        [JsonProperty("fam_cracode")]
        [Description("CRA Code")]
        public string TaxCode { get; set; }

        [JsonProperty("ssg_description")]
        public string Description { get; set; }

        [JsonProperty("fams_taxamount")]
        [Description("Tax Amount")]
        public string TaxAmount { get; set; }
    }

    public class SSG_TaxIncomeInformation : TaxIncomeInformationEntity
    {
        [JsonProperty("ssg_taxincomeinformationid")]
        public Guid TaxincomeinformationId { get; set; }

    }
}

