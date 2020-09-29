using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class ResponsePerson : PersonalInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string OtherName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime? DateOfDeath { get; set; }

        public string Gender { get; set; }
        public bool? DateDeathConfirmed { get; set; }
        public string Incacerated { get; set; }

        [Description("Height is in centimers")]
        public string Height { get; set; }

        [Description("Weight is in pounds")]
        public string Weight { get; set; }
        public string HairColour { get; set; }
        public string EyeColour { get; set; }
        public string Complexion { get; set; }
        public string DistinguishingFeatures { get; set; }
        public string WearGlasses { get; set; }
    }
}
