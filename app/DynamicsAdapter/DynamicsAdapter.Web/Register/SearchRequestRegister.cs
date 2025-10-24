using BcGov.Fams3.Redis;
using DynamicsAdapter.Web.Mapping;
using DynamicsAdapter.Web.SearchAgency.Models;
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

        Task <bool> SearchForSearchRequestKeys(SSG_SearchApiRequest request);
        Task<bool> RegisterSearchApiRequest(SSG_SearchApiRequest request);

        Task<bool> RemoveSearchApiRequest(SSG_SearchApiRequest request);
        Task<SSG_SearchApiRequest> GetSearchApiRequest(Guid guid);

        Task<SSG_DataProvider[]> GetDataProvidersList();
        Task<SSG_SearchApiRequest> GetSearchApiRequest(string searchRequestKey);
        Task<bool> RemoveSearchApiRequest(Guid guid);
        Task<bool> RemoveSearchApiRequest(string searchRequestKey);
        Task<SSG_Identifier> GetMatchedSourceIdentifier(PersonalIdentifier identifer, string searchRequestKey);
        Task<SSG_Identifier> GetMatchedSourceIdentifier(PersonalIdentifier identifer, Guid searchApiRequestId);
        Task<bool> DataPartnerSearchIsComplete(string searchRequestKey);
        Task<bool> RegisterResponseApiCall(SearchResponseReady ready);
        Task<SearchResponseReady> GetSearchResponseReady(String fileId, string agencyFileId);
        Task<bool> DeleteSearchResponseReady(String fileId, string agencyFileId);
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
                               .Where(m=>(m.IsCouldNotLocate==false))
                               .GroupBy(x => (x.Identification, x.IdentifierType))                               
                               .Select(y => y.First()).ToArray<SSG_Identifier>();

            request.Identifiers = uniqueIdentifers;
            return request;
        }

        public async Task<bool> RegisterSearchApiRequest(SSG_SearchApiRequest request)
        {
            if (request == null)
            {
                _logger.LogDebug("RegisterSearchApiRequest called with a null request.");
                return false;
            }

            var key = $"{Keys.REDIS_KEY_PREFIX}{request.SearchRequest.FileId}_{request.SequenceNumber}";

            _logger.LogDebug(
                "Saving SearchApiRequest to redis cache with key: {Key}. Request summary: FileId={FileId}, SequenceNumber={SequenceNumber}, SearchRequestId={SearchRequestId}",
                key,
                request.SearchRequest?.FileId,
                request.SequenceNumber,
                request.SearchRequest?.SearchRequestId
            );

            await _cache.Save(key, request);
            return true;
        }

        public async Task<bool> SearchForSearchRequestKeys(SSG_SearchApiRequest request)
        {
            if (request == null)
            {
                _logger.LogDebug("SearchForSearchRequestKeys called with a null request.");
                return false;
            }

            var keyPattern = $"{Keys.REDIS_KEY_PREFIX}{request.SearchRequest.FileId}_{request.SequenceNumber}";

            _logger.LogDebug(
                "Searching cache for keys matching pattern: {KeyPattern}. Request summary: FileId={FileId}, SequenceNumber={SequenceNumber}, SearchRequestId={SearchRequestId}",
                keyPattern,
                request.SearchRequest?.FileId,
                request.SequenceNumber,
                request.SearchRequest?.SearchRequestId
            );

            var keys = await _cache.SearchKeys(keyPattern);
            var found = keys.Count() > 0;

            _logger.LogDebug(
                "SearchForSearchRequestKeys result: Found={FoundCount}, KeyPattern={KeyPattern}",
                keys.Count(),
                keyPattern
            );

            return found;
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
            if (string.IsNullOrEmpty(searchRequestKey))
            {
                _logger.LogDebug("GetSearchApiRequest called with an empty or null searchRequestKey.");
                return null;
            }

            var fullKey = $"{Keys.REDIS_KEY_PREFIX}{searchRequestKey}";
            _logger.LogDebug("Attempting to retrieve SearchApiRequest from cache with key: {FullKey}", fullKey);

            string data = await _cache.Get(fullKey);

            if (string.IsNullOrEmpty(data))
            {
                _logger.LogDebug("No data found in cache for key: {FullKey}", fullKey);
                return null;
            }

            try
            {
                var request = JsonConvert.DeserializeObject<SSG_SearchApiRequest>(data);
                _logger.LogDebug("Successfully deserialized SearchApiRequest for key: {FullKey}", fullKey);
                return request;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deserialize SearchApiRequest for key: {FullKey}", fullKey);
                return null;
            }
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
            _logger.LogDebug("GetMatchedSourceIdentifier started: searchRequestKey={SearchRequestKey}, identifier={Identifier}",
                           searchRequestKey, identifer != null ? identifer.Value : "null");

            SSG_SearchApiRequest searchApiRequest = await GetSearchApiRequest(searchRequestKey);
            
            if (searchApiRequest == null)
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
            return searchApiRequest.Identifiers.FirstOrDefault(
                m => m.Identification == identifer.Value
                     && m.IdentifierType == type);
        }

        public async Task<bool> RemoveSearchApiRequest(Guid guid)
        {
            if (guid == Guid.Empty)
            {
                _logger.LogDebug("RemoveSearchApiRequest called with an empty Guid.");
                return false;
            }

            var fullKey = $"{Keys.REDIS_KEY_PREFIX}{guid}";
            _logger.LogDebug("Attempting to remove SearchApiRequest from cache with key: {FullKey}", fullKey);

            await _cache.Delete(fullKey);

            return true;
        }

        public async Task<bool> RemoveSearchApiRequest(string searchRequestKey)
        {
            if (string.IsNullOrWhiteSpace(searchRequestKey))
            {
                _logger.LogDebug("RemoveSearchApiRequest called with an empty or null searchRequestKey.");
                return false;
            }

            var fullKey = $"{Keys.REDIS_KEY_PREFIX}{searchRequestKey}";
            _logger.LogDebug("Attempting to remove SearchApiRequest from cache with key: {FullKey}", fullKey);

            await _cache.Delete(fullKey);

            _logger.LogDebug("Successfully removed SearchApiRequest from cache with key: {FullKey}", fullKey);
            return true;
        }

        public async Task<bool> RemoveSearchApiRequest(SSG_SearchApiRequest request)
        {
            if (request == null)
            {
                _logger.LogDebug("RemoveSearchApiRequest called with a null request object.");
                return false;
            }

            if (request.SearchRequest == null)
            {
                _logger.LogDebug("RemoveSearchApiRequest called with a null SearchRequest inside the request object.");
                return false;
            }

            var fullKey = $"{Keys.REDIS_KEY_PREFIX}{request.SearchRequest.FileId}_{request.SequenceNumber}";
            _logger.LogDebug("Attempting to remove SearchApiRequest from cache with key: {FullKey}", fullKey);

            await _cache.Delete(fullKey);

            _logger.LogDebug("Successfully removed SearchApiRequest from cache with key: {FullKey}", fullKey);
            return true;
        }

        public async Task<bool> DataPartnerSearchIsComplete(string searchRequestKey)
        {
            string data = await _cache.GetString($"{searchRequestKey}");

            _logger.LogDebug($"DataPartnerSearchIsComplete : {data}");
            if (string.IsNullOrEmpty(data)) return true;
            IEnumerable<JToken> tokens = JObject.Parse(data).SelectTokens("$.DataPartners[?(@.Completed == false)]");
            return !tokens.Any();
        }

        public async Task<bool> RegisterResponseApiCall(SearchResponseReady ready)
        {
            if (ready == null)
            {
                _logger.LogDebug("RegisterResponseApiCall called with a null 'ready' object.");
                return false;
            }

            var fullKey = $"{Keys.REDIS_KEY_PREFIX}{Keys.REDIS_RESPONSE_KEY_PREFIX}{ready.FileId}_{ready.AgencyFileId}";
            _logger.LogDebug("Saving SearchResponseReady to cache with key: {FullKey}.", fullKey);

            await _cache.Save(fullKey, ready);

            _logger.LogDebug("Successfully saved SearchResponseReady to cache with key: {FullKey}", fullKey);
            return true;
        }

        public async Task<SearchResponseReady> GetSearchResponseReady(String fileId, string agencyFileId)
        {
            if (string.IsNullOrEmpty(fileId) || string.IsNullOrEmpty(agencyFileId))
            {
                _logger.LogDebug("GetSearchResponseReady called with invalid parameters. fileId: {FileId}, agencyFileId: {AgencyFileId}", fileId, agencyFileId);
                return null;
            }

            var fullKey = $"{Keys.REDIS_KEY_PREFIX}{Keys.REDIS_RESPONSE_KEY_PREFIX}{fileId}_{agencyFileId}";
            _logger.LogDebug("Attempting to retrieve SearchResponseReady from cache with key: {FullKey}", fullKey);

            string responseReadyStr = await _cache.Get(fullKey);

            if (string.IsNullOrEmpty(responseReadyStr))
            {
                _logger.LogDebug("No data found in cache for key: {FullKey}", fullKey);
                return null;
            }
    
            try
            {
                var response = JsonConvert.DeserializeObject<SearchResponseReady>(responseReadyStr);
                _logger.LogDebug("Successfully retrieved and deserialized SearchResponseReady for key: {FullKey}", fullKey);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deserialize SearchResponseReady from cache for key: {FullKey}.", fullKey);
                return null;
            }
        }

        public async Task<bool> DeleteSearchResponseReady(String fileId, string agencyFileId)
        {
            if (string.IsNullOrEmpty(fileId) || string.IsNullOrEmpty(agencyFileId))
            {
                _logger.LogDebug("DeleteSearchResponseReady called with invalid parameters. fileId: {FileId}, agencyFileId: {AgencyFileId}", fileId, agencyFileId);
                return false;
            }

            var fullKey = $"{Keys.REDIS_KEY_PREFIX}{Keys.REDIS_RESPONSE_KEY_PREFIX}{fileId}_{agencyFileId}";
            _logger.LogDebug("Attempting to delete SearchResponseReady from cache with key: {FullKey}", fullKey);

            await _cache.Delete(fullKey);

            _logger.LogDebug("Successfully deleted SearchResponseReady from cache with key: {FullKey}", fullKey);
            return true;
        }
    }
}
