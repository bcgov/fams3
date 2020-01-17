using Fams3Adapter.Dynamics.Identifier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.PersonSearch.Models
{
    public class BaseActual
    {
        public IEnumerable<ReferenceDateActual> ReferenceDates { get; set; }
    }

    public class PersonalIdentifierActual : BaseActual
    {
        public string Value { get; set; }
        public PersonalIdentifierType Type { get; set; }
        public string TypeCode { get; set; }
        public string IssuedBy { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
    }

    public class PhoneNumberActual : BaseActual
    {
        public string SuppliedBy { get; set; }
        public DateTime? Date { get; set; }
        public string DateType { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumberType { get; set; }
    }

    public class AddressActual : BaseActual
    {
        public string Type { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string StateProvince { get; set; }
        public string City { get; set; }
        public string CountryRegion { get; set; }
        public string ZipPostalCode { get; set; }
        public string SuppliedBy { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class NameActual : BaseActual
    {
        public string Type { get; set; }

        public DateTime? EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public string Description { get; set; }
    }

    public class ReferenceDateActual 
    {
        public int Index { get; set; }
        public string Key { get; set; }
        public DateTime Value { get; set; }
    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public IEnumerable<PersonalIdentifierActual> Identifiers { get; set; }

        public IEnumerable<PhoneNumberActual> PhoneNumbers { get; set; }
        public IEnumerable<AddressActual> Addresses { get; set; }
        public IEnumerable<NameActual> Names { get; set; }
    }
}
