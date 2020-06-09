using AutoMapper;
using DynamicsAdapter.Web.PersonSearch.Models;
using DynamicsAdapter.Web.Register;
using Fams3Adapter.Dynamics;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.SearchApiEvent;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Fams3Adapter.Dynamics.SearchRequest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using Serilog.Context;
using System;
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
        private readonly IMapper _mapper;
        private readonly ISearchRequestRegister _register;

        public PersonSearchController(ISearchResultService searchResultService,
            ISearchApiRequestService searchApiRequestService,
            ILogger<PersonSearchController> logger, 
            IMapper mapper,
            ISearchRequestRegister register)
        {
            _searchResultService = searchResultService;
            _searchApiRequestService = searchApiRequestService;
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
        [Route("Completed/{id}")]
        [OpenApiTag("Person Search Events API")]
        public async Task<IActionResult> Completed(Guid id, [FromBody]PersonSearchCompleted personCompletedEvent)
        {
            try
            {
                Guard.NotNull(personCompletedEvent, nameof(personCompletedEvent));
                using (LogContext.PushProperty("FileId", personCompletedEvent?.FileId))
                using (LogContext.PushProperty("DataPartner", personCompletedEvent?.ProviderProfile.Name))
                {
                    _logger.LogInformation("Received Person search completed event with SearchRequestId is " + id);
                    var cts = new CancellationTokenSource();

                    //update completed event
                    var searchApiEvent = _mapper.Map<SSG_SearchApiEvent>(personCompletedEvent);
                    _logger.LogDebug($"Attempting to create a new event for SearchApiRequest [{id}]");
                    var result = await _searchApiRequestService.AddEventAsync(id, searchApiEvent, cts.Token);
                    _logger.LogInformation($"Successfully created completed event for SearchApiRequest [{id}]");

                    //upload search result to dynamic search api
                    var searchRequestId = await _searchApiRequestService.GetLinkedSearchRequestIdAsync(id, cts.Token);
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
                            SSG_Identifier sourceIdentifer = await _register.GetMatchedSourceIdentifier(p.SourcePersonalIdentifier, id);
                            await _searchResultService.ProcessPersonFound(p, personCompletedEvent.ProviderProfile, searchRequest, cts.Token, sourceIdentifer);
                        }
                    }                   

                    return Ok();
                }

            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }


        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("Accepted/{id}")]
        [OpenApiTag("Person Search Events API")]
        public async Task<IActionResult> Accepted(Guid id, [FromBody]PersonSearchAccepted personAcceptedEvent)
        {
            using (LogContext.PushProperty("FileId", personAcceptedEvent?.FileId))
            using (LogContext.PushProperty("DataPartner", personAcceptedEvent?.ProviderProfile.Name))
            {
                _logger.LogInformation($"Received new event for SearchApiRequest [{id}]");

                var token = new CancellationTokenSource();

                try
                {
                    var searchApiEvent = _mapper.Map<SSG_SearchApiEvent>(personAcceptedEvent);
                    _logger.LogDebug($"Attempting to create a new event for SearchApiRequest [{id}]");
                    var result = await _searchApiRequestService.AddEventAsync(id, searchApiEvent, token.Token);
                    _logger.LogInformation($"Successfully created accepted event for SearchApiRequest [{id}]");
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
        [Route("Failed/{id}")]
        [OpenApiTag("Person Search Events API")]
        public async Task<IActionResult> Failed(Guid id, [FromBody]PersonSearchFailed personFailedEvent)
        {
            using (LogContext.PushProperty("FileId", personFailedEvent?.FileId))
            using (LogContext.PushProperty("DataPartner", personFailedEvent?.ProviderProfile.Name))
            {
                _logger.LogInformation($"Received new event for SearchApiRequest [{id}]");

                var token = new CancellationTokenSource();

                try
                {
                    var searchApiEvent = _mapper.Map<SSG_SearchApiEvent>(personFailedEvent);
                    _logger.LogDebug($"Attempting to create a new event for SearchApiRequest [{id}]");
                    var result = await _searchApiRequestService.AddEventAsync(id, searchApiEvent, token.Token);
                    _logger.LogInformation($"Successfully created failed event for SearchApiRequest [{id}]");
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
        [Route("Finalized/{id}")]
        [OpenApiTag("Person Search Events API")]
        public async Task<IActionResult> Finalized(Guid id, [FromBody]PersonSearchFinalized personFinalizedEvent)
        {
            using (LogContext.PushProperty("FileId", personFinalizedEvent?.FileId))
            {
                _logger.LogInformation($"Received new event for SearchApiRequest [{id}]");

                var token = new CancellationTokenSource();

                try
                {
                    var result = await _searchApiRequestService.MarkComplete(id, token.Token);
                    _logger.LogInformation($"Successfully finalized Person Search [{id}]");
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
        [Route("Rejected/{id}")]
        [OpenApiTag("Person Search Events API")]
        public async Task<IActionResult> Rejected(Guid id, [FromBody]PersonSearchRejected personRejectedEvent)
        {
            using (LogContext.PushProperty("FileId", personRejectedEvent?.FileId))
            using (LogContext.PushProperty("DataPartner", personRejectedEvent?.ProviderProfile.Name))
            {
                _logger.LogInformation($"Received new event for SearchApiRequest [{id}]");

                var token = new CancellationTokenSource();

                try
                {
                    var searchApiEvent = _mapper.Map<SSG_SearchApiEvent>(personRejectedEvent);
                    _logger.LogDebug($"Attempting to create a new event for SearchApiRequest [{id}]");
                    var result = await _searchApiRequestService.AddEventAsync(id, searchApiEvent, token.Token);
                    _logger.LogInformation($"Successfully created rejected event for SearchApiRequest [{id}]");


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