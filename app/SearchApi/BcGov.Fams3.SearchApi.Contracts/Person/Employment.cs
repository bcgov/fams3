using System;
using System.Collections.Generic;
using System.Text;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class Employment : PersonalInfo
    {
        public string IncomeAssistance { get; set; }

        public string IncomeAssistanceStatus { get; set; }

        public Employer Employer { get; set; }

        public string Occupation { get; set; }

    }
}
