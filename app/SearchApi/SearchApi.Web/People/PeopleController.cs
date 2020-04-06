using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using OpenTracing;
using SearchApi.Web.Messaging;

namespace SearchApi.Web.Controllers
{
    /// <summary>
    /// the PeopleController represents the people API.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {

        private readonly ITracer _tracer;

        private readonly IDispatcher _dispatcher;

        private readonly ILogger _logger;

        private readonly IDistributedCache _distributedCache;

        public PeopleController(
            ILogger<PeopleController> logger, 
            ITracer tracer,
            IDispatcher dispatcher)
        {
            this._logger = logger;
            this._tracer = tracer;
            this._dispatcher = dispatcher;
      
        }

        /// <summary>
        /// Receives a <see cref="PersonSearchRequest"/> and start the process of searching additional information for a person
        /// by submitting an ExecuteSearch Command to appropriate queue.
        /// </summary>
        /// <param name="personSearchRequest">A request to search a person</param>
        /// <returns><see cref="PersonSearchResponse"/></returns>
        [HttpPost]
        [Route("search")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PersonSearchResponse), StatusCodes.Status202Accepted)]
        [OpenApiTag("People API")]
        public async Task<IActionResult> Search([FromHeader(Name = "X-RequestId")] string id, [FromBody]PersonSearchRequest personSearchRequest)
        {

            if(id == null || !Guid.TryParse(id, out var searchRequestId))
            {
                searchRequestId = Guid.NewGuid();
            }

            _logger.LogInformation($"Successfully received new search request [{searchRequestId}].");

            _tracer.ActiveSpan.SetTag("searchRequestId", $"{searchRequestId}");

            _logger.LogDebug($"Attempting to publish ${nameof(PersonSearchOrdered)} to destination queue.");

            await _dispatcher.Dispatch(personSearchRequest, searchRequestId);

            _logger.LogInformation($"Successfully published ${nameof(PersonSearchOrdered)} to destination queue.");

            return Accepted(new PersonSearchResponse(searchRequestId));
        }

        public class PersonSearchOrderEvent : PersonSearchOrdered
        {

            public PersonSearchOrderEvent(Guid searchRequestId)
            {
                SearchRequestId = searchRequestId;
                TimeStamp = DateTime.Now;
            }

            public Guid SearchRequestId { get; }
            public DateTime TimeStamp { get; }
            public Person Person { get; set; }
        }
    }
}