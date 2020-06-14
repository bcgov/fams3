﻿using System.Threading;
using System.Threading.Tasks;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using MassTransit;
using Microsoft.Extensions.Logging;
using SearchApi.Web.Notifications;
using Serilog.Context;

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
            using (LogContext.PushProperty("SearchRequestKey",context.Message?.SearchRequestKey))
            {
                var cts = new CancellationTokenSource();
                _logger.LogInformation($"received new {nameof(PersonSearchAdapterEvent)} event from {context.Message.ProviderProfile?.Name}");
                await _searchApiNotifier.NotifyEventAsync(context.Message.SearchRequestKey, context.Message, eventName,
                    cts.Token);
            }
        }
    }



    public class PersonSearchAcceptedConsumer : PersonEventConsumer, IConsumer<PersonSearchAccepted>
    {
        private readonly ILogger<PersonSearchAcceptedConsumer> _logger;
        private readonly ISearchApiNotifier<PersonSearchAdapterEvent> _searchApiNotifier;

        public PersonSearchAcceptedConsumer(ISearchApiNotifier<PersonSearchAdapterEvent> searchApiNotifier, ILogger<PersonSearchAcceptedConsumer> logger) : base(searchApiNotifier, logger)
        {
            _searchApiNotifier = searchApiNotifier;
            _logger = logger;

        }

        public async Task Consume(ConsumeContext<PersonSearchAccepted> context)
        {
            await base.Consume(context, EventName.Accepted);

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
            await base.Consume(context, EventName.Completed);
            await base.Consume(context, EventName.Finalized);
        }

    }

    public class PersonSearchRejectedConsumer : PersonEventConsumer, IConsumer<PersonSearchRejected>
    {
        private readonly ILogger<PersonSearchRejectedConsumer> _logger;
        private readonly ISearchApiNotifier<PersonSearchAdapterEvent> _searchApiNotifier;

        public PersonSearchRejectedConsumer(ISearchApiNotifier<PersonSearchAdapterEvent> searchApiNotifier, ILogger<PersonSearchRejectedConsumer> logger) : base(searchApiNotifier, logger)
        {
            _searchApiNotifier = searchApiNotifier;
            _logger = logger;

        }

        public async Task Consume(ConsumeContext<PersonSearchRejected> context)
        {
      
            await base.Consume(context, EventName.Rejected);
            await base.Consume(context, EventName.Finalized);
           
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
            await base.Consume(context, EventName.Failed);
        }

    }
}