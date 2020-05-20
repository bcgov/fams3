using System.Collections.Generic;
using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public  class InsuranceClaim : PersonalInfo
    {

        [Description("Claim Type")]
        public string ClaimType { get; set; }

        [Description("Claim Status")]
        public string ClaimStatus { get; set; }

        [Description("claim number")]
        public string ClaimNumber { get; set; }

        [Description("claim center")]
        public string ClaimCenter { get; set; }

        [Description("Adjustor")]
        public Name Adjustor { get; set; }

        [Description("Adjustor Phone")]
        public Phone AdjustorPhone { get; set; }

        [Description("Identifer involved in insurance claim")]
        public IEnumerable<PersonalIdentifier> Identifiers { get; set; }

        [Description("Person involved")]
        public Name InsuredPerson { get; set; }



    }
}
