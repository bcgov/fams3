using Microsoft.Extensions.Logging;
using Quartz;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.SearchRequest
{
    /// <summary>
    /// The SearchRequestJob orchestrates dyanmics search request.
    /// </summary>
    [DisallowConcurrentExecution]
    public class SearchRequestJob : IJob
    {
        private readonly ILogger<SearchRequestJob> _logger;
        public SearchRequestJob(ILogger<SearchRequestJob> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Search Request started!");
            return Task.CompletedTask;
        }
    }

}
