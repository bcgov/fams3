using Fams3Adapter.Dynamics.Identifier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.PersonSearch.Models
{
    public class PhoneNumberActual 
    {
        public string SuppliedBy { get; set; }
        public DateTime? Date { get; set; }
        public string DateType { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumberType { get; set; }
    }

    public class NameActual 
    {
        public string Type { get; set; }

        public DateTime? EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public string Description { get; set; }
    }


    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public IEnumerable<PersonalIdentifier> Identifiers { get; set; }

        public IEnumerable<PhoneNumberActual> PhoneNumbers { get; set; }
        public IEnumerable<Address> Addresses { get; set; }
        public IEnumerable<NameActual> Names { get; set; }
    }
}
