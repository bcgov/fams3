using Newtonsoft.Json;
using System;

namespace Fams3Adapter.Dynamics.Pension
{
    public class PensionEntity : DynamicsEntity
    {

        [JsonProperty("ssg_name")]
        public string Provider { get; set; }

        [JsonProperty("ssg_providerphone")]
        public string ProviderPhone { get; set; }

        [JsonProperty("ssg_balanceamount_base")]
        public double BalanceAmount_base { get; set; }

        [JsonProperty("ssg_balanceamount")]
        public double BalanceAmount { get; set; }

        [JsonProperty("transactioncurrencyid")]
        public string Currency { get; set; }

        [JsonProperty("exchangerate")]
        public double ExchangeRate { get; set; }

        [JsonProperty("ssg_provideraddressline1")]
        public string AddressLine1 { get; set; }

        [JsonProperty("ssg_provideraddressline2")]
        public string AddressLine2 { get; set; }

        [JsonProperty("ssg_provideraddressline3")]
        public string AddressLine3 { get; set; }

        [JsonProperty("ssg_providercity")]
        public string City { get; set; }

        [JsonProperty("ssg_providerprovincestate")]
        public string ProvinceState { get; set; }

        [JsonProperty("ssg_providercountry")]
        public string Country { get; set; }

        [JsonProperty("ssg_providerpostalcode")]
        public string PostalCode { get; set; }
    }

    public class SSG_Asset_PensionDisablility : PensionEntity
    {
        [JsonProperty("ssg_asset_pensiondisabilityid")]
        public Guid PensionId { get; set; }
    }
}
