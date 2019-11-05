using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using SearchApi.Core.Adapters.Contracts;

namespace SearchApi.Web.Search
{
    public class MatchFoundConsumer : IConsumer<MatchFound>
    {

        private readonly ILogger<MatchFoundConsumer> _logger;

        public MatchFoundConsumer(ILogger<MatchFoundConsumer> logger)
        {
            this._logger = logger;
        }

        public async Task Consume(ConsumeContext<MatchFound> context)
        {
            this._logger.LogInformation($"received new {nameof(MatchFound)} event");
            await Task.FromResult(0);
        }
    }
}