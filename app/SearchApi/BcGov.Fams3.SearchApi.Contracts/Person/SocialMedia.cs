using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class SocialMedia : PersonalInfo
    {

        [Description("the type of social media")]
        public string Type { get; set; }

        [Description("the social media account address")]
        public string AccountAddress { get; set; }

    }
}
