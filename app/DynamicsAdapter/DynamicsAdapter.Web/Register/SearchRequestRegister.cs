using BcGov.Fams3.Redis;
using DynamicsAdapter.Web.Mapping;
using Fams3Adapter.Dynamics.DataProvider;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.Register
{
    public interface ISearchRequestRegister
    {
        SSG_SearchApiRequest FilterDuplicatedIdentifier(SSG_SearchApiRequest request);
        Task<bool> RegisterSearchApiRequest(SSG_SearchApiRequest request);
        Task<SSG_SearchApiRequest> GetSearchApiRequest(Guid guid);

        Task<SSG_DataProvider[]> GetDataProvidersList();
        Task<SSG_SearchApiRequest> GetSearchApiRequest(string searchRequestKey);
        Task<bool> RemoveSearchApiRequest(Guid guid);
        Task<bool> RemoveSearchApiRequest(string searchRequestKey);
        Task<SSG_Identifier> GetMatchedSourceIdentifier(PersonalIdentifier identifer, string searchRequestKey);
        Task<SSG_Identifier> GetMatchedSourceIdentifier(PersonalIdentifier identifer, Guid searchApiRequestId);

        Task<bool> DataPartnerSearchIsComplete(string searchRequestKey);
    }

    public class SearchRequestRegister : ISearchRequestRegister
    {
        private readonly ICacheService _cache;
        private readonly ILogger<SearchRequestRegister> _logger;
        private readonly IDataPartnerService _dataProviderService;

        public SearchRequestRegister(ICacheService cache, ILogger<SearchRequestRegister> logger, IDataPartnerService dataProviderService)
        {
            _cache = cache;
            _logger = logger;
            this._dataProviderService = dataProviderService;
        }

        public SSG_SearchApiRequest FilterDuplicatedIdentifier(SSG_SearchApiRequest request)
        {
            SSG_Identifier[] uniqueIdentifers = request.Identifiers
                               .GroupBy(x => (x.Identification, x.IdentifierType))
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

        public async Task<bool> RegisterDataProviders(SSG_DataProvider[] providers)
        {
            if (providers == null) return false;
            await _cache.Save($"{Keys.REDIS_KEY_PREFIX}{Keys.DATA_PROVIDER_KEY}", providers, new TimeSpan(24, 0, 10));
            return true;
        }

        public async Task<SSG_DataProvider[]> GetDataProvidersList()
        {
          
            string data = await _cache.Get($"{Keys.REDIS_KEY_PREFIX}{Keys.DATA_PROVIDER_KEY}");
            if (string.IsNullOrEmpty(data))
            {

                var providers = await _dataProviderService.GetAllDataProviders(new CancellationTokenSource().Token);

                await RegisterDataProviders(providers.ToArray());

                return providers.ToArray();
            }
            return JsonConvert.DeserializeObject<SSG_DataProvider[]>(data);
           
        }

        public async Task<SSG_SearchApiRequest> GetSearchApiRequest(string searchRequestKey)
        {
            string data = await _cache.Get($"{Keys.REDIS_KEY_PREFIX}{searchRequestKey}");
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
            if (identifer == null)
            {
                _logger.LogDebug("source identifier from personfound is null");
                return null;
            }

            int? type = IDType.IDTypeDictionary.FirstOrDefault(m => m.Value == identifer.Type).Key;
            return searchApiReqeust.Identifiers.FirstOrDefault(
                m => m.Identification == identifer.Value
                     && m.IdentifierType == type);
        }

        public async Task<SSG_Identifier> GetMatchedSourceIdentifier(PersonalIdentifier identifer, string searchRequestKey)
        {
            SSG_SearchApiRequest searchApiReqeust = await GetSearchApiRequest(searchRequestKey);
            if (searchApiReqeust == null)
            {
                _logger.LogError("Cannot find the searchApiRequest in Redis Cache.");
                return null;
            }
            if (identifer == null)
            {
                _logger.LogDebug("source identifier from personfound is null");
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

        public async Task<bool> RemoveSearchApiRequest(string searchRequestKey)
        {
            await _cache.Delete($"{Keys.REDIS_KEY_PREFIX}{searchRequestKey}");
            return true;
        }

        public async Task<bool> DataPartnerSearchIsComplete(string searchRequestKey)
        {
            string data = await _cache.Get($"{searchRequestKey}");
            if (string.IsNullOrEmpty(data)) return true;
            IEnumerable<JToken> tokens = JObject.Parse(data).SelectTokens("$.DataPartners[?(@.Completed == false)].Completed");
            return !tokens.Any();
        }
    }
}
