using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using SearchApi.Core.Adapters.Contracts;
using SearchApi.Web.Notifications;

namespace SearchApi.Web.Search
{
    public class MatchFoundConsumer : IConsumer<MatchFound>
    {

        private readonly ILogger<MatchFoundConsumer> _logger;

        private readonly ISearchApiNotifier<MatchFound> _searchApiNotifier;

        public MatchFoundConsumer(ISearchApiNotifier<MatchFound> searchApiNotifier, ILogger<MatchFoundConsumer> logger)
        {
            _searchApiNotifier = searchApiNotifier;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<MatchFound> context)
        {
            var cts = new CancellationTokenSource();
            var profile = context.Headers.Get<ProviderProfile>(nameof(ProviderProfile));
            _logger.LogInformation($"received new {nameof(MatchFound)} event from {profile.Name}");
            await _searchApiNotifier.NotifyEventAsync(context.Message.SearchRequestId, context.Message,
                cts.Token);
        }
    }
}