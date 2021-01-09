using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class Email : PersonalInfo
    {
        [Description("the type of email")]
        public string Type { get; set; }

        [Description("the social media account address")]
        public string EmailAddress { get; set; }

    }
}
