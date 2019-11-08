using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Threading.Tasks;
using DynamicsAdapter.Web.Auth;
using DynamicsAdapter.Web.Services.Dynamics;
using DynamicsAdapter.Web.Services.Dynamics.Model;

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

        private readonly IDynamicsApiClient _dynamicsApiClient;

        public SearchRequestJob(ISearchApiClient searchApiClient,
            IDynamicsApiClient dynamicsApiClient,
            ILogger<SearchRequestJob> logger)
        {
            _logger = logger;
            _dynamicsApiClient = dynamicsApiClient;
            _searchApiClient = searchApiClient;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Search Request started!");


            _logger.LogDebug("Attempting to get search request from dynamics");
            var request = await _dynamicsApiClient.Get<SSG_SearchRequests>(
                $"ssg_searchrequests({Guid.NewGuid()})");
            if (request == null) return;

            _logger.LogInformation($"Successfully got entity from dynamics");

            _logger.LogDebug("Attempting to post person search");

            

            var result = await _searchApiClient.SearchAsync(new PersonSearchRequest()
            {
                FirstName = request.SSG_PersonGivenName,
                LastName = request.SSG_PersonSurname,
                DateOfBirth = request.SSG_PersonBirthDate
            });
            _logger.LogInformation($"Successfully posted person search id:{result.Id}");

        }
    }

}
