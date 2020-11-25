using BcGov.Fams3.SearchApi.Contracts.IA;
using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Core.Adapters.Models;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BcGov.Fams3.SearchApi.Core.Adapters.Middleware
{
    public class IASearchObserver : IConsumeMessageObserver<IASearchOrdered>
    {

        private readonly ILogger<IASearchObserver> _logger;

        public IASearchObserver(ILogger<IASearchObserver> logger)
        {
            _logger = logger;

        }

        public async Task PreConsume(ConsumeContext<IASearchOrdered> context)
        {
            _logger.LogInformation($"IA Search recieved and pre consuming");
            await Task.FromResult(0);
            return;
        }

        public async Task PostConsume(ConsumeContext<IASearchOrdered> context)
        {
            _logger.LogInformation($"IA Search request processed");
            await Task.FromResult(0);
            return;
        }

        public async Task ConsumeFault(ConsumeContext<IASearchOrdered> context, Exception exception)
        {
            _logger.LogError(exception, "Adapter Failed to perform IA search request.");
            await context.Publish<IASearchFailed>(new DefaultIASearchFailed(context.Message.SearchRequestId, context.Message.SearchRequestKey,
                new Person { }, context.Message.BatchNo));
        }
    }
}