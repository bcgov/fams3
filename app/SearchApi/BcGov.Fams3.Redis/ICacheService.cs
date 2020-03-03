using BcGov.Fams3.Redis.Configuration;
using BcGov.Fams3.Redis.Model;
using BcGov.Fams3.SearchApi.Contracts.Person;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace BcGov.Fams3.Redis
{
    public interface ICacheService
    {
        Task<bool> SaveRequest(SearchRequest searchRequest);
        Task<SearchRequest> GetRequest(Guid searchRequestId);
        Task<bool> DeleteRequest(Guid searchRequestId);
    }

    public class CacheService : ICacheService
    {
        private static IDatabase _database;

        public CacheService(IDatabase database)
        {
            _database = database;
        }

        public Task<SearchRequest> GetRequest(Guid searchRequestId)
        {
            try
            {
                if (searchRequestId == null) return null;
                string str = searchRequestId.ToString();
                string searchRequestStr = _database.StringGet(searchRequestId.ToString(), CommandFlags.None);
                if (searchRequestStr == null) return null;
                return Task.FromResult(JsonConvert.DeserializeObject<SearchRequest>(searchRequestStr));
            }
            catch (Exception e)
            {
                throw e; 
            }
        }

        public Task<bool> SaveRequest(SearchRequest searchRequest)
        {
            try
            {
                if (searchRequest == null) return Task.FromResult(false);
                else
                {
                    return Task.FromResult(_database.StringSet(searchRequest.SearchRequestId.ToString(), JsonConvert.SerializeObject(searchRequest)));
                }
            }catch(Exception e)
            {
                throw e;
            }
        }

        public Task<bool> DeleteRequest(Guid searchRequestId)
        {
            try
            {
                if (searchRequestId == null) return Task.FromResult(false);
                return Task.FromResult(_database.KeyDelete(searchRequestId.ToString()));
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
