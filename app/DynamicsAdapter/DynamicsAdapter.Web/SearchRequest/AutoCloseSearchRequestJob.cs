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
                _logger.LogInformation("Autoclose: Successfully Retrieved "+ requestList.Count+" Records");
                foreach (SSG_SearchRequest ssgSearchRequest in requestList)
                {
                    if (ssgSearchRequest.AutoCloseStatus == SearchRequestAutoCloseStatusCode.NoCPMatch.Value ||
                        ssgSearchRequest.AutoCloseStatus == SearchRequestAutoCloseStatusCode.CPMissingData.Value)
                    {
                        _logger.LogInformation("Autoclose: Calling ssg_SearchRequestCreateCouldNotAutoCloseNote SearchRequestId=" + ssgSearchRequest.SearchRequestId + " AutoCloseStatus "+ssgSearchRequest.AutoCloseStatus+"");
                        await _searchRequestService.SearchRequestCreateCouldNotAutoCloseNote(ssgSearchRequest.SearchRequestId);
                        _logger.LogInformation("Autoclose: ssg_SearchRequestCreateCouldNotAutoCloseNote successfully");
                    }
                    else
                    {
                        if (ssgSearchRequest.AutoCloseStatus == SearchRequestAutoCloseStatusCode.ReadyToClose.Value)
                        {
                            IDictionary<string, object> updatedFields = new Dictionary<string, object>
                            {
                                { "statuscode", SearchRequestStatusCode.SearchRequestAutoClosed.Value}
                            };
                            _logger.LogInformation("Autoclose: Updating Search Status SearchRequestId=" + ssgSearchRequest.SearchRequestId + "");
                            await _searchRequestService.UpdateSearchRequest(ssgSearchRequest.SearchRequestId, updatedFields, cts.Token);
                            _logger.LogInformation("Autoclose: Updated Search Request successfully");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation("Autoclose: Encountered Unexpected Exception");
                _logger.LogError(e, e.Message, null);
            }
        }

        private async Task<List<SSG_SearchRequest>> GetAutoCloseSearchRequestAsync(CancellationToken cancellationToken)
        {
            var request = await _searchRequestService.GetAutoCloseSearchRequestAsync(cancellationToken);
            return request.ToList();
        }

    }

}
