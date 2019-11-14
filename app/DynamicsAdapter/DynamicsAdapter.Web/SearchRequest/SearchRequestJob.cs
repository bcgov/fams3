using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Threading.Tasks;
using DynamicsAdapter.Web.Services.Dynamics.Model;
using OpenTracing;

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

        private readonly ISearchRequestService _searchRequestService;


        public SearchRequestJob(ISearchApiClient searchApiClient,
            ISearchRequestService searchRequestService,
            ILogger<SearchRequestJob> logger)
        {
            _logger = logger;
            _searchRequestService = searchRequestService;
     
            _searchApiClient = searchApiClient;
        }

        public async Task Execute(IJobExecutionContext context)
        {

            _logger.LogInformation("Search Request started!");

            var cts = new CancellationTokenSource();


            List<SSG_SearchApiRequest> requestList = await GetAllReadyForSearchAsync(cts.Token);
            foreach (var ssgSearchRequest in requestList)
            {
                _logger.LogDebug(
                    $"Attempting to post person search for request {ssgSearchRequest.SSG_SearchApiRequestId}");

                var result = await _searchApiClient.SearchAsync(new PersonSearchRequest()
                {
                    FirstName = ssgSearchRequest.SSG_PersonGivenName,
                    LastName = ssgSearchRequest.SSG_PersonSurname,
                    DateOfBirth = ssgSearchRequest.SSG_PersonBirthDate
                }, $"{ssgSearchRequest.SSG_SearchApiRequestId}", cts.Token);
                _logger.LogInformation($"Successfully posted person search id:{result.Id}");
            }

        }

        private async Task<List<SSG_SearchApiRequest>> GetAllReadyForSearchAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Attempting to get search request from dynamics");
            var request = await _searchRequestService.GetAllReadyForSearchAsync(cancellationToken);
            _logger.LogInformation("Successfully retrieved search requests from dynamics");
            return request.ToList();
        }

    }

}
