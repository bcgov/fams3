using System;
using System.Threading.Tasks;
using BcGov.Fams3.SearchApi.Core.Adapters.Configuration;
using BcGov.Fams3.SearchApi.Core.Adapters.Contracts;
using BcGov.Fams3.SearchApi.Core.Adapters.Models;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BcGov.Fams3.SearchApi.Core.Adapters.Middleware
{
    public class PersonSearchObserver : IConsumeMessageObserver<PersonSearchOrdered>
    {

        private readonly ProviderProfile _providerProfile;
        private readonly ILogger<PersonSearchObserver> _logger;

        public PersonSearchObserver(IOptions<ProviderProfileOptions> providerProfile, ILogger<PersonSearchObserver> logger)
        {
            _logger = logger;
            _providerProfile = providerProfile.Value;
        }

        public async Task PreConsume(ConsumeContext<PersonSearchOrdered> context)
        {
            _logger.LogInformation($"Adapter {_providerProfile.Name} provider received new person search request.");
            await Task.FromResult(0);
            return;
        }

        public async Task PostConsume(ConsumeContext<PersonSearchOrdered> context)
        {
            _logger.LogInformation($"Adapter {_providerProfile.Name} provider successfully processed new person search request.");
            await Task.FromResult(0);
            return;
        }

        public async Task ConsumeFault(ConsumeContext<PersonSearchOrdered> context, Exception exception)
        {
            _logger.LogError(exception, "Adapter Failed to execute person search.");
                await context.Publish<PersonSearchFailed>(new DefaultPersonSearchFailed(context.Message.SearchRequestId,
                    _providerProfile, exception.Message));
        }
    }
}