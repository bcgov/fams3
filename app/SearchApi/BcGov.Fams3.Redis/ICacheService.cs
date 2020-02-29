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
        void SaveRequest(SearchRequest searchRequest);
        SearchRequest GetRequest(Guid searchRequestId);
        void DeleteRequest(Guid searchRequestId);
    }

    public class CacheService : ICacheService
    {
        private static IDatabase _database;
        public CacheService(string redisConfiguration )
        {
            var connection = RedisConnectionFactory.OpenConnection(redisConfiguration);
            _database = connection.GetDatabase();
        }

        public SearchRequest GetRequest(Guid searchRequestId)
        {
            string searchRequest = _database.StringGet(searchRequestId.ToString());
            return (SearchRequest)(JsonConvert.DeserializeObject(searchRequest));
        }

        public void SaveRequest(SearchRequest searchRequest)
        {
            _database.StringSet(searchRequest.SearchRequestId.ToString(),JsonConvert.SerializeObject(searchRequest));
        }

        public void DeleteRequest(Guid searchRequestId)
        {
            _database.KeyDelete(searchRequestId.ToString());
        }
    }
}
