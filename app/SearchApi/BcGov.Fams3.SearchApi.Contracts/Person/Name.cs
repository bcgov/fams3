using System;
using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class Name 
    {
        [Description("The first name")]
        public string FirstName { get; set; }

        [Description("The last name")]
        public string LastName { get; set; }

        [Description("The middle name")]
        public string MiddleName { get; set; }

        [Description("the type of the names")]
        public string Type { get; set; }

        [Description("the name effective date")]
        public DateTime? EffectiveDate { get; set; }

        [Description("The name end date")]
        public DateTime? EndDate { get; set; }

        [Description("The name description")]
        public string Description { get; set; }
    }
}
