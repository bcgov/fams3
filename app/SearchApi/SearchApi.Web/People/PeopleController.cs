using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag;
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

        private ITracer _tracer;

        private IBus _bus;

        public PeopleController(IBus bus, ITracer tracer)
        {
            this._bus = bus;
            this._tracer = tracer;
        }

        /// <summary>
        /// Receives a <see cref="PersonSearchRequest"/> and start the process of searching additional information for a person.
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

            _tracer.ActiveSpan.SetTag("searchRequestId", $"{searchRequestId}");

            await this._bus.Send<ExecuteSearch>(new
            {
                Id = searchRequestId,
                personSearchRequest.FirstName,
                personSearchRequest.LastName,
                personSearchRequest.DateOfBirth
            });

            return Accepted(new PersonSearchResponse(searchRequestId));
        }
    }
}