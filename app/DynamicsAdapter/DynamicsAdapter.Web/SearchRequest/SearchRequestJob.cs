using AutoMapper;
using DynamicsAdapter.Web.Configuration;
using DynamicsAdapter.Web.Register;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

        private readonly SearchApiConfiguration _searchApiConfiguration;

        public SearchRequestJob(ISearchApiClient searchApiClient,
            ISearchApiRequestService searchApiRequestService,
            ILogger<SearchRequestJob> logger,
            IMapper mapper,
            ISearchRequestRegister register,
            IOptions<SearchApiConfiguration> searchApiConfigurationOptions)
        {
            _logger = logger;
            _searchApiRequestService = searchApiRequestService;
            _searchApiClient = searchApiClient;
            _mapper = mapper;
            _register = register;
            _searchApiConfiguration = searchApiConfigurationOptions.Value;
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
                        if (!await _register.SearchForSearchRequestKeys(ssgSearchRequest))
                        {
                            try
                            {
                                using (LogContext.PushProperty("SearchRequestKey", $"{ssgSearchRequest.SearchRequest?.FileId}_{ssgSearchRequest.SequenceNumber}"))
                                {
                                    _logger.LogDebug(
                                       $"Attempting to post person search for request {ssgSearchRequest.SearchApiRequestId}");

                                    SSG_SearchApiRequest request = _register.FilterDuplicatedIdentifier(ssgSearchRequest);

                                    bool registerSuccessfully = await _register.RegisterSearchApiRequest(request);

                                    if (registerSuccessfully)
                                    {
                                        try
                                        {
                                    
                                            var result = await _searchApiClient.SearchAsync(
                                                _mapper.Map<PersonSearchRequest>(request),
                                                $"{request.SearchApiRequestId}",
                                                cts.Token);
                                          
                                            _logger.LogInformation($"Successfully posted person search id:{result.Id}");
                                            await MarkInProgress(ssgSearchRequest, cts.Token);

                                        }
                                        catch (Exception ex)
                                        {
                                            _logger.LogError(ex, "Send request to SearchApi failed", null);
                                            await _register.RemoveSearchApiRequest(request);
                                        }
                                   
                                    }
                                    else
                                    {
                                        throw new RegisterFailedException("Register SearchApiRequest to cache failed.");
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e, e.Message, null);
                                await _register.RemoveSearchApiRequest(ssgSearchRequest);
                            }
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
           
            var request = await _searchApiRequestService.GetAllReadyForSearchAsync(cancellationToken, await _register.GetDataProvidersList(), _searchApiConfiguration.AvailableDataPartner );

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
