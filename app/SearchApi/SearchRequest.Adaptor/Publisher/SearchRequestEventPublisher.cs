using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using BcGov.Fams3.SearchApi.Contracts.SearchRequest;
using BcGov.Fams3.SearchApi.Core.Configuration;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTracing;
using SearchRequest.Adaptor.Publisher.Models;
using SearchRequestAdaptor.Publisher.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SearchRequestAdaptor.Publisher
{
    public interface ISearchRequestEventPublisher
    {
        public Task PublishSearchRequestFailed(SearchRequestEvent baseEvent, string message);
        public Task PublishSearchRequestSaved(SearchRequestSavedEvent savedEvent);
        public Task PublishSearchRequestRejected(SearchRequestEvent baseEvent, IEnumerable<ValidationResult> reasons);

        public Task PublishSearchRequestNotification(SearchRequestNotificationEvent baseEvent);

    }

    public class SearchRequestEventPublisher : ISearchRequestEventPublisher
    {

        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly RabbitMqConfiguration _rabbitMqConfiguration;
        private readonly ILogger _logger;

        public SearchRequestEventPublisher(ISendEndpointProvider sendEndpointProvider, IOptions<RabbitMqConfiguration> rabbitMqOptions, ILogger<SearchRequestEventPublisher> logger)
        {
            _logger = logger;
            _sendEndpointProvider = sendEndpointProvider;
            _rabbitMqConfiguration = rabbitMqOptions.Value;
        }
        public async Task PublishSearchRequestFailed(SearchRequestEvent baseEvent, string message)
        {
            if (baseEvent == null) throw new ArgumentNullException(nameof(SearchRequestEvent));

            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"rabbitmq://{this._rabbitMqConfiguration.Host}:{this._rabbitMqConfiguration.Port}/{nameof(SearchRequestFailed)}_queue"));

            _logger.LogInformation($"SearchRequestFailed has been published to {endpoint.ToString()}");
            await endpoint.Send<SearchRequestFailed>(new SearchRequestFailedEvent(baseEvent)
            {
                Cause = message
            });
        }

        public async Task PublishSearchRequestNotification(SearchRequestNotificationEvent baseEvent)
        {
            if (baseEvent == null) throw new ArgumentNullException(nameof(SearchRequestEvent));

            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"rabbitmq://{this._rabbitMqConfiguration.Host}:{this._rabbitMqConfiguration.Port}/{nameof(SearchRequestNotification)}_queue"));

            await endpoint.Send<SearchRequestNotification>(baseEvent);
        }

        public async Task PublishSearchRequestRejected(SearchRequestEvent baseEvent, IEnumerable<ValidationResult> reasons)
        {
            if (baseEvent == null) throw new ArgumentNullException(nameof(SearchRequestEvent));

            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"rabbitmq://{this._rabbitMqConfiguration.Host}:{this._rabbitMqConfiguration.Port}/{nameof(SearchRequestRejected)}_queue"));

            _logger.LogInformation($"SearchRequestRejected has been published to {endpoint.ToString()}");
            await endpoint.Send<SearchRequestRejected>(new SearchRequestRejectedEvent(baseEvent)
            {
                Reasons = reasons
            });
        }

        public async Task PublishSearchRequestSaved(SearchRequestSavedEvent savedEvent)
        {
            if (savedEvent == null) throw new ArgumentNullException(nameof(SearchRequestSaved));

            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"rabbitmq://{this._rabbitMqConfiguration.Host}:{this._rabbitMqConfiguration.Port}/{nameof(SearchRequestSaved)}_queue"));

            _logger.LogInformation($"SearchRequestSaved has been published to {endpoint.ToString()}");
            await endpoint.Send<SearchRequestSaved>(savedEvent);
        }
    }
}
