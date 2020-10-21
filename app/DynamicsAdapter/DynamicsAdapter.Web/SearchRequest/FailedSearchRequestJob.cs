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
    public class FailedSearchRequestJob : IJob
    {
        private readonly ILogger<FailedSearchRequestJob> _logger;

        private readonly ISearchApiClient _searchApiClient;

        private readonly ISearchApiRequestService _searchApiRequestService;

        private readonly IMapper _mapper;

        private readonly ISearchRequestRegister _register;


        public FailedSearchRequestJob(ISearchApiClient searchApiClient,
            ISearchApiRequestService searchApiRequestService,
            ILogger<FailedSearchRequestJob> logger,
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

            _logger.LogInformation("Retreiving failed Search Request started!");

            var cts = new CancellationTokenSource();

            try
            {
                List<SSG_SearchApiRequest> requestList = await GetAllFailedForSearchAsync(cts.Token);

                foreach (SSG_SearchApiRequest ssgSearchRequest in requestList)
                {
                    if (ssgSearchRequest.SearchRequestId != Guid.Empty)
                    {
                        try
                        {
                            using (LogContext.PushProperty("SearchRequestKey", $"{ssgSearchRequest.SearchRequest?.FileId}_{ssgSearchRequest.SequenceNumber}"))
                            {
                                _logger.LogDebug(
                                   $"Attempting to post failed person search for request {ssgSearchRequest.SearchApiRequestId}");

                                SSG_SearchApiRequest request = _register.FilterDuplicatedIdentifier(ssgSearchRequest);

                                bool registerSuccessfully = await _register.RegisterSearchApiRequest(request);

                                if (registerSuccessfully)
                                {
                                    var result = await _searchApiClient.People_SearchAsync(
                                        _mapper.Map<PersonSearchRequest>(request),
                                        $"{request.SearchApiRequestId}",
                                        cts.Token);

                                    _logger.LogInformation($"Successfully posted person search id:{result.Id}");

                                    
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
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message, null);
            }
        }


        private async Task<List<SSG_SearchApiRequest>> GetAllFailedForSearchAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Attempting to get failed search request that can be retried from dynamics");

            var request = await _searchApiRequestService.GetAllValidFailedSearchRequest(cancellationToken, await _register.GetDataProvidersList());

            _logger.LogInformation("Successfully retrieved failed search requests that can be retried from dynamics");
            return request.ToList();
        }


   

    }

}
