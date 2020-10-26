using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class RelatedPerson : PersonalInfo
    {
        [Description("The first name")]
        public string FirstName { get; set; }

        [Description("The last name")]
        public string LastName { get; set; }

        [Description("The middle name")]
        public string MiddleName { get; set; }

        [Description("The other name")]
        public string OtherName { get; set; }

        [Description("The gender of the related person")]
        public string Gender { get; set; }

        [Description("The birth date of the related person")]
        public DateTime? DateOfBirth { get; set; }

        [Description("The relationship type")]
        public string Type { get; set; }

        [Description("The related person type")]
        public string PersonType { get; set; }

        [Description("The death date of the related person")]
        public DateTime? DateOfDeath { get; set; }

    }
}
