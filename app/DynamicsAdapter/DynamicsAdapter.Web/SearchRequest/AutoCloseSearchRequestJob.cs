using AutoMapper;
using DynamicsAdapter.Web.Configuration;
using DynamicsAdapter.Web.Register;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Fams3Adapter.Dynamics.SearchRequest;
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
    /// The AutoCloseSearchRequestJob orchestrates dyanmics auto-close search requests.
    /// </summary>
    [DisallowConcurrentExecution]
    public class AutoCloseSearchRequestJob : IJob
    {
        private readonly ILogger<AutoCloseSearchRequestJob> _logger;

        private readonly ISearchRequestService _searchRequestService;

        //private readonly IMapper _mapper;

        //private readonly ISearchRequestRegister _register;

        public AutoCloseSearchRequestJob(
            ISearchRequestService searchRequestService,
            ILogger<AutoCloseSearchRequestJob> logger
            //IMapper mapper,
            //ISearchRequestRegister register
            )
        {
            _logger = logger;
            _searchRequestService = searchRequestService;
            //_mapper = mapper;
            //_register = register;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Autoclose: Initiating job");
            var cts = new CancellationTokenSource();
            try
            {
                _logger.LogInformation("Autoclose: Checking for Search Requests");
                List<SSG_SearchRequest> requestList = await GetAutoCloseSearchRequestAsync(cts.Token);
                _logger.LogInformation("Autoclose: Successfully Retrieved {RecordsCount}", requestList.Count);
                int count = 0;
                foreach (SSG_SearchRequest ssgSearchRequest in requestList)
                {
                    if (ssgSearchRequest.AutoCloseStatus == SearchRequestAutoCloseStatusCode.NoCPMatch.Value ||
                        ssgSearchRequest.AutoCloseStatus == SearchRequestAutoCloseStatusCode.CPMissingData.Value)
                    {
                        _logger.LogInformation("Autoclose: Calling ssg_SearchRequestCreateCouldNotAutoCloseNote {SearchRequestId} {AutoCloseStatus}",ssgSearchRequest.SearchRequestId, ssgSearchRequest.AutoCloseStatus);
                        try
                        {
                            await _searchRequestService.SearchRequestCreateCouldNotAutoCloseNote(ssgSearchRequest.SearchRequestId);
                            count++;
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "Autoclose: Encountered Unexpected Exception", null);
                        }
                        _logger.LogInformation("Autoclose: ssg_SearchRequestCreateCouldNotAutoCloseNote successfully");
                    }
                    else
                    {
                        if (ssgSearchRequest.AutoCloseStatus == SearchRequestAutoCloseStatusCode.ReadyToClose.Value)
                        {
                            _logger.LogInformation("Autoclose: Updating Search Status {SearchRequestId}",  ssgSearchRequest.SearchRequestId);
                            try
                            {
                                await _searchRequestService.UpdateSearchRequestStatusAutoClosed(ssgSearchRequest.SearchRequestId, cts.Token);
                                count++;
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e, "Autoclose: Encountered Unexpected Exception", null);
                            }
                            _logger.LogInformation("Autoclose: Updated Search Request successfully");
                        }
                    }
                }
                _logger.LogInformation("Autoclose: Successfully Processed {SearchRequestCount} Records", count);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Autoclose: Encountered Unexpected Exception", null);
            }
        }

        private async Task<List<SSG_SearchRequest>> GetAutoCloseSearchRequestAsync(CancellationToken cancellationToken)
        {
            var request = await _searchRequestService.GetAutoCloseSearchRequestAsync(cancellationToken);
            return request.ToList();
        }

    }

}
