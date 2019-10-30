using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag;
using NSwag.Annotations;
using OpenTracing;
using SearchApi.Core.Contracts;

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

        private readonly ISendEndpointProvider _sendEndpointProvider;

        private readonly ILogger _logger;

        public PeopleController(ISendEndpointProvider sendEndpointProvider, ILogger<PeopleController> logger, ITracer tracer)
        {
            this._sendEndpointProvider = sendEndpointProvider;
            this._logger = logger;
            this._tracer = tracer;
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
        public async Task<IActionResult> Search([FromBody]PersonSearchRequest personSearchRequest)
        { 
            Guid searchRequestId = Guid.NewGuid();

            _logger.LogInformation($"Successfully received new search request [{searchRequestId}].");

            _tracer.ActiveSpan.SetTag("searchRequestId", $"{searchRequestId}");

            _logger.LogDebug($"Attempting to send {nameof(ExecuteSearch)} to destination queue.");
            await this._sendEndpointProvider.Send<ExecuteSearch>(new
            {
                Id = searchRequestId,
                personSearchRequest.FirstName,
                personSearchRequest.LastName,
                personSearchRequest.DateOfBirth
            });
            _logger.LogInformation($"Successfully sent {nameof(ExecuteSearch)} to destination queue.");

            return Accepted(new PersonSearchResponse(searchRequestId));
        }
    }
}