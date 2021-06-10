using BcGov.Fams3.SearchApi.Contracts.SearchRequest;
using BcGov.Fams3.SearchApi.Core.Configuration;
using MassTransit;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SearchRequestAdaptor.Notifier;
using Serilog.Context;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SearchRequestAdaptor.Consumer
{
    public class SearchRequestOrderedConsumer : IConsumer<SearchRequestOrdered>
    {
        private readonly ILogger<SearchRequestOrderedConsumer> _logger;
        private readonly ISearchRequestNotifier<SearchRequestEvent> _searchRequestNotifier;
        private readonly IOptions<RetryConfiguration>  _retryConfig;

        public SearchRequestOrderedConsumer(ISearchRequestNotifier<SearchRequestEvent> searchRequestNotifier, ILogger<SearchRequestOrderedConsumer> logger, IOptions<RetryConfiguration> retryConfig)
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
                try
                {
                    _logger.LogInformation("get the searchRequestOrdered message.");
                    await _searchRequestNotifier.NotifySearchRequestEventAsync(context.Message.RequestId, context.Message, CancellationToken.None, context.GetRetryAttempt(), _retryConfig.Value.RetryTimes);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "SearchRequestOrdered {requestRef} is put into error queue.", context.Message?.RequestId);
                    throw;
                }
            }
        }

    }

}
