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
        bool SaveRequest(SearchRequest searchRequest);
        SearchRequest GetRequest(Guid searchRequestId);
        bool DeleteRequest(Guid searchRequestId);
    }

    public class CacheService : ICacheService
    {
        private static IDatabase _database;

        public CacheService(IDatabase database)
        {
            _database = database;
        }

        public SearchRequest GetRequest(Guid searchRequestId)
        {
            try
            {
                if (searchRequestId == null) return null;
                string str = searchRequestId.ToString();
                string searchRequestStr = _database.StringGet(searchRequestId.ToString(), CommandFlags.None);
                if (searchRequestStr == null) return null;
                return JsonConvert.DeserializeObject<SearchRequest>(searchRequestStr);
            }catch(Exception)
            {
                return null;
            }
        }

        public bool SaveRequest(SearchRequest searchRequest)
        {
            try
            {
                if (searchRequest == null) return false;
                else
                {
                    return _database.StringSet(searchRequest.SearchRequestId.ToString(), JsonConvert.SerializeObject(searchRequest));
                }
            }catch(Exception)
            {
                return false;
            }
        }

        public bool DeleteRequest(Guid searchRequestId)
        {
            if (searchRequestId == null) return false;
            return _database.KeyDelete(searchRequestId.ToString());
        }
    }
}
