using Fams3Adapter.Dynamics.Identifier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.PersonSearch.Models
{
    public class PersonalIdentifierActual : PersonalIdentifier
    {
    }
    public class PhoneNumberActual : PhoneNumber
    { }

    public class AddressActual : Web.Address
    { }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public IEnumerable<PersonalIdentifierActual> Identifiers { get; set; }

        public IEnumerable<PhoneNumberActual> PhoneNumbers { get; set; }
        public IEnumerable<AddressActual> Addresses { get; set; }

    }
}
