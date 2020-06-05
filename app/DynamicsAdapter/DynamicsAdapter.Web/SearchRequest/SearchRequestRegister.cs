using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.SearchApiRequest;
using System;
using System.Linq;

namespace DynamicsAdapter.Web.SearchRequest
{
    public interface ISearchRequestRegister
    {
        SSG_SearchApiRequest FilterDuplicatedIdentifier(SSG_SearchApiRequest request);
        bool RegisterSearchRequest(SSG_SearchApiRequest request);
    }

    public class SearchRequestRegister : ISearchRequestRegister
    {
        public SSG_SearchApiRequest FilterDuplicatedIdentifier(SSG_SearchApiRequest request)
        {
            SSG_Identifier_WithGuid[] uniqueIdentifers = request.Identifiers
                               .GroupBy(x => (x.Identification, x.IdentifierType, x.IssuedBy.ToLower()))
                               .Select(y => y.First()).ToArray<SSG_Identifier_WithGuid>();

            request.Identifiers = uniqueIdentifers;
            return request;
        }

        public bool RegisterSearchRequest(SSG_SearchApiRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
