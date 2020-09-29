using System;
using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class Pension : PersonalInfo
    {
        [Description("Pension Provider")]
        public string Provider { get; set; }

        [Description("Phone Number")]
        public string ProviderPhone { get; set; }

        [Description("Balance Amount (Base)")]
        public string BalanceAmount_base { get; set; }

        [Description("Balance Amount")]
        public string BalanceAmount { get; set; }

        [Description("Currency")]
        public string Currency { get; set; }

        [Description("Exchange Rate")]
        public string ExchangeRate { get; set; }

        public Address ProviderAddress { get; set; }
    }
}
