using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class CompensationClaim : PersonalInfo
    {
        [Description("Claim Type")]
        public string ClaimType { get; set; }

        [Description("Claim Status")]
        public string ClaimStatus { get; set; }

        [Description("claim number")]
        public string ClaimNumber { get; set; }

        [Description("Claimant number")]
        public string ClaimantNumber { get; set; }

        [Description("Bank information")]
        public BankInfo BankInfo { get; set; }

        [Description("Employer")]
        public Employer Employer { get; set; }
    }
}
