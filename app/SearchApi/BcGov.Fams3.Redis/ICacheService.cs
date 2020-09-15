
using BcGov.Fams3.Redis.Model;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BcGov.Fams3.Redis
{
    public interface ICacheService
    {
        Task SaveRequest(SearchRequest searchRequest);
        Task SaveRequest(string data, string key);
        Task<SearchRequest> GetRequest(string searchRequestKey);
        Task DeleteRequest(string id);

        Task Save(string key, dynamic data, TimeSpan expiry);

        Task Save(string key, dynamic data);
        Task<string> Get(string key);
        Task Delete(string key);
    }

    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;


        public CacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;

        }



        public async Task SaveRequest(SearchRequest searchRequest)
        {

            
            if (searchRequest is null) throw new ArgumentNullException("SaveRequest : Search request cannot be null");
            if (string.IsNullOrEmpty(searchRequest.SearchRequestKey)) throw new ArgumentNullException("SaveRequest : Search request key cannot be null");
            await _distributedCache.SetStringAsync(searchRequest.SearchRequestKey, JsonConvert.SerializeObject(searchRequest), new CancellationToken());


        }

        public async Task SaveRequest(string data, string key)
        {
            if (string.IsNullOrEmpty(data)) throw new ArgumentNullException("SaveRequest : Data cannot be empty");
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("SaveRequest : Key cannot be null");
            await _distributedCache.SetStringAsync(key, data, new CancellationToken());

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
            await _distributedCache.SetStringAsync(key, (string)JsonConvert.SerializeObject(data), new CancellationToken());
        }

        public async Task Save(string key, dynamic data, TimeSpan expiry)
        {
            if (data == null) throw new ArgumentNullException("Save : data cannot be null");
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("Save : Key cannot be null");
            await _distributedCache.SetStringAsync(key, (string)JsonConvert.SerializeObject(data),new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiry } , new CancellationToken());
        }


        public async Task<string> Get(string key)
        {

            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("Get : Key cannot be null");

            string strData = await _distributedCache.GetStringAsync(key, new CancellationToken());

            return strData;

        }

        public async Task Delete(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("Delete : Key cannot be null");
            await _distributedCache.RemoveAsync(key, new CancellationToken());

        }
    }
}
