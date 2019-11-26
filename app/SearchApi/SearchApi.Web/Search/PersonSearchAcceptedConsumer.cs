using MassTransit;
using Microsoft.Extensions.Logging;
using SearchApi.Core.Adapters.Contracts;
using SearchApi.Web.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SearchApi.Web.Search
{
    public class PersonSearchAcceptedConsumer : IConsumer<PersonSearchAccepted>
    {

        private readonly ILogger<PersonSearchAcceptedConsumer> _logger;

        private readonly ISearchApiNotifier<PersonSearchAccepted> _searchApiNotifier;

        public PersonSearchAcceptedConsumer(ISearchApiNotifier<PersonSearchAccepted> searchApiNotifier, ILogger<PersonSearchAcceptedConsumer> logger)
        {
            _searchApiNotifier = searchApiNotifier;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PersonSearchAccepted> context)
        {
            var cts = new CancellationTokenSource();
            var profile = context.Headers.Get<ProviderProfile>(nameof(ProviderProfile));
            _logger.LogInformation($"received new {nameof(PersonSearchAccepted)} event from {profile.Name}");
            await _searchApiNotifier.NotifyEventAsync(context.Message.SearchRequestId, context.Message,
                cts.Token);
        }
    }
}
