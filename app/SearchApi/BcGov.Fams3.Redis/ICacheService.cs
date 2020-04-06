
using BcGov.Fams3.Redis.Model;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading;

namespace BcGov.Fams3.Redis
{
    public interface ICacheService
    {
        Task SaveRequest(SearchRequest searchRequest);
        Task<SearchRequest> GetRequest(Guid searchRequestId);
        Task DeleteRequest(Guid searchRequestId);
    }

    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger _logger;

        public CacheService(IDistributedCache distributedCache, ILogger<CacheService> logger)
        {
            _distributedCache = distributedCache;
        }



        public async Task SaveRequest(SearchRequest searchRequest)
        {


            if (searchRequest is null) throw new ArgumentNullException("SaveRequest : Search request cannot be null");
            if (searchRequest.SearchRequestId.Equals(default(Guid))) throw new ArgumentNullException("SaveRequest : Search request id cannot be null");
            await _distributedCache.SetStringAsync(searchRequest.SearchRequestId.ToString(), JsonConvert.SerializeObject(searchRequest), new CancellationToken());


        }

        public async Task DeleteRequest(Guid searchRequestId)
        {
          
               if (searchRequestId.Equals(default(Guid))) throw new ArgumentNullException("DeleteRequest : Search request cannot be null");
                await _distributedCache.RemoveAsync(searchRequestId.ToString(), new CancellationToken());

        }

        public async  Task<SearchRequest> GetRequest(Guid searchRequestId)
        {
            
                if (searchRequestId.Equals(default(Guid))) throw new ArgumentNullException("GetRequest : Search request cannot be null");
    
                string searchRequestStr = await _distributedCache.GetStringAsync(searchRequestId.ToString(), new CancellationToken());
                if (searchRequestStr == null) return null;
                return await Task.FromResult(JsonConvert.DeserializeObject<SearchRequest>(searchRequestStr));
          
        }
    }
}
