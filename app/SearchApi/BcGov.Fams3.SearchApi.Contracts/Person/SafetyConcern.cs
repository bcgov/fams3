using System;
using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class SafetyConcern : PersonalInfo
    {
        [Description("type")]
        public string Type { get; set; }

        [Description("detail")]
        public string Detail { get; set; }
    }
}
