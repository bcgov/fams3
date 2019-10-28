using Microsoft.Extensions.Logging;
using Quartz;
using System.Threading.Tasks;
using DynamicsAdapter.Web.SearchApi;

namespace DynamicsAdapter.Web.SearchRequest
{
    /// <summary>
    /// The SearchRequestJob orchestrates dyanmics search request.
    /// </summary>
    [DisallowConcurrentExecution]
    public class SearchRequestJob : IJob
    {
        private readonly ILogger<SearchRequestJob> _logger;

        private readonly IPeopleClient _peopleClient;

        public SearchRequestJob(IPeopleClient peopleClient, ILogger<SearchRequestJob> logger)
        {
            _logger = logger;
            _peopleClient = peopleClient;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Search Request started!");

            _logger.LogDebug("Attempting to post person search");

            var result = await _peopleClient.SearchAsync(new PersonSearchRequest()
            {
                FirstName = "bcgov",
                LastName = "test"
            });

            _logger.LogDebug($"Successfully posted person search id:{result.Id}");

        }
    }

}
