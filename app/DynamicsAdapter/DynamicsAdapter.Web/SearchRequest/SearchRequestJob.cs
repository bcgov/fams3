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

        private readonly ISearchApiClient _searchApiClient;

        public SearchRequestJob(ISearchApiClient searchApiClient, ILogger<SearchRequestJob> logger)
        {
            _logger = logger;
            _searchApiClient = searchApiClient;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Search Request started!");

            _logger.LogDebug("Attempting to post person search");

            var result = await _searchApiClient.SearchAsync(new PersonSearchRequest()
            {
                FirstName = "bcgov",
                LastName = "test"
            });

            _logger.LogInformation($"Successfully posted person search id:{result.Id}");

        }
    }

}
