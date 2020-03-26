using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Threading.Tasks;
using Fams3Adapter.Dynamics.SearchApiRequest;
using OpenTracing;
using Fams3Adapter.Dynamics.Identifier;
using AutoMapper;
using DynamicsAdapter.Web.PersonSearch.Models;

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

        private readonly ISearchApiRequestService _searchApiRequestService;

        private readonly IMapper _mapper;


        public SearchRequestJob(ISearchApiClient searchApiClient,
            ISearchApiRequestService searchApiRequestService,
            ILogger<SearchRequestJob> logger,
            IMapper mapper)
        {
            _logger = logger;
            _searchApiRequestService = searchApiRequestService;
     
            _searchApiClient = searchApiClient;
            _mapper = mapper;
        }

        public async Task Execute(IJobExecutionContext context)
        {

            _logger.LogInformation("Search Request started!");

            var cts = new CancellationTokenSource();

            try 
            {
                List<SSG_SearchApiRequest> requestList = await GetAllReadyForSearchAsync(cts.Token);

                foreach (SSG_SearchApiRequest ssgSearchRequest in requestList)
                {
                    if (ssgSearchRequest.SearchRequestId != Guid.Empty)
                    {
                        _logger.LogDebug(
                            $"Attempting to post person search for request {ssgSearchRequest.SearchApiRequestId}");

                        var result = await _searchApiClient.SearchAsync(
                            _mapper.Map<PersonSearchRequest>(ssgSearchRequest),
                            $"{ssgSearchRequest.SearchApiRequestId}",
                            cts.Token);

                        _logger.LogInformation($"Successfully posted person search id:{result.Id}");

                        await MarkInProgress(ssgSearchRequest, cts.Token);
                    }
                }
            }
            catch(Exception e) 
            {
                _logger.LogError(e, e.Message, null);
            }
        }

        private async Task<List<SSG_SearchApiRequest>> GetAllReadyForSearchAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Attempting to get search request from dynamics");
            var request = await _searchApiRequestService.GetAllReadyForSearchAsync(cancellationToken);
            _logger.LogInformation("Successfully retrieved search requests from dynamics");
            return request.ToList();
        }

        private async Task<SSG_SearchApiRequest> MarkInProgress(SSG_SearchApiRequest ssgSearchRequest,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug(
                    $"Attempting to update searchRequest[{ssgSearchRequest.SearchApiRequestId}] to {SearchApiRequestStatusReason.InProgress.ToString()} status");
                var request =
                    await _searchApiRequestService.MarkInProgress(ssgSearchRequest.SearchApiRequestId,
                        cancellationToken);
                _logger.LogInformation(
                    $"Successfully updated searchRequest[{ssgSearchRequest.SearchApiRequestId}] to {SearchApiRequestStatusReason.InProgress.ToString()} status");
                return request;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updated searchRequest[{ssgSearchRequest.SearchApiRequestId}] to {SearchApiRequestStatusReason.InProgress.ToString()} status", ex);
                throw;
            }
        }

    }

}
