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
    public class NotificationAcknowledgedConsumer : IConsumer<NotificationAcknowledged>
    {
        private readonly ILogger<NotificationAcknowledgedConsumer> _logger;
        private readonly ISearchRequestNotifier<SearchRequestEvent> _searchRequestNotifier;
        private readonly IOptions<RetryConfiguration>  _retryConfig;

        public NotificationAcknowledgedConsumer(ISearchRequestNotifier<SearchRequestEvent> searchRequestNotifier, ILogger<NotificationAcknowledgedConsumer> logger, IOptions<RetryConfiguration> retryConfig)
        {
            _logger = logger;
            _searchRequestNotifier = searchRequestNotifier;
            _retryConfig = retryConfig;

        }

        public async Task Consume(ConsumeContext<NotificationAcknowledged> context)
        {
            using (LogContext.PushProperty("RequestRef", $"{context.Message?.RequestId}"))
            using (LogContext.PushProperty("AgencyCode", $"{context.Message?.ProviderProfile?.Name}"))
            {
                _logger.LogInformation("get the NotificationAcknowledged message.");
                var cts = new CancellationTokenSource();
                await _searchRequestNotifier.NotifySearchRequestEventAsync(context.Message.RequestId, context.Message, cts.Token, context.GetRetryAttempt(), _retryConfig.Value.RetryTimes);
            }
        }

    }

}
