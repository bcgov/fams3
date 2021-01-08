using System.Collections.Generic;
using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class Employer
    {
        [Description("The name of the employer")]
        public string Name { get; set; }

        [Description("The name of the owner of the employer")]
        public string OwnerName { get; set; }
        [Description("the phone numbers and fax of the company")]
        public IEnumerable<Phone> Phones { get; set; }

        [Description("The address of the employer")]
        public Address Address { get; set; }

        [Description("The full name  of the contact")]
        public string ContactPerson { get; set; }

        [Description("The Employer DBA name")]
        public string DBAName { get; set; }

        public IEnumerable<EmployerContact> EmployerContacts { get; set; }
    }

    public class EmployerContact
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public Phone Phone { get; set; }
        public string Fax { get; set; }
        public string Type { get; set; }
    }
}
