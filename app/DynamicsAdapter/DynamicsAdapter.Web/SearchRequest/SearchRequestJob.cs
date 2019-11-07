using System.Threading;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Threading.Tasks;
using DynamicsAdapter.Web.Auth;

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

        private readonly IOAuthApiClient _oAuthApiClient;

        public SearchRequestJob(ISearchApiClient searchApiClient, 
            IOAuthApiClient oAuthApiClient,
            ILogger<SearchRequestJob> logger)
        {
            _logger = logger;
            _oAuthApiClient = oAuthApiClient;
            _searchApiClient = searchApiClient;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Search Request started!");


            _logger.LogDebug("Attempting to get token");
            var token = await _oAuthApiClient.GetRefreshToken(CancellationToken.None);
            _logger.LogInformation($"Successfully got token from provider, token expires in {token.ExpiresIn}");

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
