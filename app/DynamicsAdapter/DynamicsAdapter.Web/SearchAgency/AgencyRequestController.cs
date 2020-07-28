using AutoMapper;
using DynamicsAdapter.Web.PersonSearch;
using DynamicsAdapter.Web.SearchAgency.Models;
using Fams3Adapter.Dynamics.SearchRequest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.SearchAgency
{
    [Route("[controller]")]
    [ApiController]
    public class AgencyRequestController : ControllerBase
    {
        private readonly ILogger<PersonSearchController> _logger;
        private readonly ISearchRequestService _searchRequestService;
        private readonly IMapper _mapper;

        public AgencyRequestController(
                ISearchRequestService searchRequestService,
                ILogger<PersonSearchController> logger,
                IMapper mapper
                )
        {

            _searchRequestService = searchRequestService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("CreateSearchRequest/{key}")]
        [OpenApiTag("Agency Search Reqeust API")]
        public async Task<IActionResult> CreateSearchRequest(string key, [FromBody]SearchRequestOrdered searchRequestOrdered)
        {
            if (string.IsNullOrEmpty(key)) return BadRequest();
            if (searchRequestOrdered.Action != RequestAction.NEW) return BadRequest();

            SearchRequestEntity searchRequestEntity = _mapper.Map<SearchRequestEntity>(searchRequestOrdered);
            searchRequestEntity.CreatedByApi = true;

            var cts = new CancellationTokenSource();
            
            await _searchRequestService.CreateSearchRequest(searchRequestEntity, cts.Token );

            return Ok();
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("UpdateSearchRequest/{key}")]
        [OpenApiTag("Agency Search Reqeust API")]
        public async Task<IActionResult> UpdateSearchRequest(string key, [FromBody]SearchRequestOrdered personCompletedEvent)
        {
            //todo: Not implemented yet.
            await Task.Delay(1);
            return Ok();
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("CancelSearchRequest/{key}")]
        [OpenApiTag("Agency Search Reqeust API")]
        public async Task<IActionResult> CancelSearchRequest(string key, [FromBody]SearchRequestOrdered personCompletedEvent)
        {
            //todo: Not implemented yet.
            await Task.Delay(1);
            return Ok();
        }
    }
}