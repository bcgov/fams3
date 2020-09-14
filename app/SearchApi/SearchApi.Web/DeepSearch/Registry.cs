using BcGov.Fams3.SearchApi.Contracts.Person;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SearchApi.Web.DeepSearch
{
    public static class Registry
    {
        public static readonly ReadOnlyDictionary<string, List<PersonalIdentifierType>> DataPartnerParameters = new ReadOnlyDictionary<string,List<PersonalIdentifierType>>(new Dictionary<string, List<PersonalIdentifierType>>()
        {
            {"ICBC", new List<PersonalIdentifierType>(){PersonalIdentifierType.PersonalHealthNumber, PersonalIdentifierType.BCDriverLicense } },
            {"BCHYDRO", new List<PersonalIdentifierType>(){PersonalIdentifierType.BCDriverLicense, PersonalIdentifierType.BCHydroBP } },
            {"MSDPR", new List<PersonalIdentifierType>(){PersonalIdentifierType.SocialInsuranceNumber } },
            {"WORKSAFEBC", new List<PersonalIdentifierType>(){PersonalIdentifierType.PersonalHealthNumber, PersonalIdentifierType.SocialInsuranceNumber } }
        });
    }
}
