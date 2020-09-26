using BcGov.Fams3.Redis;
using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using Microsoft.Extensions.Logging;
using SearchApi.Web.Controllers;
using SearchApi.Web.DeepSearch.Schema;
using SearchApi.Web.Messaging;
using SearchApi.Web.Notifications;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SearchApi.Web.DeepSearch
{
    public interface IDeepSearchDispatcher
    {
      
        Task StartAnotherWave(string searchRequestKey, WaveSearchData wave, Person person);

        Task FiinalizeDeepSearch(string searchRequestKey, PersonSearchFinalized finalizedSearch);
    }
   
    public class DeepSearchDispatcher : IDeepSearchDispatcher
    {
        private readonly ILogger<DeepSearchDispatcher> _logger;
        private readonly IDispatcher _dispatcher;
        private readonly ISearchApiNotifier<PersonSearchAdapterEvent> _searchApiNotifier;
        public DeepSearchDispatcher(ILogger<DeepSearchDispatcher> logger,IDispatcher dispatcher)
        {
            _logger = logger;
            _dispatcher = dispatcher;
          //  _searchApiNotifier = searchApiNotifier;
        }

        public async Task FiinalizeDeepSearch(string searchRequestKey, PersonSearchFinalized finalizedSearch)
        {
            await _searchApiNotifier.NotifyEventAsync(searchRequestKey, finalizedSearch, EventName.Finalized, new CancellationToken());
        }

        public async Task StartAnotherWave(string searchRequestKey, WaveSearchData wave, Person person)
        {
            await _dispatcher.Dispatch(new PersonSearchRequest(person.FirstName, person.LastName, person.DateOfBirth, person.Identifiers, person.Addresses, person.Phones, person.Names, person.RelatedPersons, person.Employments, new List<DataProvider>
                        {
                            new DataProvider { Completed = false, Name = wave.DataPartner, NumberOfRetries = 1, TimeBetweenRetries = 3 }
                        }, searchRequestKey), Guid.NewGuid());
        }

       
    }
}
