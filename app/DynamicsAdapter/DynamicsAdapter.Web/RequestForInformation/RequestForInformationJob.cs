using AutoMapper;
using DynamicsAdapter.Web.Register;
using Fams3Adapter.Dynamics.RfiService;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Microsoft.Extensions.Logging;
using Quartz;
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
    public class RequestForInformationJob : IJob
    {
        private readonly ILogger<SearchRequestJob> _logger;

        private readonly IRfiSubmittalService _rfiService;
        private readonly ISearchApiClient _searchApiClient;
		private readonly ISearchRequestRegister _register;
        private readonly IMapper _mapper;

        public RequestForInformationJob(IRfiSubmittalService rfiSubmittalService,
            ILogger<SearchRequestJob> logger,
			ISearchRequestRegister register,
            ISearchApiClient searchApiClient,
            IMapper mapper)
        {
            _logger = logger;
            _rfiService = rfiSubmittalService;
			_register = register;
            _searchApiClient = searchApiClient;
            _mapper = mapper;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("RFI Polling started!");

            var cts = new CancellationTokenSource();

            try
            {
                List<SSG_RfiMessage> rfiList = await GetAllReadyForSendAsync(cts.Token);
                _logger.LogInformation($"RFI processing {rfiList.Count()} records!");
                
                foreach (SSG_RfiMessage rfi in rfiList)
                {
                    var result = await _searchApiClient.Rfi_SendAsync(_mapper.Map<RequestForInformation>(rfi),rfi.Id.ToString(),cts.Token);

                    _logger.LogInformation($"Successfully posted rfi:{result.Id}");
                    await MarkInProgress(rfi, cts.Token);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message, null);
            }
        }

        private async Task<List<SSG_RfiMessage>> GetAllReadyForSendAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Attempting to get RFI letters from dynamics");
           
            var request = await _rfiService.GetAllReadyForSendAsync(cancellationToken,await _register.GetDataProvidersList());

            _logger.LogInformation("Successfully retrieved RFI letters from dynamics");
            return request.ToList();
        }

        private async Task<SSG_RfiMessage> MarkInProgress(SSG_RfiMessage ssgRfi,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug(
                    $"Attempting to update rfi[{ssgRfi.Id}] to {RfiStatusCodes.InProgress.ToString()} status");
                var request = await _rfiService.MarkInProgress(ssgRfi.Id, cancellationToken);
                _logger.LogInformation(
                    $"Successfully updated rfi[{ssgRfi.Id}] to {RfiStatusCodes.InProgress.ToString()} status");
                return request;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updated rfi[{ssgRfi.Id}] to {RfiStatusCodes.InProgress.ToString()} status", ex);
                throw;
            }
        }

    }

}
