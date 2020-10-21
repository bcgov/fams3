using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BcGov.Fams3.Redis;
using BcGov.Fams3.Redis.Model;
using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSwag.Annotations;
using OpenTracing;
using SearchApi.Web.DeepSearch;
using SearchApi.Web.Messaging;
using Serilog.Context;

namespace SearchApi.Web.Controllers
{
    /// <summary>
    /// the PeopleController represents the people API.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class RfiController : ControllerBase
    {
        private readonly ITracer _tracer;
        private readonly IDispatcher _dispatcher;
        private readonly ILogger _logger;
        private readonly ICacheService _cacheService;

        public RfiController(
            ILogger<PeopleController> logger, 
            ITracer tracer,
            IDispatcher dispatcher,
            ICacheService distributedCache)
        {
            _logger = logger;
            _tracer = tracer;
            _dispatcher = dispatcher;
            _cacheService = distributedCache;
           
      
        }

        /// <summary>
        /// Receives a <see cref="RequestForInformation"/> and start the process of sending a request for information for a person
        /// by submitting to appropriate queue.
        /// </summary>
        /// <param name="rfi">A request for information</param>
        /// <returns><see cref="RfiResponse"/></returns>
        [HttpPost]
        [Route("send")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(RfiResponse), StatusCodes.Status202Accepted)]
        [OpenApiTag("RequestForInformation API")]
        public async Task<IActionResult> Send([FromHeader(Name = "X-RequestId")] string id, [FromBody]RequestForInformation rfi)
        {
            using (LogContext.PushProperty("Request For Information Key", rfi?.Id))
            {
                if (id == null || !Guid.TryParse(id, out var rfiId))
                {
                    rfiId = Guid.NewGuid();
                }

                _logger.LogInformation($"Successfully received new request for information [{rfiId}].");

                _tracer.ActiveSpan.SetTag("searchRequestId", $"{rfiId}");

                _logger.LogDebug($"Attempting to publish ${nameof(RequestForInformation)} to destination queue.");

                await _dispatcher.Dispatch(rfi, rfiId);

                _logger.LogInformation($"Successfully published ${nameof(RequestForInformation)} to destination queue.");

                return Accepted(new RfiResponse(rfiId));
            }
        }
	}
}