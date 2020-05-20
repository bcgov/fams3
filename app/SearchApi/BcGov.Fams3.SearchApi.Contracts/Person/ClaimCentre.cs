using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class ClaimCentre
    {
        [Description("Location code")]
        public string Location { get; set; }

        [Description("Contact address")]
        public Address ContactAddress { get; set; }

        [Description("Contact address")]
        public List<Phone> ContactNumber { get; set; }

      
    }
}
