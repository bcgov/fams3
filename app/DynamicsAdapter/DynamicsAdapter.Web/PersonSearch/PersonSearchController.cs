using AutoMapper;
using DynamicsAdapter.Web.PersonSearch.Models;
using DynamicsAdapter.Web.Register;
using Fams3Adapter.Dynamics;
using Fams3Adapter.Dynamics.DataProvider;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.SearchApiEvent;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSwag.Annotations;
using Serilog.Context;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.PersonSearch
{
    [Route("[controller]")]
    [ApiController]
    public class PersonSearchController : ControllerBase
    {
        private readonly ILogger<PersonSearchController> _logger;
        private readonly ISearchResultService _searchResultService;
        private readonly ISearchApiRequestService _searchApiRequestService;
        private readonly IDataPartnerService _dataPartnerService;
        private readonly IMapper _mapper;
        private readonly ISearchRequestRegister _register;

        public PersonSearchController(ISearchResultService searchResultService,
            ISearchApiRequestService searchApiRequestService,
            IDataPartnerService dataPartnerService,
            ILogger<PersonSearchController> logger, 
            IMapper mapper,
            ISearchRequestRegister register)
        {
            _searchResultService = searchResultService;
            _searchApiRequestService = searchApiRequestService;
            _dataPartnerService = dataPartnerService;
            _logger = logger;
            _mapper = mapper;
            _register = register;
        }

        //POST: Completed/id
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("Completed/{key}")]
        [OpenApiTag("Person Search Events API")]
        public async Task<IActionResult> Completed(string key, [FromBody]PersonSearchCompleted personCompletedEvent)
        {
            try
            {
                Guard.NotNull(personCompletedEvent, nameof(personCompletedEvent));
                using (LogContext.PushProperty("SearchRequestKey", personCompletedEvent?.SearchRequestKey))
                using (LogContext.PushProperty("DataPartner", personCompletedEvent?.ProviderProfile.Name))
                {
                    _logger.LogInformation("Received Person search completed event");
                    var cts = new CancellationTokenSource();
                    SSG_SearchApiRequest request = await _register.GetSearchApiRequest(key);
                    //update completed event
                    var searchApiEvent = _mapper.Map<SSG_SearchApiEvent>(personCompletedEvent);
                    _logger.LogDebug($"Attempting to create a new event for SearchApiRequest");
                    await _searchApiRequestService.AddEventAsync(request.SearchApiRequestId, searchApiEvent, cts.Token);
                    _logger.LogInformation($"Successfully created completed event for SearchApiRequest");

                    //upload search result to dynamic search api
                    var searchRequestId = await _searchApiRequestService.GetLinkedSearchRequestIdAsync(request.SearchApiRequestId, cts.Token);
                    SSG_SearchRequest searchRequest = new SSG_SearchRequest()
                    {
                        SearchRequestId = searchRequestId
                    };

                    if (personCompletedEvent?.MatchedPersons != null)
                    {
                        //try following code, but automapper throws exception.Cannot access a disposed object.Object name: 'IServiceProvider'.
                        //Parallel.ForEach<Person>(personCompletedEvent.MatchedPersons, async p =>
                        //{
                        //    await _searchResultService.ProcessPersonFound(p, personCompletedEvent.ProviderProfile, searchRequest, cts.Token);
                        //});
                        foreach (PersonFound p in personCompletedEvent.MatchedPersons)
                        {
                            SSG_Identifier sourceIdentifer = await _register.GetMatchedSourceIdentifier(p.SourcePersonalIdentifier, key);
                            await _searchResultService.ProcessPersonFound(p, personCompletedEvent.ProviderProfile, searchRequest, request.SearchApiRequestId, cts.Token, sourceIdentifer);
                        }
                    }

                    await UpdateRetries(personCompletedEvent?.ProviderProfile.Name,0, cts, request);
                    return Ok();
                }

            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }

        private async Task UpdateRetries(string providerProfileName, int noOfTry, CancellationTokenSource cts, SSG_SearchApiRequest request)
        {
            var dataSearchApiDataProvider = await _dataPartnerService.GetSearchApiRequestDataProvider(request.SearchApiRequestId, providerProfileName, cts.Token);
           
            if (dataSearchApiDataProvider != null)
            {
                //_logger.LogInformation($"Updating the no of tries for search api request {request.SearchApiRequestId}");
                dataSearchApiDataProvider.NumberOfFailures = (noOfTry == 1) ? dataSearchApiDataProvider.NumberOfFailures + noOfTry : noOfTry;
                if (dataSearchApiDataProvider.NumberOfDaysToRetry == dataSearchApiDataProvider.NumberOfFailures)
                    dataSearchApiDataProvider.AllRetriesDone = NullableBooleanType.Yes.Value;
                else
                    dataSearchApiDataProvider.AllRetriesDone = NullableBooleanType.No.Value;


                await _dataPartnerService.UpdateSearchRequestApiProvider(dataSearchApiDataProvider, cts.Token);
               // _logger.LogInformation($"Updated the no of tries for search api request {request.SearchApiRequestId}");
            }
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("Accepted/{key}")]
        [OpenApiTag("Person Search Events API")]
        public async Task<IActionResult> Accepted(string key, [FromBody]PersonSearchAccepted personAcceptedEvent)
        {
            using (LogContext.PushProperty("SearchRequestKey", personAcceptedEvent?.SearchRequestKey))
            using (LogContext.PushProperty("DataPartner", personAcceptedEvent?.ProviderProfile.Name))
            {
                _logger.LogInformation($"Received new event for SearchApiRequest");

                var token = new CancellationTokenSource();

                try
                {
                    SSG_SearchApiRequest request = await _register.GetSearchApiRequest(key);
                    _logger.LogInformation(JsonConvert.SerializeObject(personAcceptedEvent));
                    _logger.LogInformation(JsonConvert.SerializeObject(request));
                    var searchApiEvent = _mapper.Map<SSG_SearchApiEvent>(personAcceptedEvent);
                    _logger.LogInformation(JsonConvert.SerializeObject(searchApiEvent));
                    _logger.LogDebug($"Attempting to create a new event for SearchApiRequest");
                    await _searchApiRequestService.AddEventAsync(request.SearchApiRequestId, searchApiEvent, token.Token);
                    _logger.LogInformation($"Successfully created accepted event for SearchApiRequest");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return BadRequest();
                }

                return Ok();
            }
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("Failed/{key}")]
        [OpenApiTag("Person Search Events API")]
        public async Task<IActionResult> Failed(string key, [FromBody]PersonSearchFailed personFailedEvent)
        {

          
            using (LogContext.PushProperty("SearchRequestKey", personFailedEvent?.SearchRequestKey))
            using (LogContext.PushProperty("DataPartner", personFailedEvent?.ProviderProfile.Name))
            {
                _logger.LogInformation($"Received new event for SearchApiRequest.");

                var token = new CancellationTokenSource();

                try
                {
                    SSG_SearchApiRequest request = await _register.GetSearchApiRequest(key);
                    var searchApiEvent = _mapper.Map<SSG_SearchApiEvent>(personFailedEvent);
                    _logger.LogDebug($"Attempting to create a new event for SearchApiRequest.");
                    await _searchApiRequestService.AddEventAsync(request.SearchApiRequestId, searchApiEvent, token.Token);
                    await UpdateRetries(personFailedEvent?.ProviderProfile.Name, 1, token, request);
                    _logger.LogInformation($"Successfully created failed event for SearchApiRequest");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return BadRequest();
                }
               
                return Ok();
            }
        }


        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("Finalized/{key}")]
        [OpenApiTag("Person Search Events API")]
        public async Task<IActionResult> Finalized(string key, [FromBody]PersonSearchFinalized personFinalizedEvent)
        {
            using (LogContext.PushProperty("SearchRequestKey", personFinalizedEvent?.SearchRequestKey))
            {
                _logger.LogInformation($"Received new event for SearchApiRequest.");

                var token = new CancellationTokenSource();

                try
                {
                    SSG_SearchApiRequest request = await _register.GetSearchApiRequest(key);
                    await _searchApiRequestService.MarkComplete(request.SearchApiRequestId, token.Token);
                    _logger.LogInformation($"Successfully finalized Person Search.");
                   // await _register.RemoveSearchApiRequest(key);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return BadRequest();
                }

                return Ok();
            }
        }


        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("Rejected/{key}")]
        [OpenApiTag("Person Search Events API")]
        public async Task<IActionResult> Rejected(string key, [FromBody]PersonSearchRejected personRejectedEvent)
        {
            using (LogContext.PushProperty("SearchRequestKey", personRejectedEvent?.SearchRequestKey))
            using (LogContext.PushProperty("DataPartner", personRejectedEvent?.ProviderProfile.Name))
            {
                _logger.LogInformation($"Received new event for SearchApiRequest [{key}]");

                var token = new CancellationTokenSource();

                try
                {
                    var searchApiEvent = _mapper.Map<SSG_SearchApiEvent>(personRejectedEvent);
                    _logger.LogDebug($"Attempting to create a new event for SearchApiRequest [{key}]");
                    SSG_SearchApiRequest request = await _register.GetSearchApiRequest(key);
                    await _searchApiRequestService.AddEventAsync(request.SearchApiRequestId, searchApiEvent, token.Token);
                    _logger.LogInformation($"Successfully created rejected event for SearchApiRequest [{key}]");


                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return BadRequest();
                }

                return Ok();
            }
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("Submitted/{key}")]
        [OpenApiTag("Person Search Events API")]
        public async Task<IActionResult> Submitted(string key, [FromBody]PersonSearchSubmitted personSearchSubmitted)
        {
            using (LogContext.PushProperty("SearchRequestKey", personSearchSubmitted?.SearchRequestKey))
            using (LogContext.PushProperty("DataPartner", personSearchSubmitted?.ProviderProfile.Name))
            {
                _logger.LogInformation($"Received new event for SearchApiRequest");

                var token = new CancellationTokenSource();

                try
                {
                    var searchApiEvent = _mapper.Map<SSG_SearchApiEvent>(personSearchSubmitted);
                    _logger.LogDebug($"Attempting to create a new event for SearchApiRequest [{key}]");
                    SSG_SearchApiRequest request = await _register.GetSearchApiRequest(key);
                    await _searchApiRequestService.AddEventAsync(request.SearchApiRequestId, searchApiEvent, token.Token);
                    _logger.LogInformation($"Successfully created submitted event for SearchApiRequest [{key}]");

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return BadRequest();
                }

                return Ok();
            }
        }
    }
}