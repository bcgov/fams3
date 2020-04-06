
using BcGov.Fams3.Redis.Model;
using Newtonsoft.Json;
using StackExchange.Redis;
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
            try
            {
                if (searchRequest == null) throw new InvalidOperationException("SaveRequest : Search request cannot be null");
                await _distributedCache.SetStringAsync(searchRequest.SearchRequestId.ToString(), JsonConvert.SerializeObject(searchRequest), new CancellationToken());
                
            }
            
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw e;
            }
        }

        public async Task DeleteRequest(Guid searchRequestId)
        {
            try
            {
                if (searchRequestId == null) throw new InvalidOperationException("DeleteRequest : Search request cannot be null");
                await _distributedCache.RemoveAsync(searchRequestId.ToString(), new CancellationToken());

            }
            
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw e;
            }
        }

        public async  Task<SearchRequest> GetRequest(Guid searchRequestId)
        {
            try
            {
                if (searchRequestId == null) throw new InvalidOperationException("GetRequest : Search request cannot be null");
                string str = searchRequestId.ToString();
                string searchRequestStr = await _distributedCache.GetStringAsync(searchRequestId.ToString(), new CancellationToken());
                if (searchRequestStr == null) return null;
                return await Task.FromResult(JsonConvert.DeserializeObject<SearchRequest>(searchRequestStr));
            }

            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw e;
            }
        }
    }
}
