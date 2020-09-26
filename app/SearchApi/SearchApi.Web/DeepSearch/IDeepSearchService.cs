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
        Task UpdateParameters(string eventName, PersonSearchCompleted eventStatus, string searchRequestKey);
        Task  DeleteFromCache(string searchRequestKey);

        Task<bool> IsWaveSearchReadyToFinalize(string searchRequestKey);

    }

    public class DeepSearchService : IDeepSearchService
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<DeepSearchService> _logger;
        private readonly IDeepSearchDispatcher _deepSearchDispatcher;
        private readonly DeepSearchOptions _deepSearchOptions;
      
        public DeepSearchService(ICacheService cacheService, ILogger<DeepSearchService> logger, IOptions<DeepSearchOptions> deepSearchOptions, IDeepSearchDispatcher deepSearchDispatcher)
        {
            _cacheService = cacheService;
            _logger = logger;
            _deepSearchOptions = deepSearchOptions.Value;
     
             _deepSearchDispatcher = deepSearchDispatcher;
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

        public  async Task DeleteFromCache(string searchRequestKey)
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

        public async Task UpdateParameters(string eventName, PersonSearchCompleted eventStatus, string searchRequestKey)
        {
            if (eventName.Equals(EventName.Completed))
            {
                

                var paramsRegistry = Registry.DataPartnerParameters[eventStatus.ProviderProfile.Name];
                IEnumerable<WaveSearchData> waveSearches = await GetWaveDataForSearch(searchRequestKey);

                List<PersonalIdentifier> existingIds = new List<PersonalIdentifier>();

                foreach (var waveitem in waveSearches)
                {
                    _logger.LogInformation($"Store all parameters {waveitem.DataPartner}");
                    waveitem.AllParameter.ForEach(p => existingIds.AddRange(p.Identifiers));
                }

                _logger.LogInformation($"{existingIds.Count()} Identifier exists");
                var filteredNewIdentifierForDataPartner = existingIds.Where(identifer => paramsRegistry.Contains(identifer.Type));
                _logger.LogInformation($"{existingIds.Count()} Identifier matched the require types for {eventStatus.ProviderProfile.Name}");

                var matchedPersons = eventStatus.MatchedPersons;
                List<PersonalIdentifier> foundId = new List<PersonalIdentifier>();
                matchedPersons.ToList().ForEach(p => p.Identifiers.ToList().ForEach(id => foundId.Add(id)));

                _logger.LogInformation($"{foundId.Count()} Identifier was returned by {eventStatus.ProviderProfile.Name}");
                var filteredFoundIdentifierForDataPartner = foundId.Where(identifer => paramsRegistry.Contains(identifer.Type));
                _logger.LogInformation($"{filteredFoundIdentifierForDataPartner.Count()} returned Identifier matched the require types for {eventStatus.ProviderProfile.Name}");

                
                foreach (var waveitem in waveSearches)
                {
                    var newToBeUsedId = filteredNewIdentifierForDataPartner.DetailedCompare(filteredFoundIdentifierForDataPartner);

                    if (newToBeUsedId.Count() != 0)
                    {
                        waveitem.AllParameter.Add(new Person { Identifiers = newToBeUsedId });
                        if (waveitem.NewParameter.Count == 0)
                            waveitem.NewParameter.Add(new Person { Identifiers = newToBeUsedId });
                        else
                            waveitem.NewParameter[0] = new Person { Identifiers = newToBeUsedId };
                    }
                    await _cacheService.Save(searchRequestKey.DeepSearchKey(eventStatus.ProviderProfile.Name), waveitem);
                }
                
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

        public async Task<bool> IsWaveSearchReadyToFinalize(string searchRequestKey)
        {
            if (!await CurrentWaveIsCompleted(searchRequestKey))
            { 
                
                var waveData = await GetWaveDataForSearch(searchRequestKey);
                if (!waveData.Any()) throw new InvalidOperationException("Unable to process wave");
                if (waveData.Any(x => x.CurrentWave == _deepSearchOptions.MaxWaveCount))
                {
                    return true;
                }
                else
                {
                    foreach (var wave in waveData)
                    {
                        foreach (var person in wave.NewParameter)
                        {
                        //    var pSR = new PersonSearchRequest(person.FirstName, person.LastName, person.DateOfBirth, person.Identifiers, person.Addresses, person.Phones, person.Names, person.RelatedPersons, person.Employments, new List<DataProvider>
                        //{
                        //    new DataProvider { Completed = false, Name = wave.DataPartner, NumberOfRetries = 1, TimeBetweenRetries = 3 }
                        //}, searchRequestKey);
                           await _deepSearchDispatcher.StartAnotherWave(searchRequestKey,wave, person);
                        }
                    }
                    return false;
                }
            
            }
            return false;
            
        }

     
    }
}
