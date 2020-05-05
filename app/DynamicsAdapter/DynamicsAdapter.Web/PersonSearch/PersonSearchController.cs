using AutoMapper;
using DynamicsAdapter.Web.PersonSearch.Models;
using Fams3Adapter.Dynamics.SearchApiEvent;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Fams3Adapter.Dynamics.SearchRequest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
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

        public PersonSearchController(ISearchResultService searchResultService,
            ISearchApiRequestService searchApiRequestService, ILogger<PersonSearchController> logger,  IMapper mapper)
        {
            _searchResultService = searchResultService;
            _searchApiRequestService = searchApiRequestService;
            _logger = logger;
            _mapper = mapper;
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
            _logger.LogInformation("Received Person search completed event with SearchRequestId is " + id);
            var cts = new CancellationTokenSource();

            try
            {
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

                if (personCompletedEvent.MatchedPersons != null)
                {
                    //try following code, but automapper throws exception.Cannot access a disposed object.Object name: 'IServiceProvider'.
                    //Parallel.ForEach<Person>(personCompletedEvent.MatchedPersons, async p =>
                    //{
                    //    await _searchResultService.ProcessPersonFound(p, personCompletedEvent.ProviderProfile, searchRequest, cts.Token);
                    //});
                    foreach (Person p in personCompletedEvent.MatchedPersons)
                    {
                        await _searchResultService.ProcessPersonFound(p, personCompletedEvent.ProviderProfile, searchRequest, cts.Token);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }

            return Ok();
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

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("Failed/{id}")]
        [OpenApiTag("Person Search Events API")]
        public async Task<IActionResult> Failed(Guid id, [FromBody]PersonSearchFailed personFailedEvent)
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


        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("Finalized/{id}")]
        [OpenApiTag("Person Search Events API")]
        public async Task<IActionResult> Finalized(Guid id)
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


        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("Rejected/{id}")]
        [OpenApiTag("Person Search Events API")]
        public async Task<IActionResult> Rejected(Guid id, [FromBody]PersonSearchRejected personRejectedEvent)
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