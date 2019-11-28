using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.SearchApiEvent;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Fams3Adapter.Dynamics.SearchRequest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DynamicsAdapter.Web.PersonSearch
{

    // TODO: all classes bellow will be coming from SEARCH API CORE LIB
    public class MatchFound
    {
        public Guid SearchRequestId { get; set; }

        public Person Person { get; set; }

        public IEnumerable<PersonId> PersonIds { get; set; }

    }

    public enum PersonIDKind
    {
        DriverLicense
    }

    public class PersonId
    {
        public PersonIDKind Kind { get; set; }
        public string Issuer { get; set; }
        public string Number { get; set; }
    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }


    public class ProviderSearchEventStatus
    {
        public Guid SearchRequestId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string ProviderName { get; set; }
        public string Message { get; set; }
        public string EventType { get; set; }
    }


    [Route("[controller]")]
    [ApiController]
    public class PersonSearchController : ControllerBase
    {
        private readonly ILogger<PersonSearchController> _logger;
        private readonly ISearchRequestService _searchRequestService;
        private readonly ISearchApiRequestService _searchApiRequestService;

        public PersonSearchController(ISearchRequestService searchRequestService,
            ISearchApiRequestService searchApiRequestService, ILogger<PersonSearchController> logger)
        {
            _searchRequestService = searchRequestService;
            _searchApiRequestService = searchApiRequestService;
            _logger = logger;
        }

        //POST: Completed/id
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("Completed/{id}")]
        public async Task<IActionResult> Completed(Guid id, [FromBody]PersonCompletedEvent personCompletedEvent)
        {
            _logger.LogInformation("Received Persone search completed event with SearchRequestId is " + id);
            var cts = new CancellationTokenSource();

            try
            {
                //update event to dynamic search api
                var searchApiEvent = new SSG_SearchApiEvent()
                {
                    Id = Guid.NewGuid(),
                    Name = "PersonSearchCompleted",
                    Message = "PersonSearch Completed",
                    ProviderName = personCompletedEvent.ProviderProfile?.Name,
                    TimeStamp = personCompletedEvent.TimeStamp,
                    Type = "Completed"
                };
                _logger.LogDebug($"Attempting to create a new event for SearchApiRequest [{id}]");
                var result = await _searchApiRequestService.AddEventAsync(id, searchApiEvent, cts.Token);
                 _logger.LogInformation($"Successfully created new event for SearchApiRequest [{id}]");

                //upload search result to dynamic search api
                MatchFound matchFound = personCompletedEvent.MatchFound;
                var searchApiRequestId = await _searchApiRequestService.GetLinkedSearchRequestIdAsync(id, cts.Token);

                foreach (var matchFoundPersonId in matchFound.PersonIds)
                {
                    //TODO: replaced with data from payload
                    var toBeReplaced = new SSG_Identifier()
                    {
                        Identification = matchFoundPersonId.Number,
                        IdentificationEffectiveDate = new DateTime(2014, 1, 1),
                        IdentifierType = IdentificationType.DriverLicense.Value,
                        IssuedBy = "ICBC",
                        SSG_SearchRequest = new SSG_SearchRequest()
                        {
                            SearchRequestId = searchApiRequestId
                        },
                        StateCode = 0,
                        StatusCode = 1
                    };

                    var identifer = await _searchRequestService.UploadIdentifier(toBeReplaced, cts.Token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }

            return Ok();
        }


        public class PersonAcceptedEvent
        {
            public Guid SearchRequestId { get; set; }
            public DateTime TimeStamp { get; set; }
            public ProviderProfile ProviderProfile { get; set; }
        }

        public class PersonCompletedEvent
        {
            public Guid SearchRequestId { get; set; }
            public DateTime TimeStamp { get; set; }
            public ProviderProfile ProviderProfile { get; set; }
            public MatchFound MatchFound { get; set; }
        }

        public class ProviderProfile
        {
            public string Name { get; set; }
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("Accepted/{id}")]
        public async Task<IActionResult> Accepted(Guid id, [FromBody]PersonAcceptedEvent personAcceptedEvent)
        {

            _logger.LogInformation($"Received new event for SearchApiRequest [{id}]");

            var token = new CancellationTokenSource();

            try
            {
                var searchApiEvent = new SSG_SearchApiEvent()
                {
                    Id = Guid.NewGuid(),
                    Name = "PersonSearchAccepted",
                    Message = "PersonSearch Accepted",
                    ProviderName = personAcceptedEvent.ProviderProfile?.Name,
                    TimeStamp = personAcceptedEvent.TimeStamp,
                    Type = "Accepted"
                };
                _logger.LogDebug($"Attempting to create a new event for SearchApiRequest [{id}]");
                var result = await _searchApiRequestService.AddEventAsync(id, searchApiEvent, token.Token);
                _logger.LogInformation($"Successfully created new event for SearchApiRequest [{id}]");
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