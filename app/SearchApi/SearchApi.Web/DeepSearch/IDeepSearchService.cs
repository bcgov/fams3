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


        Task UpdateDataPartner(string searchRequestKey, string dataPartner, string eventName);


        Task ProcessWaveSearch(string searchRequestKey);

    }

    public class DeepSearchService : IDeepSearchService
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<DeepSearchService> _logger;
        private readonly IDeepSearchDispatcher _dispatcher;
        private readonly DeepSearchOptions _deepSearchOptions;
        private readonly ISearchApiNotifier<PersonSearchAdapterEvent> _searchApiNotifier;
        public DeepSearchService(ICacheService cacheService, ILogger<DeepSearchService> logger, IOptions<DeepSearchOptions> deepSearchOptions, ISearchApiNotifier<PersonSearchAdapterEvent> searchApiNotifier, IDeepSearchDispatcher dispatcher)
        {
            _cacheService = cacheService;
            _logger = logger;
            _deepSearchOptions = deepSearchOptions.Value;
            _searchApiNotifier = searchApiNotifier;
            _dispatcher = dispatcher;
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


        private async Task<IEnumerable<WaveSearchData>> GetWaveDataForSearch(string searchRequestKey)
        {
            List<WaveSearchData> waveMetaDatas = new List<WaveSearchData>();

            var keys = await _cacheService.SearchKeys($"{searchRequestKey}*");

            foreach (var key in keys)
            {
                waveMetaDatas.Add(JsonConvert.DeserializeObject<WaveSearchData>(await _cacheService.Get(key)));
            }

            return waveMetaDatas.AsEnumerable();
        }

        public async Task ProcessWaveSearch(string searchRequestKey)
        {
            if (!await CurrentWaveIsCompleted(searchRequestKey))
            {

                var waveData = await GetWaveDataForSearch(searchRequestKey);
                if (!waveData.Any()) throw new InvalidOperationException("Unable to process wave");
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
                            await _dispatcher.Dispatch(searchRequestKey, wave, person);
                    }
                }

            }

        }


    }
}
