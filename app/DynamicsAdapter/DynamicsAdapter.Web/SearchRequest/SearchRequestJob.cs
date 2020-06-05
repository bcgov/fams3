using AutoMapper;
using DynamicsAdapter.Web.Register;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Microsoft.Extensions.Logging;
using Quartz;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        private readonly ISearchApiRequestService _searchApiRequestService;

        private readonly IMapper _mapper;

        private readonly ISearchRequestRegister _register;


        public SearchRequestJob(ISearchApiClient searchApiClient,
            ISearchApiRequestService searchApiRequestService,
            ILogger<SearchRequestJob> logger,
            IMapper mapper,
            ISearchRequestRegister register)
        {
            _logger = logger;
            _searchApiRequestService = searchApiRequestService;

            _searchApiClient = searchApiClient;
            _mapper = mapper;
            _register = register;
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
                        using (LogContext.PushProperty("FileId", ssgSearchRequest.SearchRequest?.FileId))
                        {
                            _logger.LogDebug(
                               $"Attempting to post person search for request {ssgSearchRequest.SearchApiRequestId}");

                            SSG_SearchApiRequest request = _register.FilterDuplicatedIdentifier(ssgSearchRequest);

                            await _register.RegisterSearchApiRequest(request);

                            var result = await _searchApiClient.SearchAsync(
                                _mapper.Map<PersonSearchRequest>(request),
                                $"{request.SearchApiRequestId}",
                                cts.Token);

                            _logger.LogInformation($"Successfully posted person search id:{result.Id}");

                            await MarkInProgress(ssgSearchRequest, cts.Token);
                        }
                    }
                }
            }
            catch (Exception e)
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

        private async Task<SSG_SearchApiRequest> MarkComplete(SSG_SearchApiRequest ssgSearchRequest,
          CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug(
                    $"Attempting to update searchRequest[{ssgSearchRequest.SearchApiRequestId}] to {SearchApiRequestStatusReason.InProgress.ToString()} status");
                var request =
                    await _searchApiRequestService.MarkComplete(ssgSearchRequest.SearchApiRequestId,
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
