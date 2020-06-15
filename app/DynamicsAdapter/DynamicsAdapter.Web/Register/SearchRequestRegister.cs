using BcGov.Fams3.Redis;
using DynamicsAdapter.Web.Mapping;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Microsoft.Extensions.Logging;
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
        Task<bool> RemoveSearchApiRequest(Guid guid);
        Task<SSG_Identifier> GetMatchedSourceIdentifier(PersonalIdentifier identifer, Guid searchApiRequestId);
    }

    public class SearchRequestRegister : ISearchRequestRegister
    {
        private readonly ICacheService _cache;
        private readonly ILogger<SearchRequestRegister> _logger;

        public SearchRequestRegister(ICacheService cache, ILogger<SearchRequestRegister> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public SSG_SearchApiRequest FilterDuplicatedIdentifier(SSG_SearchApiRequest request)
        {
            SSG_Identifier[] uniqueIdentifers = request.Identifiers
                               .GroupBy(x => (x.Identification, x.IdentifierType, x.IssuedBy?.ToLower()))
                               .Select(y => y.First()).ToArray<SSG_Identifier>();

            request.Identifiers = uniqueIdentifers;
            return request;
        }

        public async Task<bool> RegisterSearchApiRequest(SSG_SearchApiRequest request)
        {
            if (request == null) return false;
            await _cache.Save($"{Keys.REDIS_KEY_PREFIX}{request.SearchRequest.FileId}_{request.SequenceNumber}", request);
            return true;
        }

        public async Task<SSG_SearchApiRequest> GetSearchApiRequest(string fileId, string sequenceNumber)
        {
            string data = await _cache.Get($"{Keys.REDIS_KEY_PREFIX}{fileId}_{sequenceNumber}");
            if (String.IsNullOrEmpty(data)) return null;
            return JsonConvert.DeserializeObject<SSG_SearchApiRequest>(data);
        }

        public async Task<SSG_SearchApiRequest> GetSearchApiRequest(Guid guid)
        {
            string data = await _cache.Get(Keys.REDIS_KEY_PREFIX + guid.ToString());
            if (String.IsNullOrEmpty(data)) return null;
            return JsonConvert.DeserializeObject<SSG_SearchApiRequest>(data);
        }

        public async Task<SSG_Identifier> GetMatchedSourceIdentifier(PersonalIdentifier identifer, Guid searchApiRequestId)
        {
            SSG_SearchApiRequest searchApiReqeust = await GetSearchApiRequest(searchApiRequestId);
            if (searchApiReqeust == null)
            {
                _logger.LogError("Cannot find the searchApiRequest in Redis Cache.");
                return null;
            }

            int? type = IDType.IDTypeDictionary.FirstOrDefault(m => m.Value == identifer.Type).Key;
            return searchApiReqeust.Identifiers.FirstOrDefault(
                m => m.Identification == identifer.Value
                     && m.IdentifierType == type);
        }

        public async Task<SSG_Identifier> GetMatchedSourceIdentifier(PersonalIdentifier identifer, string fileId, string sequenceNumber)
        {
            SSG_SearchApiRequest searchApiReqeust = await GetSearchApiRequest(fileId, sequenceNumber);
            if (searchApiReqeust == null)
            {
                _logger.LogError("Cannot find the searchApiRequest in Redis Cache.");
                return null;
            }

            int? type = IDType.IDTypeDictionary.FirstOrDefault(m => m.Value == identifer.Type).Key;
            return searchApiReqeust.Identifiers.FirstOrDefault(
                m => m.Identification == identifer.Value
                     && m.IdentifierType == type);
        }

        public async Task<bool> RemoveSearchApiRequest(Guid guid)
        {
            await _cache.Delete(Keys.REDIS_KEY_PREFIX + guid.ToString());
            return true;
        }

        public async Task<bool> RemoveSearchApiRequest(string fileId, string sequenceNumber)
        {
            await _cache.Delete($"{Keys.REDIS_KEY_PREFIX}{fileId}_{sequenceNumber}");
            return true;
        }
    }
}
