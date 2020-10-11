using System;
using System.Collections.Generic;
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

        [Description("the date of birth of this name person")]
        public DateTime? DateOfBirth { get; set; }

        [Description("The owner of the name e.g. Applicant, Person Sought, e.t.c")]
        public OwnerType Owner { get; set; }

        public IEnumerable<PersonalIdentifier> Identifiers { get; set; }
        public IEnumerable<Address> Addresses { get; set; }
        public IEnumerable<Phone> Phones { get; set; }
    }
}
