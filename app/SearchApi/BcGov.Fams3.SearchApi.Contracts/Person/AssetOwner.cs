using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class AssetOwner
    {
        [Description("The asset owner type")]
        public string Type { get; set; }

        [Description("The asset owner description")]
        public string Description { get; set; }

        [Description("The asset owner organization name")]
        public string OrganizationName { get; set; }

        [Description("The asset owner first name")]
        public string FirstName { get; set; }

        [Description("The asset owner last name")]
        public string LastName { get; set; }

        [Description("The asset owner middle name")]
        public string MiddleName { get; set; }

        [Description("The asset owner other name")]
        public string OtherName { get; set; }

        [Description("The asset owner notes")]
        public string Notes { get; set; }
    }
}
