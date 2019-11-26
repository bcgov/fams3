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

        //POST: MatchFound/id
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("MatchFound/{id}")]
        public async Task<IActionResult> MatchFound(Guid id, [FromBody] MatchFound matchFound)
        {
            _logger.LogInformation("Received MatchFound response with SearchRequestId is " + id);
            var cts = new CancellationTokenSource();

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
                        SearchRequestId = id
                    },
                    StateCode = 0,
                    StatusCode = 1
                };

                try
                {
                    var result = await _searchRequestService.UploadIdentifier(toBeReplaced, cts.Token);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return BadRequest();
                }

            }

            return Ok();
        }


        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("Event/{id}")]
        public async Task<IActionResult> Event(Guid id, [FromBody] Object temp)
        {

            _logger.LogInformation($"Received new event for SearchApiRequest [{id}]");

            var token = new CancellationTokenSource();

            var searchApiEvent = new SSG_SearchApiEvent()
            {
                Id = Guid.NewGuid(),
                Message = "Test Event from dynadapter",
                ProviderName = "Dynadapter",
                TimeStamp = DateTime.Now,
                Type = "Test"
            };

            try
            {
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