using AutoMapper;
using DynamicsAdapter.Web.PersonSearch.Models;
using DynamicsAdapter.Web.Register;
using Fams3Adapter.Dynamics.DataProvider;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.SearchApiEvent;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Types;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.PersonSearch
{
    [DisallowConcurrentExecution]
    public class SearchResultUploadJob : IJob
    {
        private readonly ILogger<SearchResultUploadJob> _logger;
        private readonly ISearchResultService _searchResultService;
        private readonly ISearchApiRequestService _searchApiRequestService;
        private readonly ISearchResultQueue _searchResultQueue;
        private readonly IMapper _mapper;
        private readonly ISearchRequestRegister _register;
        private readonly IDataPartnerService _dataPartnerService;
        private bool _inProcessing = false;

        public SearchResultUploadJob(ISearchResultQueue searchResultQueue, ISearchResultService searchResultService, ILogger<SearchResultUploadJob> logger, IMapper mapper,
            ISearchRequestRegister register, ISearchApiRequestService searchApiRequestService, IDataPartnerService dataPartnerService)
        {
            _logger = logger;
            _searchResultQueue = searchResultQueue;
            _searchResultService = searchResultService;
            _searchApiRequestService = searchApiRequestService;
            _mapper = mapper;
            _register = register;
            _dataPartnerService = dataPartnerService;
            _inProcessing = false;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try { 
                _logger.LogInformation("Search Request started!");
                PersonSearchCompleted completeEvent = _searchResultQueue.Dequeue();

                if (completeEvent != null && !_inProcessing)
                {
                    _inProcessing = true;
                    _logger.LogInformation("Received Person search completed event");
                    var cts = new CancellationTokenSource();
                    SSG_SearchApiRequest request = await _register.GetSearchApiRequest(completeEvent?.SearchRequestKey);
                    //update completed event
                    var searchApiEvent = _mapper.Map<SSG_SearchApiEvent>(completeEvent);
                    _logger.LogDebug($"Attempting to create a new event for SearchApiRequest");
                    await _searchApiRequestService.AddEventAsync(request.SearchApiRequestId, searchApiEvent, cts.Token);
                    _logger.LogInformation($"Successfully created completed event for SearchApiRequest");

                    //upload search result to dynamic search api
                    var searchRequestId = await _searchApiRequestService.GetLinkedSearchRequestIdAsync(request.SearchApiRequestId, cts.Token);
                    SSG_SearchRequest searchRequest = new SSG_SearchRequest()
                    {
                        SearchRequestId = searchRequestId
                    };

                    if (completeEvent?.MatchedPersons != null)
                    {

                        foreach (PersonFound p in completeEvent.MatchedPersons)
                        {
                            SSG_Identifier sourceIdentifer = await _register.GetMatchedSourceIdentifier(p.SourcePersonalIdentifier, completeEvent?.SearchRequestKey);
                            await _searchResultService.ProcessPersonFound(p, completeEvent.ProviderProfile, searchRequest, request.SearchApiRequestId, cts.Token, sourceIdentifer);
                        }
                    }

                    await _dataPartnerService.UpdateRetries(completeEvent?.ProviderProfile.Name, 0, cts, request);
                    _inProcessing = false;
                }
            }
            catch (Exception e)
            {
                _inProcessing = false;
                _logger.LogError(e, e.Message, null);
            }
        }
    }
}
