using BcGov.Fams3.SearchApi.Contracts.SearchRequest;
using MassTransit;
using Microsoft.Extensions.Logging;
using OpenTracing;
using SearchRequestAdaptor.Publisher.Models;
using System.Threading.Tasks;

namespace SearchRequestAdaptor.Publisher
{
    public interface ISearchRequestEventPublisher
    {
        public Task PublishSearchRequestFailed(SearchRequestEvent baseEvent, string message);

    }

    public class SearchRequestEventPublisher : ISearchRequestEventPublisher
    {

        private readonly IBusControl _busControl;

        private readonly ILogger _logger;
        public SearchRequestEventPublisher(IBusControl busControl, ILogger<SearchRequestEventPublisher> logger)
        {
            _logger = logger;
            _busControl = busControl;
        }
        public async Task PublishSearchRequestFailed(SearchRequestEvent baseEvent, string message)
        {
            SearchRequestFailedEvent failedEvent = new SearchRequestFailedEvent(baseEvent) { Cause = message };
            await _busControl.Publish<SearchRequestFailed>(failedEvent);
        }
    }
}
