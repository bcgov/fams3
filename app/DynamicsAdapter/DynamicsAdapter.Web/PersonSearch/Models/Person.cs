using Fams3Adapter.Dynamics.Identifier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.PersonSearch.Models
{
    // TODO: all classes bellow will be coming from SEARCH API CORE LIB
    public enum PersonalIdentifierType
    {
        DriverLicense,
        SocialInsuranceNumber,
        PersonalHealthNumber,
        BirthCertificate,
        CorrectionsId,
        NativeStatusCard,
        Passport,
        WcbClaim,
        Other,
        SecurityKeyword
    }

    public class PersonalIdentifier
    {
        public string SerialNumber { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public PersonalIdentifierType Type { get; set; }
        public string IssuedBy { get; set; }
    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public IEnumerable<PersonalIdentifier> Identifiers { get; set; }
        public IEnumerable<Address> Addresses { get; set; }
    }
}
