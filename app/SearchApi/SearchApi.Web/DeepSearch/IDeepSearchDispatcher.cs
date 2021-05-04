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
      
        Task StartAnotherWave(string searchRequestKey, WaveSearchData wave, Person person, int noOfRetries, int timeBetweenRetries);

    }
   
    public class DeepSearchDispatcher : IDeepSearchDispatcher
    {
        private readonly ILogger<DeepSearchDispatcher> _logger;
        private readonly IDispatcher _dispatcher;
   
        public DeepSearchDispatcher(ILogger<DeepSearchDispatcher> logger,IDispatcher dispatcher)
        {
            _logger = logger;
            _dispatcher = dispatcher;
        }

        public async Task StartAnotherWave(string searchRequestKey, WaveSearchData wave, Person person, int noOfRetries, int timeBetweenRetries)
        {
            _logger.LogInformation($"About to dispatch new wave {wave.CurrentWave} for {wave.DataPartner}");
            await _dispatcher.Dispatch(new PersonSearchRequest(person.FirstName, person.LastName, person.DateOfBirth, person.Identifiers, person.Addresses, person.Phones, person.Names, person.RelatedPersons, person.Employments, new List<DataProvider>
                        {
                            new DataProvider { Completed = false, Name = wave.DataPartner, NumberOfRetries = noOfRetries, TimeBetweenRetries = timeBetweenRetries, SearchSpeedType = wave.SearchSpeed }
                        }, searchRequestKey), Guid.NewGuid());

        }



       
    }
}
