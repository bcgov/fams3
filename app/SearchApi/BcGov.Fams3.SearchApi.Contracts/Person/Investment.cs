using System;
using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class Investment : PersonalInfo
    {
        [Description("Account Number")]
        public string AccountNumber { get; set; }

        [Description("Company")]
        public string Company { get; set; }

        [Description("Location")]
        public string Location { get; set; }

        [Description("Balance Amount")]
        public string BalanceAmount { get; set; }

        [Description("Type of Investment")]
        public string Type { get; set; }

        [Description("Maturity Date")]
        public DateTime? MaturityDate { get; set; }
    }
}
