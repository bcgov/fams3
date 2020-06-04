
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
        Task<SearchRequest> GetRequest(Guid id);
        Task DeleteRequest(Guid id);
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
            if (searchRequest.SearchRequestId.Equals(default(Guid))) throw new ArgumentNullException("SaveRequest : Search request id cannot be null");
            await _distributedCache.SetStringAsync(searchRequest.SearchRequestId.ToString(), JsonConvert.SerializeObject(searchRequest), new CancellationToken());


        }

        public async Task SaveRequest(string data, string key)
        {


            if (string.IsNullOrEmpty(data)) throw new ArgumentNullException("SaveRequest : Data cannot be empty");
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("SaveRequest : Key cannot be null");
            await _distributedCache.SetStringAsync(key, data, new CancellationToken());

        }

        public async Task DeleteRequest(Guid searchRequestId)
        {

            if (searchRequestId.Equals(default(Guid))) throw new ArgumentNullException("DeleteRequest : Search request cannot be null");
            await _distributedCache.RemoveAsync(searchRequestId.ToString(), new CancellationToken());

        }

        public async Task<SearchRequest> GetRequest(Guid searchRequestId)
        {

            if (searchRequestId.Equals(default(Guid))) throw new ArgumentNullException("GetRequest : Search request cannot be null");

            string searchRequestStr = await _distributedCache.GetStringAsync(searchRequestId.ToString(), new CancellationToken());
            if (searchRequestStr == null) return null;
            return await Task.FromResult(JsonConvert.DeserializeObject<SearchRequest>(searchRequestStr));

        }

        public async Task Save(string key, dynamic data)
        {
            if (data == null) throw new ArgumentNullException("Save : data cannot be null");
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("Save : Key cannot be null");
            await _distributedCache.SetStringAsync(key, (string)JsonConvert.SerializeObject(data), new CancellationToken());
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
