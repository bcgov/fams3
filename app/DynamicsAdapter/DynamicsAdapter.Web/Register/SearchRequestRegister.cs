using BcGov.Fams3.Redis;
using DynamicsAdapter.Web.Mapping;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.Register
{
    public interface ISearchRequestRegister
    {
        SSG_SearchApiRequest FilterDuplicatedIdentifier(SSG_SearchApiRequest request);
        Task<bool> RegisterSearchApiRequest(SSG_SearchApiRequest request);
        Task<SSG_SearchApiRequest> GetSearchApiRequest(Guid guid);
        Task<SSG_Identifier> GetMatchedSourceIdentifier(PersonalIdentifier identifer, Guid searchApiRequestId);
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
            SSG_Identifier[] uniqueIdentifers = request.Identifiers
                               .GroupBy(x => (x.Identification, x.IdentifierType, x.IssuedBy.ToLower()))
                               .Select(y => y.First()).ToArray<SSG_Identifier>();

            request.Identifiers = uniqueIdentifers;
            return request;
        }

        public async Task<bool> RegisterSearchApiRequest(SSG_SearchApiRequest request)
        {
            if (request == null) return false;
            await _cache.Save(request.SearchApiRequestId.ToString(), request);
            return true;
        }

        public async Task<SSG_SearchApiRequest> GetSearchApiRequest(Guid guid) 
        {
            string data = await _cache.Get(guid.ToString());
            if (String.IsNullOrEmpty(data)) return null;
            return JsonConvert.DeserializeObject<SSG_SearchApiRequest>(data);
        }

        public async Task<SSG_Identifier> GetMatchedSourceIdentifier(PersonalIdentifier identifer, Guid searchApiRequestId)
        {
            SSG_SearchApiRequest searchApiReqeust = await GetSearchApiRequest(searchApiRequestId);
            return searchApiReqeust.Identifiers.FirstOrDefault(
                m => m.Identification == identifer.Value
                     && m.IdentifierType == IDType.IDTypeDictionary.FirstOrDefault(m => m.Value == identifer.Type).Key);
        }  
    }
}
