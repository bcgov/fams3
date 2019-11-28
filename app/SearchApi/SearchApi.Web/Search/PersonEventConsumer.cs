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
    public abstract class PersonEventConsumer
    {

        private readonly ILogger<PersonEventConsumer> _logger;
    

        private readonly ISearchApiNotifier<PersonSearchAdapterEvent> _searchApiNotifier;

        public PersonEventConsumer(ISearchApiNotifier<PersonSearchAdapterEvent> searchApiNotifier, ILogger<PersonEventConsumer> logger)
        {
            _searchApiNotifier = searchApiNotifier;
            _logger = logger;
          
        }

        public async Task Consume(ConsumeContext<PersonSearchAdapterEvent> context)
        {
            var cts = new CancellationTokenSource();
            var profile = context.Headers.Get<ProviderProfile>(nameof(ProviderProfile));
            _logger.LogInformation($"received new {nameof(PersonFound)} event from {profile.Name}");
            await _searchApiNotifier.NotifyEventAsync(context.Message.SearchRequestId, context.Message,
                cts.Token);

        }
    }



    public class PersonSearchAcceptedConsumer : PersonEventConsumer, IConsumer<PersonSearchAccepted>
    {
        private readonly ILogger<PersonSearchAcceptedConsumer> _logger;
        private readonly ISearchApiNotifier<PersonSearchAdapterEvent> _searchApiNotifier;

        public PersonSearchAcceptedConsumer(ISearchApiNotifier<PersonSearchAdapterEvent> searchApiNotifier, ILogger<PersonSearchAcceptedConsumer> logger ) : base( searchApiNotifier, logger)
        {
            _searchApiNotifier = searchApiNotifier;
            _logger = logger;

        }

        public async Task Consume(ConsumeContext<PersonSearchAccepted> context)
        {
            await base.Consume(context);
        }

    }
}