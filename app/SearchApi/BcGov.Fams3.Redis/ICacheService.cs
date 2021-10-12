
using BcGov.Fams3.Redis.Model;
using Microsoft.Extensions.Caching.Distributed;
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

        public CacheService(IDistributedCache distributedCache, IRedisCacheClient stackRedisCacheClient)
        {
            _distributedCache = distributedCache;
            _stackRedisCacheClient = stackRedisCacheClient;

        }



        public async Task SaveRequest(SearchRequest searchRequest)
        {           
            if (searchRequest is null) throw new ArgumentNullException("SaveRequest : Search request cannot be null");
            if (string.IsNullOrEmpty(searchRequest.SearchRequestKey)) throw new ArgumentNullException("SaveRequest : Search request key cannot be null");
            //await _distributedCache.SetStringAsync(searchRequest.SearchRequestKey, JsonConvert.SerializeObject(searchRequest), new CancellationToken());

            //FAMS3-3861: Error protection: add status tracking in Redis 
            bool finished = false;
            int tryCounts = 0;
            int MAX_TRY_COUNT = 4;
            while (!finished && tryCounts < MAX_TRY_COUNT)
            {
                tryCounts++;
                try
                {
                    await _distributedCache.SetStringAsync(searchRequest.SearchRequestKey, JsonConvert.SerializeObject(searchRequest), new CancellationToken());
                    finished = true;
                }
                catch (Exception ex)
                {
                    tryCounts++;
                }
            }

            if (!finished)
            {
                await DeleteRequest(searchRequest.SearchRequestKey);
                throw new ArgumentNullException("SaveRequest : request key cannot save in Redis");
            }
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

        public async Task<int> UpdateDataPartnerCompleteStatus(string key, string dataPartner)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("Save : Key cannot be null");
            bool committed = false;
            int tryCounts = 0;
            int MAX_TRY_COUNT = 1000;
            while (!committed && tryCounts<MAX_TRY_COUNT)
            {
                tryCounts++;
                RedisValue oldValue = await _stackRedisCacheClient.Db0.Database.HashGetAsync(key, new RedisValue("data"));
                var searchRequest=JsonConvert.DeserializeObject<SearchRequest>(oldValue.ToString());
                if (searchRequest != null)
                {
                    var partner = searchRequest.DataPartners?.FirstOrDefault(x => x.Name == dataPartner);
                    if (partner != null)
                    {
                        partner.Completed = true;
                    }
                }
                var trans = _stackRedisCacheClient.Db0.Database.CreateTransaction();
                trans.AddCondition(Condition.HashEqual(key, new RedisValue("data"), oldValue));
                string newData = JsonConvert.SerializeObject(searchRequest);
                var task = trans.HashSetAsync(key, new RedisValue("data"),new RedisValue(newData),When.Always, CommandFlags.None);
                if (await trans.ExecuteAsync())
                {
                    var foo = await task;
                    committed = true;
                }
            }
            return tryCounts;
        }
    }
}
