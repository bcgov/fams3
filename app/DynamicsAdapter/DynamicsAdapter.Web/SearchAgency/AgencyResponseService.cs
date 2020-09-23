using AutoMapper;
using DynamicsAdapter.Web.SearchAgency.Models;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.SearchResponse;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.SearchAgency
{
    public interface IAgencyResponseService
    {
        Task<SSG_SearchRequestResponse> GetSearchRequestResponse(SearchResponseReady responseReady);
    }

    public class AgencyResponseService : IAgencyResponseService
    {
        private readonly ILogger<AgencyResponseService> _logger;
        private readonly ISearchResponseService _searchResponseService;
        private readonly IMapper _mapper;


        public AgencyResponseService(ISearchResponseService searchResponseService, ILogger<AgencyResponseService> logger, IMapper mapper)
        {
            _searchResponseService = searchResponseService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<SSG_SearchRequestResponse> GetSearchRequestResponse(SearchResponseReady searchResponseReady)
        {
            var cts = new CancellationTokenSource();

            SSG_SearchRequestResponse sr = await _searchResponseService.GetSearchResponse(Guid.Parse(searchResponseReady.ResponseGuid), cts.Token);
            return sr;

        }
    }
}
