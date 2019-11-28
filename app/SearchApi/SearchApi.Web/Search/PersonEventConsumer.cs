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

        public async Task Consume(ConsumeContext<PersonSearchAdapterEvent> context, string eventName)
        {
            var cts = new CancellationTokenSource();
            _logger.LogInformation($"received new {nameof(PersonSearchAdapterEvent)} event from {context.Message.ProviderProfile?.Name}");
            await _searchApiNotifier.NotifyEventAsync(context.Message.SearchRequestId, context.Message, eventName,
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
            await base.Consume(context, "Accepted");
        }

    }

    public class PersonSearchCompletedConsumer : PersonEventConsumer, IConsumer<PersonSearchCompleted>
    {
        private readonly ILogger<PersonSearchCompletedConsumer> _logger;
        private readonly ISearchApiNotifier<PersonSearchAdapterEvent> _searchApiNotifier;

        public PersonSearchCompletedConsumer(ISearchApiNotifier<PersonSearchAdapterEvent> searchApiNotifier, ILogger<PersonSearchCompletedConsumer> logger) : base(searchApiNotifier, logger)
        {
            _searchApiNotifier = searchApiNotifier;
            _logger = logger;

        }

        public async Task Consume(ConsumeContext<PersonSearchCompleted> context)
        {
            await base.Consume(context, "Completed");
        }

    }



    public class PersonSearchFailedConsumer : PersonEventConsumer, IConsumer<PersonSearchFailed>
    {
        private readonly ILogger<PersonSearchFailedConsumer> _logger;
        private readonly ISearchApiNotifier<PersonSearchAdapterEvent> _searchApiNotifier;

        public PersonSearchFailedConsumer(ISearchApiNotifier<PersonSearchAdapterEvent> searchApiNotifier, ILogger<PersonSearchFailedConsumer> logger) : base(searchApiNotifier, logger)
        {
            _searchApiNotifier = searchApiNotifier;
            _logger = logger;

        }

        public async Task Consume(ConsumeContext<PersonSearchFailed> context)
        {
            await base.Consume(context, "Failed");
        }

    }
}