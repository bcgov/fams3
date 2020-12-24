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
        Task<Person> GetSearchRequestResponse(SearchResponseReady responseReady);
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

        public async Task<Person> GetSearchRequestResponse(SearchResponseReady searchResponseReady)
        {
            if (Guid.Parse(searchResponseReady.ResponseGuid) == Guid.Empty) return null;
            var cts = new CancellationTokenSource();
            SSG_SearchRequestResponse sr;
            try
            {
                sr = await _searchResponseService.GetSearchResponse(Guid.Parse(searchResponseReady.ResponseGuid), cts.Token);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        
            if (sr == null) return null;
            Person person = _mapper.Map<Person>(sr);

            return person;

        }
    }
}
