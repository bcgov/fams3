using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using SearchApi.Core.Contracts;

namespace SearchAdapter.ICBC.SearchRequest
{
    public class SearchRequestHandler : IConsumer<ExecuteSearch>
    {

        private readonly ILogger<SearchRequestHandler> _logger;

        public SearchRequestHandler(ILogger<SearchRequestHandler> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ExecuteSearch> context)
        {
            _logger.LogInformation($"Successfully handling new search request [{context.Message.Id}]");
            await Task.FromResult(0);
        }
    }
}