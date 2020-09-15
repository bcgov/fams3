using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using BcGov.Fams3.SearchApi.Core.Configuration;
using MassTransit;
using Microsoft.Extensions.Options;
using SearchApi.Web.Controllers;
using SearchApi.Web.DeepSearch;
using SearchApi.Web.DeepSearch.Schema;

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
        private readonly IDeepSearchService _deepSearchService;
        private readonly RabbitMqConfiguration _rabbitMqConfiguration;
        //, IDeepSearchService deepSearchService 
        public Dispatcher(ISendEndpointProvider sendEndpointProvider, IOptions<RabbitMqConfiguration> rabbitMqOptions)
        {
          //  _deepSearchService = deepSearchService;
            _sendEndpointProvider = sendEndpointProvider;
            _rabbitMqConfiguration = rabbitMqOptions.Value;
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
   
                await _deepSearchService.SaveRequest(personSearchRequest, requestDataProvider.Name);

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

    }
}