using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BcGov.Fams3.Redis;
using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using BcGov.Fams3.SearchApi.Core.Configuration;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SearchApi.Web.Controllers;
using SearchApi.Web.DeepSearch;
using SearchApi.Web.DeepSearch.Schema;
using SearchApi.Web.Search;

namespace SearchApi.Web.Messaging
{
    public interface IDispatcher
    {
        Task Dispatch(PersonSearchRequest personSearchRequest, Guid searchRequestId);
    }

    /// <summary>
    /// The dispatcher class is responsible for dispatching messages to data partners based on the request configuration
    /// </summary>
    public class Dispatcher : IDispatcher
    {

        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly ICacheService _cacheService;
        private readonly ILogger<IDispatcher> _logger;
        private readonly RabbitMqConfiguration _rabbitMqConfiguration;
        //, IDeepSearchService deepSearchService 
        public Dispatcher(ILogger<IDispatcher> logger ,ISendEndpointProvider sendEndpointProvider, IOptions<RabbitMqConfiguration> rabbitMqOptions, ICacheService cacheService)
        {
            _cacheService = cacheService;
            _sendEndpointProvider = sendEndpointProvider;
            _rabbitMqConfiguration = rabbitMqOptions.Value;
            _logger = logger;
        }

        /// <summary>
        /// Dispatch a request based on the <see cref="PersonSearchRequest" dataproviders/>
        /// </summary>
        /// <param name="personSearchRequest"></param>
        /// <param name="searchRequestId"></param>
        /// <returns></returns>
        public async Task Dispatch(PersonSearchRequest personSearchRequest, Guid searchRequestId)
        {

            if(personSearchRequest == null) throw new ArgumentNullException(nameof(personSearchRequest));
            if(searchRequestId.Equals(default(Guid))) throw new ArgumentNullException(nameof(searchRequestId));

            if (personSearchRequest.DataProviders == null)
            {
                await Task.CompletedTask;
                return;
            }

         

            foreach (var requestDataProvider in personSearchRequest.DataProviders)
            {

                if (requestDataProvider.SearchSpeedType == SearchSpeedType.Fast && !personSearchRequest.IsPreScreenSearch)
                    await SaveForDeepSearch(personSearchRequest, requestDataProvider);

                var endpoint = await getEndpointAddress(requestDataProvider.Name);

                await endpoint.Send<PersonSearchOrdered>(new PeopleController.PersonSearchOrderEvent(searchRequestId, personSearchRequest.SearchRequestKey)
                {
                    Person = personSearchRequest,
                    TimeBetweenRetries = requestDataProvider.TimeBetweenRetries,
                    NumberOfRetries = requestDataProvider.NumberOfRetries
                });

                
            }
        }

        private Task<ISendEndpoint> getEndpointAddress(string providerName)
        {
            return _sendEndpointProvider.GetSendEndpoint(new Uri($"rabbitmq://{this._rabbitMqConfiguration.Host}:{this._rabbitMqConfiguration.Port}/PersonSearchOrdered_{providerName.ToUpperInvariant()}"));
        }

        private async Task SaveForDeepSearch(PersonSearchRequest person, DataProvider dataPartner)
        {
            _logger.Log(LogLevel.Information, $"Check if request {person.SearchRequestKey} has an active wave on-going");

            _logger.Log(LogLevel.Information, $"In wave for {person.SearchRequestKey} with {dataPartner.Name} -  {nameof(dataPartner.SearchSpeedType)} Search");

            string cacheKey = person.SearchRequestKey.DeepSearchKey(dataPartner.Name);
            var waveMetaData = await _cacheService.Get(cacheKey);

            if (string.IsNullOrEmpty(waveMetaData))
            {
                _logger.Log(LogLevel.Information, $"{person.SearchRequestKey} does not have active wave");
                await _cacheService.Save(cacheKey, new WaveSearchData
                {
                    AllParameter = new List<Person>
                    {
                        person
                    },
                    NewParameter =null,
                    CurrentWave = 1,
                    DataPartner = dataPartner.Name,
                    NumberOfRetries = dataPartner.NumberOfRetries,
                    TimeBetweenRetries = dataPartner.TimeBetweenRetries,
                    SearchRequestKey = person.SearchRequestKey,
                    SearchSpeed = dataPartner.SearchSpeedType
                });
                _logger.Log(LogLevel.Information, $"{person.SearchRequestKey} saved");
            }
            else
            {
                _logger.Log(LogLevel.Information, $"{person.SearchRequestKey} has an active wave");
                WaveSearchData metaData = JsonConvert.DeserializeObject<WaveSearchData>(waveMetaData);
                _logger.Log(LogLevel.Information, $"{person.SearchRequestKey} Current Metadata Wave : {metaData.CurrentWave}");
                metaData.CurrentWave++;
                metaData.NewParameter = null;
                await _cacheService.Save(cacheKey, metaData);
                _logger.Log(LogLevel.Information, $"{person.SearchRequestKey} New wave {metaData.CurrentWave} saved");


            }

            await ResetDataPartner(person.SearchRequestKey, dataPartner.Name);


        }
        private async Task ResetDataPartner(string searchRequestKey, string dataPartner)
        {
            try
            {
                var searchRequest = await _cacheService.GetRequest(searchRequestKey);
                if(searchRequest!=null) searchRequest.ResetDataPartner(dataPartner);
                await _cacheService.SaveRequest(searchRequest);
            }
            catch (Exception exception)
            {
                _logger.LogError($"Reset Data Partner Status Failed. For {searchRequestKey}. [{exception.Message}]");

            }
        }

    }
}