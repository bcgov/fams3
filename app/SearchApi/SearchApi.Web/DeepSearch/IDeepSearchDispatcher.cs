using BcGov.Fams3.Redis;
using BcGov.Fams3.SearchApi.Contracts.Person;
using Microsoft.Extensions.Logging;
using SearchApi.Web.Controllers;
using SearchApi.Web.DeepSearch.Schema;
using SearchApi.Web.Messaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SearchApi.Web.DeepSearch
{
    public interface IDeepSearchDispatcher
    {
      
        Task Dispatch(string searchRequestKey, WaveSearchData wave, Person person);
    }
   
    public class DeepSearchDispatcher : IDeepSearchDispatcher
    {
        private readonly ILogger<DeepSearchDispatcher> _logger;
        private readonly ICacheService _cacheService;
        private readonly IDispatcher _dispatcher;
        public DeepSearchDispatcher(ILogger<DeepSearchDispatcher> logger, ICacheService cacheService, IDispatcher dispatcher)
        {
            _logger = logger;
            _cacheService = cacheService;
            _dispatcher = dispatcher; ;
        }

        public async Task Dispatch(string searchRequestKey, WaveSearchData wave, Person person)
        {
            await _dispatcher.Dispatch(new PersonSearchRequest(person.FirstName, person.LastName, person.DateOfBirth, person.Identifiers, person.Addresses, person.Phones, person.Names, person.RelatedPersons, person.Employments, new List<DataProvider>
                        {
                            new DataProvider { Completed = false, Name = wave.DataPartner, NumberOfRetries = 1, TimeBetweenRetries = 3 }
                        }, searchRequestKey), Guid.NewGuid());
        }

       
    }
}
