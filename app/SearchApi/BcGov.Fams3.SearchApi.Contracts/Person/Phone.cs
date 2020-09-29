using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class Phone : PersonalInfo
    {
        [Description("The Phone number")]
        public string PhoneNumber { get; set; }
        [Description("The extension number")]
        public string Extension { get; set; }
        [Description("The phone number type")]
        public string Type { get; set; }

        [Description("The owner of the phone e.g. Applicant, Person Sought, e.t.c")]
        public OwnerType Owner { get; set; }

        [Description("The contact person name for this phone number ")]
        public string ContactName { get; set; }
    }
}
