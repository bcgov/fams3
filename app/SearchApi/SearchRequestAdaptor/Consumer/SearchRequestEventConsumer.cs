using BcGov.Fams3.SearchApi.Contracts.SearchRequest;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace SearchRequestAdaptor.Consumer
{
    public class SearchRequestOrderedConsumer : IConsumer<SearchRequestOrdered>
    {
        private readonly ILogger<SearchRequestOrderedConsumer> _logger;

        public SearchRequestOrderedConsumer(ILogger<SearchRequestOrderedConsumer> logger)
        {
            _logger = logger;

        }

        public async Task Consume(ConsumeContext<SearchRequestOrdered> context)
        {
            _logger.LogInformation("get the searchRequestOrdered message.");
        }

    }

}
