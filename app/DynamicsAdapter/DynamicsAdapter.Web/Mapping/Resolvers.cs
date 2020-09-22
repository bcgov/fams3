using AutoMapper;
using Fams3Adapter.Dynamics;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.SearchApiRequest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicsAdapter.Web.Mapping
{

    public class Date1Resolver : IValueResolver<PersonalInfo, DynamicsEntity, DateTime?>
    {
        public DateTime? Resolve(PersonalInfo source, DynamicsEntity destination, DateTime? destMember, ResolutionContext context)
        {
            return source.ReferenceDates?.SingleOrDefault(m => m.Index == 0)?.Value.DateTime;
        }

    }


    public class Date1LabelResolver : IValueResolver<PersonalInfo, DynamicsEntity, string>
    {
        public string Resolve(PersonalInfo source, DynamicsEntity destination, string destMember, ResolutionContext context)
        {
            return source.ReferenceDates?.SingleOrDefault(m => m.Index == 0)?.Key;
        }
    }

    public class Date2Resolver : IValueResolver<PersonalInfo, DynamicsEntity, DateTime?>
    {
        public DateTime? Resolve(PersonalInfo source, DynamicsEntity destination, DateTime? destMember, ResolutionContext context)
        {
            return source.ReferenceDates?.SingleOrDefault(m => m.Index == 1)?.Value.DateTime;
        }
    }

    public class Date2LabelResolver : IValueResolver<PersonalInfo, DynamicsEntity, string>
    {
        public string Resolve(PersonalInfo source, DynamicsEntity destination, string destMember, ResolutionContext context)
        {
            return source.ReferenceDates?.SingleOrDefault(m => m.Index == 1)?.Key;
        }
    }



    public class PersonalIdentifier_ReferenceDateResolver : IValueResolver<IdentifierEntity, PersonalIdentifier, ICollection<ReferenceDate>>
    {
        public ICollection<ReferenceDate> Resolve(IdentifierEntity source, PersonalIdentifier destination, ICollection<ReferenceDate> destMember, ResolutionContext context)
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

    public class NamesResolver : IValueResolver<SSG_SearchApiRequest, PersonSearchRequest, ICollection<Name>>
    {
        public ICollection<Name> Resolve(SSG_SearchApiRequest source, PersonSearchRequest destination, ICollection<Name> destMember, ResolutionContext context)
        {
            if (source?.SearchRequest?.ApplicantFirstName != null || source?.SearchRequest?.ApplicantLastName != null)
                return new List<Name>() { new Name { FirstName = source?.SearchRequest?.ApplicantFirstName, LastName = source?.SearchRequest?.ApplicantLastName, Owner = OwnerType.Applicant } };
            else
                return null;
        }
    }
}
