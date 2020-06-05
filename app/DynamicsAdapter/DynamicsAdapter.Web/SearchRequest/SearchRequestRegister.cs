using BcGov.Fams3.Redis;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.SearchApiRequest;
<<<<<<< HEAD
using Newtonsoft.Json;
=======
>>>>>>> master
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.SearchRequest
{
    public interface ISearchRequestRegister
    {
        SSG_SearchApiRequest FilterDuplicatedIdentifier(SSG_SearchApiRequest request);
        Task<bool> RegisterSearchApiRequest(SSG_SearchApiRequest request);
        Task<SSG_SearchApiRequest> GetSearchApiRequest(Guid guid);
    }

    public class SearchRequestRegister : ISearchRequestRegister
    {
        private readonly ICacheService _cache;

        public SearchRequestRegister(ICacheService cache)
        {
            _cache = cache;
        }

        public SSG_SearchApiRequest FilterDuplicatedIdentifier(SSG_SearchApiRequest request)
        {
            SSG_Identifier_WithGuid[] uniqueIdentifers = request.Identifiers
                               .GroupBy(x => (x.Identification, x.IdentifierType, x.IssuedBy.ToLower()))
                               .Select(y => y.First()).ToArray<SSG_Identifier_WithGuid>();

            request.Identifiers = uniqueIdentifers;
            return request;
        }

        public async Task<bool> RegisterSearchApiRequest(SSG_SearchApiRequest request)
        {
<<<<<<< HEAD
            if (request == null) return false;
            await _cache.Save(request.SearchApiRequestId.ToString(), request);
            return true;
=======
            throw new NotImplementedException();
>>>>>>> master
        }

        public async Task<SSG_SearchApiRequest> GetSearchApiRequest(Guid guid) 
        {
            if (guid == null) return null;
            string data = await _cache.Get(guid.ToString());
            return JsonConvert.DeserializeObject<SSG_SearchApiRequest>(data);
        }

    }
}
