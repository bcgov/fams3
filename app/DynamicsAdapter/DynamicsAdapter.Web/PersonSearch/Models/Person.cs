using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.PersonSearch.Models
{
    // TODO: all classes bellow will be coming from SEARCH API CORE LIB
   

    public enum PersonIDKind
    {
        DriverLicense
    }

    public class PersonId
    {
        public PersonIDKind Kind { get; set; }
        public string Issuer { get; set; }
        public string Number { get; set; }
    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
