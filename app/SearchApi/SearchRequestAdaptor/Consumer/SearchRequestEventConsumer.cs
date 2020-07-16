using BcGov.Fams3.SearchApi.Contracts.SearchRequest;
using MassTransit;
using Microsoft.Extensions.Logging;
using SearchRequestAdaptor.Notifier;
using Serilog.Context;
using System.Threading;
using System.Threading.Tasks;

namespace SearchRequestAdaptor.Consumer
{
    public abstract class SearchRequestEventConsumer
    {

        private readonly ILogger<SearchRequestEventConsumer> _logger;
        private readonly SearchRequestNotifier<SearchRequestEvent> _searchRequestNotifier;

        public SearchRequestEventConsumer(SearchRequestNotifier<SearchRequestEvent> searchRequestNotifier, ILogger<SearchRequestEventConsumer> logger)
        {
            _searchRequestNotifier = searchRequestNotifier;
            _logger = logger;

        }

        public async Task Consume(ConsumeContext<SearchRequestEvent> context, string eventName)
        {
            using (LogContext.PushProperty("SearchRequestKey", context.Message?.SearchRequestKey))
            {
                var cts = new CancellationTokenSource();
                _logger.LogInformation($"received new {nameof(SearchRequestEvent)} event from {context.Message.ProviderProfile?.Name}");
                await _searchRequestNotifier.NotifyEventAsync(context.Message.SearchRequestKey, context.Message, eventName,
                    cts.Token);
            }
        }
    }

    public class SearchRequestOrderedConsumer : SearchRequestEventConsumer, IConsumer<SearchRequestOrdered>
    {
        private readonly ILogger<SearchRequestOrderedConsumer> _logger;
        private readonly SearchRequestNotifier<SearchRequestEvent> _searchRequestNotifier;

        public SearchRequestOrderedConsumer(SearchRequestNotifier<SearchRequestEvent> searchRequestNotifier, ILogger<SearchRequestOrderedConsumer> logger) : base(searchRequestNotifier, logger)
        {
            _searchRequestNotifier = searchRequestNotifier;
            _logger = logger;

        }

        public async Task Consume(ConsumeContext<SearchRequestOrdered> context)
        {
            await base.Consume(context, "Ordered");
        }

    }

}
