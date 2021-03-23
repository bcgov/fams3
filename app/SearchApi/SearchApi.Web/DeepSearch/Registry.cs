using BcGov.Fams3.SearchApi.Contracts.Person;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SearchApi.Web.DeepSearch
{
    public static class Registry
    {
        public static readonly ReadOnlyDictionary<string, PersonalIdentifierType[]> DataPartnerParameters = new ReadOnlyDictionary<string, PersonalIdentifierType[]>(new Dictionary<string, PersonalIdentifierType[]>()
        {
            {"ICBC", new PersonalIdentifierType[]{ PersonalIdentifierType.PersonalHealthNumber, PersonalIdentifierType.BCDriverLicense } },
            {"BCHYDRO", new PersonalIdentifierType[]{PersonalIdentifierType.BCDriverLicense, PersonalIdentifierType.BCHydroBP } },
            {"MSDPR", new PersonalIdentifierType[]{PersonalIdentifierType.SocialInsuranceNumber } },
            {"WORKSAFEBC",new PersonalIdentifierType[]{PersonalIdentifierType.PersonalHealthNumber, PersonalIdentifierType.SocialInsuranceNumber, PersonalIdentifierType.WorkSafeBCCCN } },
            {"MH_HCIM", new PersonalIdentifierType[]{PersonalIdentifierType.PersonalHealthNumber } },
            {"MH_RAPIDR", new PersonalIdentifierType[]{PersonalIdentifierType.PersonalHealthNumber } },
            {"MH_RAPIDE", new PersonalIdentifierType[]{PersonalIdentifierType.PersonalHealthNumber } },
            {"CORNET", new PersonalIdentifierType[]{ PersonalIdentifierType.CorrectionsId, PersonalIdentifierType.PersonalHealthNumber, PersonalIdentifierType.BCDriverLicense } }
        });
    }
}
