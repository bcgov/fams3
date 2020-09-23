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
   
                await SaveRequest(personSearchRequest, requestDataProvider.Name);

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

        private async Task SaveRequest(PersonSearchRequest person, string dataPartner)
        {
            _logger.Log(LogLevel.Debug, $"Check if request {person.SearchRequestKey} has an active wave on-going");
            string cacheKey = person.SearchRequestKey.DeepSearchKey(dataPartner);
            var waveMetaData = await _cacheService.Get(cacheKey);

            if (string.IsNullOrEmpty(waveMetaData))
            {
                _logger.Log(LogLevel.Debug, $"{person.SearchRequestKey} does not have active wave");
                await _cacheService.Save(cacheKey, new WaveSearchData
                {
                    AllParameter = new List<Person>
                    {
                        person
                    },
                    NewParameter =null,
                    CurrentWave = 1,
                    DataPartner = dataPartner,
                    SearchRequestKey = person.SearchRequestKey

                });
                _logger.Log(LogLevel.Debug, $"{person.SearchRequestKey} saved");
            }
            else
            {
                _logger.Log(LogLevel.Debug, $"{person.SearchRequestKey} has an active wave");
                WaveSearchData metaData = JsonConvert.DeserializeObject<WaveSearchData>(waveMetaData);
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

    }
}