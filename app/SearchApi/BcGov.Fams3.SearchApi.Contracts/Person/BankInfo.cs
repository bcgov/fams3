using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class BankInfo : PersonalInfo
    {
        [Description("Bank name")]
        public string BankName { get; set; }

        [Description("The last name")]
        public string Branch { get; set; }

        [Description("The middle name")]
        public string TransitNumber { get; set; }

        [Description("The other name")]
        public string AccountNumber { get; set; }      
    }
}
