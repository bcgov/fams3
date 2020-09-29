using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Newtonsoft.Json;
using System;

namespace Fams3Adapter.Dynamics.Investment
{
    public class InvestmentEntity : DynamicsEntity
    {
        [JsonProperty("ssg_name")]
        public string AccountNumber { get; set; }

        [JsonProperty("ssg_company")]
        public string Company { get; set; }

        [JsonProperty("ssg_location")]
        public string Location { get; set; }

        [JsonProperty("ssg_balanceamount")]
        public string BalanceAmount { get; set; }

        [JsonProperty("ssg_typeofinvestment")]
        public string Type { get; set; }

        [JsonProperty("ssg_maturitydate")]
        public DateTime? MaturityDate { get; set; }
    }

    public class SSG_Asset_Investment : InvestmentEntity
    {
        [JsonProperty("ssg_asset_investmentid")]
        public Guid InvestmentId { get; set; }
    }
}
