using BcGov.Fams3.Redis;
using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SearchApi.Web.Configuration;
using SearchApi.Web.Controllers;
using SearchApi.Web.DeepSearch.Schema;
using SearchApi.Web.Messaging;
using SearchApi.Web.Notifications;
using SearchApi.Web.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SearchApi.Web.DeepSearch
{
    public interface IDeepSearchService
    {
   
        Task SaveRequest(PersonSearchRequest person, string dataPartner);

        Task UpdateDataPartner(string searchRequestKey, string dataPartner, string eventName);


        Task ProcessWaveSearch(string searchRequestKey);

    }

    public class DeepSearchService : IDeepSearchService
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<DeepSearchService> _logger;
        private readonly IDispatcher _dispatcher;
        private readonly DeepSearchOptions _deepSearchOptions;
        private readonly ISearchApiNotifier<PersonSearchAdapterEvent> _searchApiNotifier;
        public DeepSearchService(ICacheService cacheService, ILogger<DeepSearchService> logger, IOptions<DeepSearchOptions> deepSearchOptions, ISearchApiNotifier<PersonSearchAdapterEvent> searchApiNotifier, IDispatcher dispatcher)
        {
            _cacheService = cacheService;
            _logger = logger;
            _deepSearchOptions = deepSearchOptions.Value;
            _searchApiNotifier = searchApiNotifier;
            _dispatcher = dispatcher;
        }


       

        public async Task SaveRequest(PersonSearchRequest person, string dataPartner)
        {
            _logger.Log(LogLevel.Debug, $"Check if request {person.SearchRequestKey} has an active wave on-going");
            string cacheKey = person.SearchRequestKey.DeepSearchKey(dataPartner);
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
                _logger.Log(LogLevel.Debug, $"{person.SearchRequestKey} saved");
            }
            else
            {
                _logger.Log(LogLevel.Debug, $"{person.SearchRequestKey} has an active wave");
                WaveMetaData metaData = JsonConvert.DeserializeObject<WaveMetaData>(waveMetaData);
                _logger.Log(LogLevel.Debug, $"{person.SearchRequestKey} Current Metadata Wave : {metaData.CurrentWave}");
                metaData.NewParameter = new List<Person>
                {
                    person
                };
                metaData.CurrentWave++;
                await _cacheService.Save(cacheKey, metaData);
                _logger.Log(LogLevel.Debug, $"{person.SearchRequestKey} New wave {metaData.CurrentWave} saved");
                

            }

           
        }

        private async Task<bool> CurrentWaveIsCompleted(string searchRequestKey)
        {
            try
            {
                return JsonConvert.SerializeObject(await _cacheService.GetRequest(searchRequestKey)).AllPartnerCompleted();

            }
            catch (Exception exception)
            {
                _logger.LogError($"Check Data Partner Status Failed. [] for {searchRequestKey}. [{exception.Message}]");
                return false;
            }
        }
        public async Task UpdateDataPartner(string searchRequestKey, string dataPartner, string eventName)
        {
            try
            {
                if (eventName.Equals(EventName.Completed) || eventName.Equals(EventName.Rejected) || eventName.Equals(EventName.Failed))
                {
                    var searchRequest = JsonConvert.SerializeObject(await _cacheService.GetRequest(searchRequestKey)).UpdateDataPartner(dataPartner);
                    await _cacheService.SaveRequest(searchRequest);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError($"Update Data Partner Status Failed. [{eventName}] for {searchRequestKey}. [{exception.Message}]");

            }
        }

        private async Task DeleteFromCache(string searchRequestKey)
        {
            try
            {
                await _cacheService.DeleteRequest(searchRequestKey);
                var keys = await _cacheService.SearchKeys($"{searchRequestKey}*");
                foreach (var key in keys)
                    await _cacheService.Delete(key);
            }
            catch (Exception exception)
            {
                _logger.LogError($"Delete search request failed. for {searchRequestKey}. [{exception.Message}]");
            }
        }


        private async Task<IEnumerable<WaveMetaData>> GetWaveDataForSearch(string searchRequestKey)
        {
            List<WaveMetaData> waveMetaDatas = new List<WaveMetaData>();

            var keys = await _cacheService.SearchKeys($"{searchRequestKey}*");

            foreach (var key in keys)
            {
                waveMetaDatas.Add(JsonConvert.DeserializeObject<WaveMetaData>(await _cacheService.Get(key)));
            }

            return waveMetaDatas.AsEnumerable();
        }

        public async Task ProcessWaveSearch(string searchRequestKey)
        {
            if (!await CurrentWaveIsCompleted(searchRequestKey))
            { 
                
                var waveData = await GetWaveDataForSearch(searchRequestKey);
                if (waveData.Any(x => x.CurrentWave == _deepSearchOptions.MaxWaveCount))
                {
                    PersonSearchAdapterEvent finalizedSearch = new PersonSearchFinalizedEvent()
                    {
                        SearchRequestKey = searchRequestKey,
                        Message = "Search Request Finalized",
                        SearchRequestId = Guid.NewGuid(),
                        TimeStamp = DateTime.Now
                    };
                    await _searchApiNotifier.NotifyEventAsync(searchRequestKey, finalizedSearch, EventName.Finalized, new CancellationToken());
                    await DeleteFromCache(searchRequestKey);
                }
                else
                {
                    foreach (var wave in waveData)
                    {
                        foreach (var person in wave.NewParameter)
                        await _dispatcher.Dispatch(new PersonSearchRequest(person.FirstName, person.LastName, person.DateOfBirth, person.Identifiers, person.Addresses, person.Phones, person.Names, person.RelatedPersons, person.Employments, new List<DataProvider>
                        {
                            new DataProvider { Completed = false, Name = wave.DataPartner, NumberOfRetries = 1, TimeBetweenRetries = 3 }
                        }, searchRequestKey), Guid.NewGuid());
                    }
                }
            
            }
            
        }
    }
}
