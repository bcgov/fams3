
using BcGov.Fams3.Redis.Model;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BcGov.Fams3.Redis
{
    public interface ICacheService
    {
        Task SaveRequest(SearchRequest searchRequest);
        Task SaveString(string data, string key);
        Task SaveString(string data, string key, TimeSpan expiry);
        Task<string> GetString(string key);
        Task<SearchRequest> GetRequest(string searchRequestKey);
        Task DeleteRequest(string searchRequestKey);
        Task<IEnumerable<string>> SearchKeys(string pattern);
        Task Save(string key, dynamic data, TimeSpan expiry);

        Task Save(string key, dynamic data);
        Task Update(string key, dynamic data);
        Task<string> Get(string key);
        Task Delete(string key);
        Task<int> UpdateDataPartnerCompleteStatus(string searchRequestkey,string dataPartner);
    }

    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRedisCacheClient _stackRedisCacheClient;
        private readonly ILogger<CacheService> _logger;

        public CacheService(IDistributedCache distributedCache, IRedisCacheClient stackRedisCacheClient, ILogger<CacheService> logger)
        {
            _distributedCache = distributedCache;
            _stackRedisCacheClient = stackRedisCacheClient;
            _logger = logger;
        }

        public async Task SaveRequest(SearchRequest searchRequest)
        {           
            if (searchRequest is null) throw new ArgumentNullException("SaveRequest : Search request cannot be null");
            if (string.IsNullOrEmpty(searchRequest.SearchRequestKey)) throw new ArgumentNullException("SaveRequest : Search request key cannot be null");
            await _distributedCache.SetStringAsync(searchRequest.SearchRequestKey, JsonConvert.SerializeObject(searchRequest), new CancellationToken());


        }

        public async Task SaveString(string data, string key)
        {
            if (string.IsNullOrEmpty(data)) throw new ArgumentNullException("SaveRequest : Data cannot be empty");
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("SaveRequest : Key cannot be null");
            await _distributedCache.SetStringAsync(key, data, new CancellationToken());

        }
        public async Task SaveString(string data, string key, TimeSpan expiry)
        {
            var options = new DistributedCacheEntryOptions();
            options.SetAbsoluteExpiration(expiry);
            if (string.IsNullOrEmpty(data)) throw new ArgumentNullException("SaveRequest : Data cannot be empty");
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("SaveRequest : Key cannot be null");
            await _distributedCache.SetStringAsync(key, data, options, new CancellationToken());

        }

        public async Task<string> GetString(string key)
        {
          
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("SaveRequest : Key cannot be null");
           return await _distributedCache.GetStringAsync(key, new CancellationToken());

        }

        public async Task DeleteRequest(string searchRequestKey)
        {

            if (string.IsNullOrEmpty(searchRequestKey)) throw new ArgumentNullException("DeleteRequest : Search request key cannot be null");
            await _distributedCache.RemoveAsync(searchRequestKey, new CancellationToken());

        }

        public async Task<SearchRequest> GetRequest(string searchRequestKey)
        {
       
            if (string.IsNullOrEmpty(searchRequestKey)) throw new ArgumentNullException("GetRequest : Search request key cannot be null");

            string searchRequestStr = await _distributedCache.GetStringAsync(searchRequestKey.ToString(), new CancellationToken());
            if (searchRequestStr == null) return null;
            return await Task.FromResult(JsonConvert.DeserializeObject<SearchRequest>(searchRequestStr));

        }

        public async Task Save(string key, dynamic data)
        {
            if (data == null) throw new ArgumentNullException("Save : data cannot be null");
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("Save : Key cannot be null");
            await _stackRedisCacheClient.Db0.AddAsync(key, (string)JsonConvert.SerializeObject(data));
        }

        public async Task<IEnumerable<string>> SearchKeys(string pattern)
        {
           
            if (string.IsNullOrEmpty(pattern)) throw new ArgumentNullException("SearchKey : pattern cannot be null");
            return await _stackRedisCacheClient.Db0.SearchKeysAsync(pattern);
        }

        public async Task Update(string key, dynamic data)
        {
            if (data == null) throw new ArgumentNullException("Save : data cannot be null");
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("Save : Key cannot be null");
            await _stackRedisCacheClient.Db0.ReplaceAsync(key, (string)JsonConvert.SerializeObject(data));
        }


        public async Task Save(string key, dynamic data, TimeSpan expiry)
        {
            if (data == null) throw new ArgumentNullException("Save : data cannot be null");
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("Save : Key cannot be null");
            await _stackRedisCacheClient.Db0.AddAsync(key, (string)JsonConvert.SerializeObject(data),expiry);
        }


        public async Task<string> Get(string key)
        {

            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("Get : Key cannot be null");

            string strData = await _stackRedisCacheClient.Db0.GetAsync<string>(key);

            return strData;

        }

        public async Task Delete(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("Delete : Key cannot be null");
            await _distributedCache.RemoveAsync(key, new CancellationToken());

        }

        /// <summary>
        /// Updates the complete status of a data partner for a given search request key in Redis. 
        /// If the search request data is not found or cannot be deserialized, it will remove the cached entry. 
        /// The method will retry up to a maximum number of attempts if there are concurrent updates to the same key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataPartner"></param>
        /// <returns></returns>
        public async Task UpdateDataPartnerCompleteStatus(string key, string dataPartner)
        {
            _logger.LogInformation(
                $"UpdateDataPartnerCompleteStatus - Updating complete status for search request key: {key}. Data partner: {dataPartner}."
            );

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("UpdateDataPartnerCompleteStatus - Key cannot be null");
            }

            int tryCounts = 0;
            int MAX_TRY_COUNT = 1000;
            while (tryCounts < MAX_TRY_COUNT)
            {
                tryCounts++;

                // Get the current search request data from Redis hash value
                RedisValue currentCachedValue = await _stackRedisCacheClient.Db0.Database.HashGetAsync(key, new RedisValue("data"));
                var searchRequest = currentCachedValue.IsNullOrEmpty
                    ? null
                    : JsonConvert.DeserializeObject<SearchRequest>(currentCachedValue.ToString());

                if (searchRequest == null)
                {
                    _logger.LogInformation(
                        $"UpdateDataPartnerCompleteStatus - The 'data' field for key: {key} is missing or could not be deserialized into a search request. Nothing to update."
                    );

                    // If the value of the deserialized 'data' field is null, then delete the field from Redis hash value.
                    if (!currentCachedValue.IsNullOrEmpty)
                    {
                        _logger.LogWarning(
                            "UpdateDataPartnerCompleteStatus - The 'data' field for key {Key} could not be deserialized (corrupt data). Removing the 'data' field from the cache entry.",
                            key
                        );

                        var cleanupTransaction = _stackRedisCacheClient.Db0.Database.CreateTransaction();
                        cleanupTransaction.AddCondition(Condition.HashEqual(key, new RedisValue("data"), currentCachedValue));
                        var cleanupTask = cleanupTransaction.HashDeleteAsync(key, new RedisValue("data"));

                        if (await cleanupTransaction.ExecuteAsync())
                        {
                            await cleanupTask;
                        }
                    }

                    // No non-null search request data found, therefore nothing to update, break out of the loop early
                    break;
                }

                var partner = searchRequest.DataPartners?.FirstOrDefault(item => item.Name == dataPartner);
                if (partner != null)
                {
                    _logger.LogInformation(
                        $"UpdateDataPartnerCompleteStatus - Search request found, updating the partner's completed status to true for key: {key}. Data partner: {dataPartner}."
                    );
                    // Search request found, update the partner's completed status to true
                    partner.Completed = true;
                }

                // Update the search request in Redis
                var updateTransaction = _stackRedisCacheClient.Db0.Database.CreateTransaction();
                updateTransaction.AddCondition(Condition.HashEqual(key, new RedisValue("data"), currentCachedValue));
                var updateTask = updateTransaction.HashSetAsync(
                    key,
                    new RedisValue("data"),
                    new RedisValue(JsonConvert.SerializeObject(searchRequest)),
                    When.Always,
                    CommandFlags.None
                );

                if (await updateTransaction.ExecuteAsync())
                {
                    await updateTask;
                    break;
                }
            }

            return;
        }
    }
}
