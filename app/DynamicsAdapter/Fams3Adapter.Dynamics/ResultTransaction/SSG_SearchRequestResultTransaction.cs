﻿using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.BankInfo;
using Fams3Adapter.Dynamics.CompensationClaim;
using Fams3Adapter.Dynamics.Email;
using Fams3Adapter.Dynamics.Employment;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.InsuranceClaim;
using Fams3Adapter.Dynamics.Name;
using Fams3Adapter.Dynamics.OtherAsset;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.PhoneNumber;
using Fams3Adapter.Dynamics.RelatedPerson;
using Fams3Adapter.Dynamics.SafetyConcern;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Fams3Adapter.Dynamics.TaxIncomeInformation;
using Fams3Adapter.Dynamics.FinancialOtherIncome;
using Fams3Adapter.Dynamics.Vehicle;
using Newtonsoft.Json;

namespace Fams3Adapter.Dynamics.ResultTransaction
{
    public class SSG_SearchRequestResultTransaction
    {
        [JsonProperty("ssg_Address")]
        public virtual SSG_Address Address { get; set; }

        [JsonProperty("ssg_EmailAddress")]
        public virtual SSG_Email Email { get; set; }

        [JsonProperty("ssg_Alias")]
        public virtual SSG_Aliase Name { get; set; }

        [JsonProperty("ssg_Person")]
        public virtual SSG_Person Person { get; set; }

        [JsonProperty("ssg_SourceIdentifier")]
        public virtual SSG_Identifier SourceIdentifier { get; set; }

        [JsonProperty("ssg_ResultIdentifier")]
        public virtual SSG_Identifier ResultIdentifier { get; set; }

        [JsonProperty("ssg_BankingInformation")]
        public virtual SSG_Asset_BankingInformation BankInfo { get; set; }

        [JsonProperty("ssg_Employer")]
        public virtual SSG_Employment Employment { get; set; }

        [JsonProperty("ssg_ICBCClaim")]
        public virtual SSG_Asset_ICBCClaim InsuranceClaim { get; set; }

        [JsonProperty("ssg_OtherAsset")]
        public virtual SSG_Asset_Other OtherAsset { get; set; }

        [JsonProperty("ssg_PhoneNumber")]
        public virtual SSG_PhoneNumber PhoneNumber { get; set; }

        [JsonProperty("ssg_RelatedPerson")]
        public virtual SSG_Identity RelatedPerson { get; set; }

        [JsonProperty("ssg_Vehicle")]
        public virtual SSG_Asset_Vehicle Vehicle { get; set; }

        [JsonProperty("ssg_WorkSafeBCClaim")]
        public virtual SSG_Asset_WorkSafeBcClaim CompensationClaim { get; set; }

        [JsonProperty("ssg_SafetyConcernDetail")]
        public virtual SSG_SafetyConcernDetail SafetyConcern { get; set; }

        [JsonProperty("ssg_taxincomeinformation")]
        public virtual SSG_TaxIncomeInformation Taxincomeinformation { get; set; }
        
        [JsonProperty("ssg_SearchAPIRequest")]
        public virtual SSG_SearchApiRequest SearchApiRequest { get; set; }

        [JsonProperty("ssg_suppliedby")]
        public int? InformationSource { get; set; }

        [JsonProperty("ssg_name")]
        public string ResultName { get; set; }

        [JsonProperty("ssg_notes")]
        public string Notes { get; set; }

        [JsonProperty("fams_financialotherincome")]
        public virtual FAMS_FinancialOtherIncome FinancialOtherIncome { get; set; }
    }
}
