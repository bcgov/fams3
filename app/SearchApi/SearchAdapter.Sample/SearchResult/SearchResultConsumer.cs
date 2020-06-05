using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using BcGov.Fams3.SearchApi.Core.Adapters.Configuration;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SearchAdapter.Sample.SearchRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace SearchAdapter.Sample.SearchResult
{

  
  
    public class SearchResultConsumer : IConsumer<PersonSearchReceived>
    {

        private readonly ILogger<SearchResultConsumer> _logger;
        private readonly ProviderProfile _profile;


        public SearchResultConsumer(
            IOptions<ProviderProfileOptions> profile,
            ILogger<SearchResultConsumer> logger)
        {
          
            _profile = profile.Value;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PersonSearchReceived> context)
        {
            _logger.LogInformation($"Successfully handling  search result");

            _logger.LogInformation($"Successfully handling  search result [{context.Message.ReceivedPayload}]");

            _logger.LogWarning("Sample Adapter, do not use in PRODUCTION.");

            await context.Publish(FakePersonBuilder.BuildFakePersonSearchCompleted(context.Message.SearchRequestId, context.Message.FileId, "FirstName", "LastName",DateTime.Now, _profile));
        }
     
    }
}
