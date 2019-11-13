using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using SearchApi.Core.Adapters.Contracts;
using SearchApi.Core.Contracts;

namespace SearchAdapter.ICBC.SearchRequest
{
    /// <summary>
    /// The SearchRequestConsumer consumes ICBC execute search commands, execute the search a publish a response back to the searchApi
    /// </summary>
    public class SearchRequestConsumer : IConsumer<ExecuteSearch>
    {

        private readonly ILogger<SearchRequestConsumer> _logger;

        public SearchRequestConsumer(ILogger<SearchRequestConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ExecuteSearch> context)
        {
            _logger.LogInformation($"Successfully handling new search request [{context.Message.Id}]");
            
            await context.Publish<MatchFound>(new IcbcMatchFoundBuilder(context.Message.Id)
                .WithFirstName(context.Message.FirstName)
                .WithLastName(context.Message.LastName)
                .WithDateOfBirth(context.Message.DateOfBirth)
                .Build());
        }
    }
}