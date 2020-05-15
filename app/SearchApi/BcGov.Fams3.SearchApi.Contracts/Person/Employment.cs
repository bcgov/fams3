using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class Employment : PersonalInfo
    {
        [Description("employment confirmation ")]
        public bool? EmploymentConfirmed { get; set; }

        [Description("income assistant ")]
        public bool? IncomeAssistance { get; set; }
        [Description("income assistant status")]
        public string IncomeAssistanceStatus { get; set; }
        [Description("employer")]
        public Employer Employer { get; set; }
        [Description("person occupation")]
        public string Occupation { get; set; }
        [Description("company website")]
        public string Website { get; set; }


    }
}
