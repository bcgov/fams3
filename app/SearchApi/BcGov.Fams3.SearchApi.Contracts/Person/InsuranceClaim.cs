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

        [Description("Claim number")]
        public string ClaimNumber { get; set; }

        [Description("Adjustor")]
        public Name Adjustor { get; set; }

        [Description("Adjustor Phone")]
        public Phone AdjustorPhone { get; set; }

        [Description("Identifer involved in insurance claim")]
        public IEnumerable<PersonalIdentifier> Identifiers { get; set; }

        [Description("Parties involved in the insurance claim")]
        public IEnumerable<InvolvedParty> InsuredParties { get; set; }
        [Description("Claim center information")]
        public ClaimCentre ClaimCentre { get; set; }
    }

    
}
