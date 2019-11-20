using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.SearchRequest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DynamicsAdapter.Web.MatchFound
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
    public class MatchFoundController : ControllerBase
    {
        private readonly ILogger<MatchFoundController> _logger;
        private ISearchRequestService _service;
        public MatchFoundController(ILogger<MatchFoundController> logger, ISearchRequestService service)
        {
            _logger = logger;
            _service = service;
        }

        //POST: MatchFound/id
        [HttpPost("{id}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MatchFound(Guid id, [FromBody]MatchFound matchFound)
        {

            _logger.LogInformation("Received MatchFound response with SearchRequestId is " + id);
            var cts = new CancellationTokenSource();

            foreach (var matchFoundPersonId in matchFound.PersonIds)
            {

                //todo, replaced with data from payload
                var toBeReplaced = new SSG_Identifier()
                {
                    SSG_Identification = matchFoundPersonId.Number,
                    ssg_identificationeffectivedate = new DateTime(2014, 1, 1),
                    IdentifierType = IdentificationType.DriverLicense.Value,
                    IssuedBy = "ICBC",
                    SSG_SearchRequest = new SSG_SearchRequest()
                    {
                        SearchRequestId = id
                    },
                    StateCode = 0,
                    StatusCode = 1
                };
                //todo

                try
                {
                    SSG_Identifier result = await _service.UploadIdentifier(toBeReplaced, cts.Token);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return BadRequest();
                }

            }

            
            return Ok();
        }
    }
}