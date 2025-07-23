using System;
using System.Collections.Generic;
using System.Text;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class JCAPerson
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string MotherMaidName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Notes { get; set; }
        public string Gender { get; set; }
        public string SocialInsuranceNumber { get; set; }
        public string SubmitterCode { get; set; }
        public string ReasonCode { get; set; }
        public string SinInformation { get; set; }
        public string TaxIncomeInformation { get; set; }
        public string TracingInformation { get; set; }
        public string NoticeOfAssessment { get; set; }
        public string NoticeOfReassessment { get; set; }
        public string FinancialOtherIncome { get; set; }
        public string QuarterlyTracingUpdates { get; set; }
    }
}
