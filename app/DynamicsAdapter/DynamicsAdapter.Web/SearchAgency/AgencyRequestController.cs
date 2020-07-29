using AutoMapper;
using DynamicsAdapter.Web.PersonSearch;
using DynamicsAdapter.Web.SearchAgency.Models;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.SearchAgency
{
    [Route("[controller]")]
    [ApiController]
    public class AgencyRequestController : ControllerBase
    {
        private readonly ILogger<AgencyRequestController> _logger;
        private readonly IAgencyRequestService _agencyRequestService;

        public AgencyRequestController(               
                ILogger<AgencyRequestController> logger,
                IAgencyRequestService agencyRequestService
                )
        {
            _logger = logger;
            _agencyRequestService = agencyRequestService;
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
            try
            {
               await _agencyRequestService.ProcessSearchRequestOrdered(searchRequestOrdered);
               return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
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