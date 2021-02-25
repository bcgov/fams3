using BcGov.Fams3.SearchApi.Contracts.SearchRequest;
using BcGov.Fams3.SearchApi.Core.Configuration;
using MassTransit;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly IOptions<RetryConfiguration>  _retryConfig;

        public SearchRequestOrderedConsumer(ISearchRequestNotifier<SearchRequestOrdered> searchRequestNotifier, ILogger<SearchRequestOrderedConsumer> logger, IOptions<RetryConfiguration> retryConfig)
        {
            _logger = logger;
            _searchRequestNotifier = searchRequestNotifier;
            _retryConfig = retryConfig;

        }

        public async Task Consume(ConsumeContext<SearchRequestOrdered> context)
        {
            using (LogContext.PushProperty("RequestRef", $"{context.Message?.Person?.Agency?.RequestId}"))
            using (LogContext.PushProperty("AgencyCode", $"{context.Message?.Person?.Agency?.Code}"))
            {
                _logger.LogInformation("get the searchRequestOrdered message.");
                var cts = new CancellationTokenSource();
                await _searchRequestNotifier.NotifySearchRequestEventAsync(context.Message.RequestId, context.Message, cts.Token, context.GetRetryAttempt(), _retryConfig.Value.RetryTimes);
            }
        }

    }

}
