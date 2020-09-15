using BcGov.Fams3.Redis;
using BcGov.Fams3.SearchApi.Contracts.Person;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SearchApi.Web.Configuration;
using SearchApi.Web.Controllers;
using SearchApi.Web.DeepSearch.Schema;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SearchApi.Web.DeepSearch
{
    public interface IDeepSearchService
    {
   
        Task<Person> SaveRequest(PersonSearchRequest person, string dataPartner);

    }

    public class DeepSearchService : IDeepSearchService
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<DeepSearchService> _logger;
        private readonly DeepSearchOptions _deepSearchOptions;
        public DeepSearchService(ICacheService cacheService, ILogger<DeepSearchService> logger, IOptions<DeepSearchOptions> deepSearchOptions)
        {
            _cacheService = cacheService;
            _logger = logger;
            _deepSearchOptions = deepSearchOptions.Value;
        }


       

        public async Task<Person> SaveRequest(PersonSearchRequest person, string dataPartner)
        {
            _logger.Log(LogLevel.Debug, $"Check if request {person.SearchRequestKey} has an active wave on-going");
            string cacheKey = string.Format(Keys.DEEP_SEARCH_REDIS_KEY_FORMAT, person.SearchRequestKey, dataPartner);
            var waveMetaData = await _cacheService.Get(cacheKey);

            if (string.IsNullOrEmpty(waveMetaData))
            {
                _logger.Log(LogLevel.Debug, $"{person.SearchRequestKey} does not have active wave");
                await _cacheService.Save(cacheKey, new WaveMetaData
                {
                    AllParameter = new List<Person>
                    {
                        person
                    },
                    NewParameter = new List<Person>
                    {
                        person
                    },
                    CurrentWave = 1,
                    DataPartner = dataPartner,
                    SearchRequestKey = person.SearchRequestKey

                });
            }
            else
            {
                _logger.Log(LogLevel.Debug, $"{person.SearchRequestKey} has an active wave");
                WaveMetaData metaData = JsonConvert.DeserializeObject<WaveMetaData>(waveMetaData);
                _logger.Log(LogLevel.Debug, $"Current Metadat : {waveMetaData}");
                metaData.NewParameter = new List<Person>
                {
                    person
                };
                metaData.CurrentWave++;
                await _cacheService.Save(cacheKey, metaData);
            }

            return new Person();
        }
    }
}
