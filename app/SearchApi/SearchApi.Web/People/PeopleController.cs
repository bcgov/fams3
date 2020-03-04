using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BcGov.Fams3.Redis;
using BcGov.Fams3.Redis.Model;
using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using OpenTracing;

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

        private readonly IBusControl _busControl;

        private readonly ILogger _logger;

        private readonly ICacheService _cacheService;

        public PeopleController(IBusControl busControl, ILogger<PeopleController> logger, ITracer tracer, ICacheService cacheService)
        {
            this._logger = logger;
            this._tracer = tracer;
            this._busControl = busControl;
            this._cacheService = cacheService;
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

            SearchRequest searchRequest = new SearchRequest
            {
                Person = personSearchRequest,
                SearchRequestId = searchRequestId,
                Providers = null
            };
            _logger.LogInformation($"Save Request [{searchRequestId}] to cache. ");
            bool saveResult = await _cacheService.SaveRequest(searchRequest);

            _logger.LogDebug($"Attempting to publish ${nameof(PersonSearchOrdered)} to destination queue.");

            await _busControl.Publish<PersonSearchOrdered>(new PersonSearchOrderEvent(searchRequestId)
            {
                Person = personSearchRequest
            });

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