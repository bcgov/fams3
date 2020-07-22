using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using BcGov.Fams3.SearchApi.Contracts.SearchRequest;
using BcGov.Fams3.SearchApi.Core.Configuration;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTracing;
using SearchRequestAdaptor.Publisher.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SearchRequestAdaptor.Publisher
{
    public interface ISearchRequestEventPublisher
    {
        public Task PublishSearchRequestFailed(SearchRequestEvent baseEvent, string message);
        public Task PublishSearchRequestSubmitted(SearchRequestEvent baseEvent, string message);
        public Task PublishSearchRequestRejected(SearchRequestEvent baseEvent, IEnumerable<ValidationResult> reasons);

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

            var endpoint = await getSearchRequestFailedEndpoint();

            await endpoint.Send<SearchRequestFailed>(new SearchRequestFailedEvent(baseEvent)
            {
                Cause = message
            });
        }

        public async Task PublishSearchRequestRejected(SearchRequestEvent baseEvent, IEnumerable<ValidationResult> reasons)
        {
            if (baseEvent == null) throw new ArgumentNullException(nameof(SearchRequestEvent));

            var endpoint = await getSearchRequestRejectedEndpoint();

            await endpoint.Send<SearchRequestRejected>(new SearchRequestRejectedEvent(baseEvent)
            {
                Reasons = reasons
            });
        }

        public async Task PublishSearchRequestSubmitted(SearchRequestEvent baseEvent, string message)
        {
            if (baseEvent == null) throw new ArgumentNullException(nameof(SearchRequestEvent));

            var endpoint = await getSearchRequestSubmittedEndpoint();

            await endpoint.Send<SearchRequestSubmitted>(new SearchRequestSubmittedEvent(baseEvent)
            {
                Message = message
            });
        }

        private Task<ISendEndpoint> getSearchRequestFailedEndpoint()
        {
            return _sendEndpointProvider.GetSendEndpoint(new Uri($"rabbitmq://{this._rabbitMqConfiguration.Host}:{this._rabbitMqConfiguration.Port}/SearchRequestFailed_queue"));
        }

        private Task<ISendEndpoint> getSearchRequestSubmittedEndpoint()
        {
            return _sendEndpointProvider.GetSendEndpoint(new Uri($"rabbitmq://{this._rabbitMqConfiguration.Host}:{this._rabbitMqConfiguration.Port}/SearchRequestSubmitted_queue"));
        }

        private Task<ISendEndpoint> getSearchRequestRejectedEndpoint()
        {
            return _sendEndpointProvider.GetSendEndpoint(new Uri($"rabbitmq://{this._rabbitMqConfiguration.Host}:{this._rabbitMqConfiguration.Port}/SearchRequestRejected_queue"));
        }
    }
}
