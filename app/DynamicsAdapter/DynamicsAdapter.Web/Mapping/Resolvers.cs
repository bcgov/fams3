using AutoMapper;
using DynamicsAdapter.Web.PersonSearch.Models;
using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.Name;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.Mapping
{
    public class FullTextResolver : IValueResolver<Address, SSG_Address, string>
    {
        public string Resolve(Address source, SSG_Address dest, string fullText, ResolutionContext context)
        {
            return $"{source.AddressLine1} {source.AddressLine2} {source.AddressLine3} {source.City} {source.StateProvince} {source.CountryRegion} {source.ZipPostalCode}";
        }
    }

    public class FullNameResolver : IValueResolver<Name, SSG_Aliase, string>
    {
        public string Resolve(Name source, SSG_Aliase dest, string fullName, ResolutionContext context)
        {
            return $"{source.FirstName} {source.MiddleName} {source.LastName}";
        }
    }

    public class Identifier_Date1Resolver : IValueResolver<PersonalIdentifierActual, SSG_Identifier, DateTime?>
    {
        public DateTime? Resolve(PersonalIdentifierActual source, SSG_Identifier dest, DateTime? date1, ResolutionContext context)
        {
            return source.ReferenceDates?.SingleOrDefault(m => m.Index == 0) ?.Value.DateTime ;
        }
    }

    public class Identifier_Date1LabelResolver : IValueResolver<PersonalIdentifierActual, SSG_Identifier, string>
    {
        public string Resolve(PersonalIdentifierActual source, SSG_Identifier dest, string label, ResolutionContext context)
        {
            return source.ReferenceDates?.SingleOrDefault(m => m.Index == 0)?.Key;
        }
    }

    public class Identifier_Date2Resolver : IValueResolver<PersonalIdentifierActual, SSG_Identifier, DateTime?>
    {
        public DateTime? Resolve(PersonalIdentifierActual source, SSG_Identifier dest, DateTime? date1, ResolutionContext context)
        {
            return source.ReferenceDates?.SingleOrDefault(m => m.Index == 1)?.Value.DateTime;
        }
    }

    public class Identifier_Date2LabelResolver : IValueResolver<PersonalIdentifierActual, SSG_Identifier, string>
    {
        public string Resolve(PersonalIdentifierActual source, SSG_Identifier dest, string label, ResolutionContext context)
        {
            return source.ReferenceDates?.SingleOrDefault(m => m.Index == 1)?.Key;
        }
    }
}
