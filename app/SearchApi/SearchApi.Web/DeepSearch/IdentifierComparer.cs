using BcGov.Fams3.SearchApi.Contracts.Person;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SearchApi.Web.DeepSearch
{
    public class IdentifierComparer : IEqualityComparer<PersonalIdentifier>
    {
        public bool Equals([DisallowNull] PersonalIdentifier x, [DisallowNull] PersonalIdentifier y)
        {
            return x.Type == y.Type && x.Value == y.Value;
        }

        public int GetHashCode([DisallowNull] PersonalIdentifier obj)
        {
            int hashType = obj.Type.GetHashCode();

            int hashValue = obj.Value.GetHashCode();

            return hashType ^ hashValue;
        }
    }

    public static class IdentifierExtensions
    {
        public static IEnumerable<PersonalIdentifier> DetailedCompare(this  IEnumerable<PersonalIdentifier> list1, IEnumerable<PersonalIdentifier> list2)
        {
            List<PersonalIdentifier> newIds = new List<PersonalIdentifier>();
            foreach (var item in list2)
                if (list1.Any(id => id.Value != item.Value))
                    newIds.Add(item);

          return  newIds.AsEnumerable();
        }

        public static IEnumerable<PersonalIdentifier> Merge(this IEnumerable<PersonalIdentifier> list1, IEnumerable<PersonalIdentifier> list2)
        {
            return list1.Union(list2);
        }


    }
}
