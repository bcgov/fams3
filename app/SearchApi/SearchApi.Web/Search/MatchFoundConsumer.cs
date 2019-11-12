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

        private readonly ISearchApiNotifier _serSearchApiNotifier;

        public MatchFoundConsumer(ISearchApiNotifier serSearchApiNotifier, ILogger<MatchFoundConsumer> logger)
        {
            _serSearchApiNotifier = serSearchApiNotifier;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<MatchFound> context)
        {
            var cts = new CancellationTokenSource();
            var profile = context.Headers.Get<ProviderProfile>(nameof(ProviderProfile));
            _logger.LogInformation($"received new {nameof(MatchFound)} event from {profile.Name}");
            await _serSearchApiNotifier.NotifyMatchFoundAsync((Guid) context.ConversationId, context.Message,
                cts.Token);
        }
    }
}