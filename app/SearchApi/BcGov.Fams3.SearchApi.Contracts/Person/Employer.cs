using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
   public  class Employer
    {
        [Description("The name of the employer")]
        public string Name { get; set; }

        [Description("The name of the owner of the employer")]
        public string OwnerName { get; set; }

        [Description("The phone number of the employer")]
        public string PhoneNumber { get; set; }

        [Description("The extension to the phone number of the employer")]
        public string PhoneExtension { get; set; }

        [Description("The Fax number of the employer")]
        public string FaxNumber { get; set; }

        [Description("The extension to the fax number of the employer")]
        public string FaxExtension { get; set; }

        [Description("The address of the employer")]
        public Address Address { get; set; }

        [Description("The full name  of the contact")]
        public string ContactPerson  { get; set; }
    }
}
