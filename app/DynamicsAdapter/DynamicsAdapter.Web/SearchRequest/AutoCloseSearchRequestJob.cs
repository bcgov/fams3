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
            _logger.LogInformation("autoclose checking sr status!");

            var cts = new CancellationTokenSource();

            try
            {
                List<SSG_SearchRequest> requestList = await GetAutoCloseSearchRequestAsync(cts.Token);
                
                foreach (SSG_SearchRequest ssgSearchRequest in requestList)
                {
                    //process each search request
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message, null);
            }
        }

        private async Task<List<SSG_SearchRequest>> GetAutoCloseSearchRequestAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Attempting to get autoclose status for search request from dynamics");
           
            var request = await _searchRequestService.GetAutoCloseSearchRequestAsync(cancellationToken);

            _logger.LogInformation("Successfully retrieved autoclose status for search requests from dynamics");
            return request.ToList();
        }

    }

}
