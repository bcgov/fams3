using BcGov.Fams3.SearchApi.Contracts.SearchRequest;
using MassTransit;

using Microsoft.Extensions.Logging;
using SearchRequestAdaptor.Notifier;
using Serilog.Context;
using System.Threading;
using System.Threading.Tasks;

namespace SearchRequestAdaptor.Consumer
{
    public class SearchRequestOrderedConsumer : IConsumer<SearchRequestOrdered>
    {
        private readonly ILogger<SearchRequestOrderedConsumer> _logger;
        private readonly ISearchRequestNotifier<SearchRequestOrdered> _searchRequestNotifier;

        public SearchRequestOrderedConsumer(ISearchRequestNotifier<SearchRequestOrdered> searchRequestNotifier, ILogger<SearchRequestOrderedConsumer> logger)
        {
            _logger = logger;
            _searchRequestNotifier = searchRequestNotifier;

        }

        public async Task Consume(ConsumeContext<SearchRequestOrdered> context)
        {
            using (LogContext.PushProperty("RequestRef", $"{context.Message?.Person?.Agency?.RequestId}"))
            using (LogContext.PushProperty("AgencyCode", $"{context.Message?.Person?.Agency?.Code}"))
            {
                _logger.LogInformation("get the searchRequestOrdered message.");
                var cts = new CancellationTokenSource();
                await _searchRequestNotifier.NotifySearchRequestEventAsync(context.Message.RequestId, context.Message, cts.Token);
            }
        }

    }

}
