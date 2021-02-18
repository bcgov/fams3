using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Newtonsoft.Json;
using System;

namespace Fams3Adapter.Dynamics.BankInfo
{
    public class BankingInformationEntity : DynamicsEntity
    {
        [JsonProperty("ssg_institutionname")]
        public string BankName { get; set; }

        [JsonProperty("ssg_branch")]
        public string Branch { get; set; }

        [JsonProperty("ssg_transitnumber")]
        public string TransitNumber { get; set; }

        [JsonProperty("ssg_accountnumber")]
        public string AccountNumber { get; set; }

        [JsonProperty("ssg_notes")]
        public string Notes { get; set; }

        [JsonProperty("ssg_suppliedby")]
        public int? InformationSource { get; set; }

        [JsonProperty("ssg_SearchRequest")]
        public virtual SSG_SearchRequest SearchRequest { get; set; }

        [JsonProperty("ssg_Person")]
        public virtual SSG_Person Person { get; set; }

        [JsonProperty("ssg_bankaccounttype")]
        public int? AccountType { get; set; }

        [JsonProperty("ssg_branchnumber")]
        public string BranchNumber { get; set; }

        [JsonProperty("ssg_couldnotlocate")]
        public bool CouldNotLocate { get; set; }
    }

    public class SSG_Asset_BankingInformation : BankingInformationEntity
    {
        [JsonProperty("ssg_asset_bankinginformationid")]
        public Guid BankingInformationId { get; set; }
    }
}
