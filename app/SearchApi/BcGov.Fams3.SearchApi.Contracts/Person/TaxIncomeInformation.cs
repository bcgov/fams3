using System;
using System.Collections.Generic;
using System.ComponentModel;


namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class TaxIncomeInformation : PersonalInfo
    {
        [Description("Tax Year of Result")]
        public string TaxYear { get; set; }

        [Description("Commission Income T4 Amount")]
        public string CommissionIncomeT4Amount { get; set; }

        [Description("Emergency Volunteer Exempt Income Amount")]
        public string EmergencyVolunteerExemptIncomeAmount { get; set; }

        [Description("Employment Income T4 Amount")]
        public string EmploymentIncomeT4Amount { get; set; }

        [Description("Date")]
        public DateTime Date { get; set; }

        [Description("JCA Code")]
        public string JCACode { get; set; }

        [Description("Text for the Trace Status Code")]
        public string TaxTraceStatusText { get; set; }
    }
}