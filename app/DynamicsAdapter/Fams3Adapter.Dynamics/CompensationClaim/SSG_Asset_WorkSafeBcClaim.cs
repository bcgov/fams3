using Fams3Adapter.Dynamics.BankInfo;
using Fams3Adapter.Dynamics.Employment;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Newtonsoft.Json;
using System;

namespace Fams3Adapter.Dynamics.CompensationClaim
{
    public class CompensationClaimEntity : DynamicsEntity
    {
        [JsonProperty("ssg_suppliertypecode")]
        public string ClaimType { get; set; }

        [JsonProperty("ssg_claimstatus")]
        public string ClaimStatus { get; set; }

        [JsonProperty("ssg_name")]
        public string ClaimNumber { get; set; }

        [JsonProperty("ssg_claimantnumber")]
        public string ClaimantNumber { get; set; }

        [JsonProperty("ssg_notes")]
        public string Notes { get; set; }

        [JsonProperty("ssg_description")]
        public string Description { get; set; }

        [JsonProperty("ssg_suppliedby")]
        public int? InformationSource { get; set; }

        [JsonProperty("ssg_SearchRequest")]
        public virtual SSG_SearchRequest SearchRequest { get; set; }

        [JsonProperty("ssg_PersonId")]
        public virtual SSG_Person Person { get; set; }

        [JsonProperty("ssg_BankingInformationId")]
        public virtual SSG_Asset_BankingInformation BankingInformation { get; set; }

        [JsonProperty("ssg_Employment")]
        public virtual SSG_Employment Employment { get; set; }

        public EmploymentEntity EmploymentEntity { get; set; }
        public BankingInformationEntity BankInformationEntity { get; set; }
    }



public class SSG_Asset_WorkSafeBcClaim : CompensationClaimEntity
    {
        [JsonProperty("ssg_asset_worksafebcclaimid")]
        public Guid CompensationClaimId{ get; set; }
        public bool IsDuplicated { get; set; }
    }
}
