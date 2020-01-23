using System;
using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class Name : PersonalInfo
    {
        [Description("The first name")]
        public string FirstName { get; set; }

        [Description("The last name")]
        public string LastName { get; set; }

        [Description("The middle name")]
        public string MiddleName { get; set; }

        [Description("The other name")]
        public string OtherName { get; set; }

        [Description("the type of the names")]
        public string Type { get; set; }

        

    }
}
