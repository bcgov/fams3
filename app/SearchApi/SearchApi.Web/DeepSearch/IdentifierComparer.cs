using BcGov.Fams3.SearchApi.Contracts.Person;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SearchApi.Web.DeepSearch
{
   

    public static class IdentifierExtensions
    {
        public static IEnumerable<PersonalIdentifier> DetailedCompare(this  IEnumerable<PersonalIdentifier> list1, IEnumerable<PersonalIdentifier> list2)
        {
            List<PersonalIdentifier> newIds = new List<PersonalIdentifier>();
            foreach (var item in list2)
                if (!list1.Any(id => id.Value == item.Value && id.Type == item.Type))
                    newIds.Add(item);
                

          return  newIds.AsEnumerable();
        }

        public static IEnumerable<PersonalIdentifier> Merge(this IEnumerable<PersonalIdentifier> list1, IEnumerable<PersonalIdentifier> list2)
        {
            return list1.Union(list2);
        }


    }
}
