using BcGov.Fams3.SearchApi.Contracts.SearchRequest;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgencyAdapter.Sample.SearchRequest
{
    public class SearchRequestResponseConsumer : IConsumer<SearchRequestNotification>
    {

        private readonly ILogger<SearchRequestResponseConsumer> _logger;


        public SearchRequestResponseConsumer(

            ILogger<SearchRequestResponseConsumer> logger)
        {

            _logger = logger;
        }

        public async Task Consume(ConsumeContext<SearchRequestNotification> context)
        {
            _logger.LogInformation($"Successfully handling new notification for search request [{context.Message.Notification}]");

            _logger.LogWarning("Sample Agency Adapter, do not use in PRODUCTION.");

            _logger.LogInformation($"Send message out to agency");

            await context.Publish(FakeSearchrequestResponseBuilder.BuildFakeSearchRequestNotification(context.Message.SearchRequestId, context.Message.SearchRequestKey, context.Message.RequestId, NotificationType.RequestAssignedToFSO, "FMEP"));

        }
    }
}
