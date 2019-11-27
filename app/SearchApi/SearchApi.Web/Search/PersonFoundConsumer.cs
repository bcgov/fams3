using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MassTransit;
using Microsoft.Extensions.Logging;
using SearchApi.Core.Adapters.Contracts;
using SearchApi.Core.Adapters.Models;
using SearchApi.Web.Notifications;

namespace SearchApi.Web.Search
{
    public class PersonFoundConsumer : IConsumer<PersonFound>
    {

        private readonly ILogger<PersonFoundConsumer> _logger;
    

        private readonly ISearchApiNotifier<PersonFound> _searchApiNotifier;

        public PersonFoundConsumer(ISearchApiNotifier<PersonFound> searchApiNotifier, ILogger<PersonFoundConsumer> logger)
        {
            _searchApiNotifier = searchApiNotifier;
            _logger = logger;
          
        }

        public async Task Consume(ConsumeContext<PersonFound> context)
        {
            var cts = new CancellationTokenSource();
            var profile = context.Headers.Get<ProviderProfile>(nameof(ProviderProfile));
            _logger.LogInformation($"received new {nameof(PersonFound)} event from {profile.Name}");
            await _searchApiNotifier.NotifyEventAsync(context.Message.SearchRequestId, context.Message,
                cts.Token);

        }
    }
}