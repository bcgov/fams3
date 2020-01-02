using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DynamicsAdapter.Web.PersonSearch.Models;
using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.PhoneNumber;
using Fams3Adapter.Dynamics.SearchApiEvent;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Fams3Adapter.Dynamics.SearchRequest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Fams3Adapter.Dynamics.Types;
using Fams3Adapter.Dynamics.Name;

namespace DynamicsAdapter.Web.PersonSearch
{
    [Route("[controller]")]
    [ApiController]
    public class PersonSearchController : ControllerBase
    {
        private readonly ILogger<PersonSearchController> _logger;
        private readonly ISearchRequestService _searchRequestService;
        private readonly ISearchApiRequestService _searchApiRequestService;
        private readonly IMapper _mapper;

        public PersonSearchController(ISearchRequestService searchRequestService,
            ISearchApiRequestService searchApiRequestService, ILogger<PersonSearchController> logger,  IMapper mapper)
        {
            _searchRequestService = searchRequestService;
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
                await UploadIdentifiers(searchRequest, personCompletedEvent, cts.Token);
                await UploadAddresses(searchRequest, personCompletedEvent, cts.Token);
                await UploadPhoneNumbers(searchRequest, personCompletedEvent, cts.Token);
                await UploadNames(searchRequest, personCompletedEvent, cts.Token);
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
        [Route("Rejected/{id}")]
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

        private async Task<bool> UploadIdentifiers(SSG_SearchRequest request, PersonSearchCompleted personCompletedEvent, CancellationToken concellationToken)
        {
            if (personCompletedEvent.MatchedPerson.Identifiers == null) return true;
            foreach (var matchFoundPersonId in personCompletedEvent.MatchedPerson.Identifiers)
            {
                SSG_Identifier identifier = _mapper.Map<SSG_Identifier>(matchFoundPersonId);
                identifier.SSG_SearchRequest = request;
                identifier.InformationSource = personCompletedEvent.ProviderProfile.DynamicsID();
                var identifer = await _searchRequestService.CreateIdentifier(identifier, concellationToken);
            }
            return true;
        }

        private async Task<bool> UploadAddresses(SSG_SearchRequest request, PersonSearchCompleted personCompletedEvent, CancellationToken concellationToken)
        {
            if (personCompletedEvent.MatchedPerson.Addresses == null) return true;
            foreach (var address in personCompletedEvent.MatchedPerson.Addresses)
            {
                SSG_Address addr = _mapper.Map<SSG_Address>(address);
                addr.SearchRequest = request;
                addr.InformationSource = personCompletedEvent.ProviderProfile.DynamicsID();
                var uploadedAddr = await _searchRequestService.CreateAddress(addr, concellationToken);
            }
            return true;
        }

        private async Task<bool> UploadPhoneNumbers(SSG_SearchRequest request, PersonSearchCompleted personCompletedEvent, CancellationToken concellationToken)
        {
            if (personCompletedEvent.MatchedPerson.PhoneNumbers == null) return true;
            foreach (var phone in personCompletedEvent.MatchedPerson.PhoneNumbers)
            {
                SSG_PhoneNumber ph = _mapper.Map<SSG_PhoneNumber>(phone);
                ph.SearchRequest = request;
                ph.InformationSource = personCompletedEvent.ProviderProfile.DynamicsID();
                await _searchRequestService.CreatePhoneNumber(ph, concellationToken);
            }
            return true;
        }

        private async Task<bool> UploadNames(SSG_SearchRequest request, PersonSearchCompleted personCompletedEvent, CancellationToken concellationToken)
        {
            if (personCompletedEvent.MatchedPerson.Names == null) return true;
            foreach (var name in personCompletedEvent.MatchedPerson.Names)
            {
                //SSG_Alias n = _mapper.Map<SSG_Alias>(name);
                SSG_Alias n = new SSG_Alias()
                {
                    FullName = "hahaha from Peggy",
                    FirstName = "firstname",
                    StateCode = 0,
                    StatusCode = 1
                };
                n.SearchRequest = request;
                //n.InformationSource = personCompletedEvent.ProviderProfile.DynamicsID();
                await _searchRequestService.CreateName(n, concellationToken);
            }
            return true;
        }
    }
}