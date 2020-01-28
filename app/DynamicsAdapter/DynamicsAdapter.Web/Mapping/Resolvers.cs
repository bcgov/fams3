using AutoMapper;
using DynamicsAdapter.Web.PersonSearch.Models;
using Fams3Adapter.Dynamics;
using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.Name;
using Fams3Adapter.Dynamics.Person;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.Mapping
{

    public class Date1Resolver : IValueResolver<PersonalInfo, DynamicsEntity, DateTime?>
    {
        public DateTime? Resolve(PersonalInfo source, DynamicsEntity dest, DateTime? date1, ResolutionContext context)
        {
            return source.ReferenceDates?.SingleOrDefault(m => m.Index == 0)?.Value.DateTime;
        }

    }
       

        public class Date1LabelResolver : IValueResolver<PersonalInfo, DynamicsEntity, string>
    {
        public string Resolve(PersonalInfo source, DynamicsEntity dest, string label, ResolutionContext context)
        {
            return source.ReferenceDates?.SingleOrDefault(m => m.Index == 0)?.Key;
        }
    }

    public class Date2Resolver : IValueResolver<PersonalInfo, DynamicsEntity, DateTime?>
    {
        public DateTime? Resolve(PersonalInfo source, DynamicsEntity dest, DateTime? date2, ResolutionContext context)
        {
            return source.ReferenceDates?.SingleOrDefault(m => m.Index == 1)?.Value.DateTime;
        }
    }

    public class Date2LabelResolver : IValueResolver<PersonalInfo, DynamicsEntity, string>
    {
        public string Resolve(PersonalInfo source, DynamicsEntity dest, string label, ResolutionContext context)
        {
            return source.ReferenceDates?.SingleOrDefault(m => m.Index == 1)?.Key;
        }
    }

  

    public class PersonalIdentifier_ReferenceDateResolver : IValueResolver<SSG_Identifier, PersonalIdentifier, ICollection<ReferenceDate>>
    {
        public ICollection<ReferenceDate> Resolve(SSG_Identifier source, PersonalIdentifier dest, ICollection<ReferenceDate> dates, ResolutionContext context)
        {
            List<ReferenceDate> referDates = new List<ReferenceDate>();
            if (source.Date1 != null)
            {
                referDates.Add(new ReferenceDate() { Index = 0, Key = source.Date1Label, Value = new DateTimeOffset((DateTime)source.Date1) });
            }
            if (source.Date2 != null)
            {
                referDates.Add(new ReferenceDate() { Index = 1, Key = source.Date2Label, Value = new DateTimeOffset((DateTime)source.Date2) });
            }
            return referDates;
        }
    }
}
