using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using BcGov.Fams3.SearchApi.Contracts.SearchRequest;
using BcGov.Fams3.SearchApi.Core.Adapters.Configuration;
using BcGov.Fams3.SearchApi.Core.Adapters.Models;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace BcGov.Fams3.SearchApi.Core.Adapters.Middleware
{
    public class SearchRequestObserver : IConsumeMessageObserver<SearchRequestOrdered>
    {
        private readonly ProviderProfile _providerProfile;
        private readonly ILogger<SearchRequestObserver> _logger;

        public SearchRequestObserver(IOptions<ProviderProfileOptions> providerProfile, ILogger<SearchRequestObserver> logger)
        {
            _logger = logger;
            _providerProfile = providerProfile.Value;
        }

        public async Task PreConsume(ConsumeContext<SearchRequestOrdered> context)
        {
            _logger.LogInformation($"Adapter {_providerProfile.Name} provider received new  search request.");
            await Task.FromResult(0);
            return;
        }

        public async Task PostConsume(ConsumeContext<SearchRequestOrdered> context)
        {
            _logger.LogInformation($"Adapter {_providerProfile.Name} provider successfully processed new search request.");
            await Task.FromResult(0);
            return;
        }

        public async Task ConsumeFault(ConsumeContext<SearchRequestOrdered> context, Exception exception)
        {
            _logger.LogError(exception, "Adapter Failed to save search request.");
            await context.Publish<SearchRequestFailed>(new DefaultSearchRequestFailed(context.Message.SearchRequestId, context.Message.RequestId,
                context.Message.SearchRequestKey,
                _providerProfile, exception.Message));
        }
    }
}
