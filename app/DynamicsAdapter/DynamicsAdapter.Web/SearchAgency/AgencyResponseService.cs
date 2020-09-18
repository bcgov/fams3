using AutoMapper;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace DynamicsAdapter.Web.SearchAgency
{
    public interface IAgencyResponseService
    {
    }

    public class AgencyResponseService : IAgencyResponseService
    {
        private readonly ILogger<AgencyResponseService> _logger;
        private readonly ISearchRequestService _searchRequestService;
        private readonly IMapper _mapper;
        private Person _personSought;
        private SSG_Person _uploadedPerson;
        private SSG_SearchRequest _uploadedSearchRequest;
        private CancellationToken _cancellationToken;
        private static int SEARCH_REQUEST_CANCELLED = 867670009;
        private static int SEARCH_REQUEST_CLOSED = 2;

        public AgencyResponseService(ISearchRequestService searchRequestService, ILogger<AgencyResponseService> logger, IMapper mapper)
        {
            _searchRequestService = searchRequestService;
            _logger = logger;
            _mapper = mapper;
            _personSought = null;
            _uploadedPerson = null;
            _uploadedSearchRequest = null;
        }

    }



}
