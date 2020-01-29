using AutoMapper;
using DynamicsAdapter.Web.PersonSearch.Models;
using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.Name;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.PhoneNumber;
using Fams3Adapter.Dynamics.SearchRequest;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.PersonSearch
{
    public interface IPersonFoundService
    {
        bool ProcessPersonFound(Person person, ProviderProfile providerProfile, SSG_SearchRequest searchRequest, CancellationToken cancellationToken);
    }

    public class PersonFoundService
    {
        private readonly ILogger<PersonFoundService> _logger;
        private readonly ISearchRequestService _searchRequestService;
        private readonly IMapper _mapper;

        public PersonFoundService(ISearchRequestService searchRequestService,ILogger<PersonFoundService> logger, IMapper mapper)
        {
            _searchRequestService = searchRequestService;
            _logger = logger;
            _mapper = mapper;
        }

        public bool ProcessPersonFound(Person person, ProviderProfile providerProfile, SSG_SearchRequest searchRequest, CancellationToken concellationToken)
        {
            return true;
        }

        private async Task<bool> UploadIdentifiers(SSG_SearchRequest request, PersonSearchCompleted personCompletedEvent, CancellationToken concellationToken)
        {
            if (personCompletedEvent.MatchedPerson.Identifiers == null) return true;
            foreach (var matchFoundPersonId in personCompletedEvent.MatchedPerson.Identifiers)
            {
                SSG_Identifier identifier = _mapper.Map<SSG_Identifier>(matchFoundPersonId);
                identifier.SSG_SearchRequest = request;
                identifier.InformationSource = personCompletedEvent.ProviderProfile.DynamicsID();
                var identifer = await _searchRequestService.CreateIdentifier(identifier, concellationToken);
            }
            return true;
        }

        private async Task<bool> SavePerson(SSG_SearchRequest request, PersonSearchCompleted personCompletedEvent, CancellationToken concellationToken)
        {
            if (personCompletedEvent.MatchedPerson == null) return true;
            SSG_Person person = _mapper.Map<SSG_Person>(personCompletedEvent.MatchedPerson);
            person.SearchRequest = request;
            person.InformationSource = personCompletedEvent.ProviderProfile.DynamicsID();
            await _searchRequestService.SavePerson(person, concellationToken);
            return true;
        }

        private async Task<bool> UploadAddresses(SSG_SearchRequest request, PersonSearchCompleted personCompletedEvent, CancellationToken concellationToken)
        {
            if (personCompletedEvent.MatchedPerson.Addresses == null) return true;
            foreach (var address in personCompletedEvent.MatchedPerson.Addresses)
            {
                SSG_Address addr = _mapper.Map<SSG_Address>(address);
                addr.SearchRequest = request;
                addr.InformationSource = personCompletedEvent.ProviderProfile.DynamicsID();
                var uploadedAddr = await _searchRequestService.CreateAddress(addr, concellationToken);
            }
            return true;
        }

        private async Task<bool> UploadPhoneNumbers(SSG_SearchRequest request, PersonSearchCompleted personCompletedEvent, CancellationToken concellationToken)
        {
            if (personCompletedEvent.MatchedPerson.Phones == null) return true;
            foreach (var phone in personCompletedEvent.MatchedPerson.Phones)
            {
                SSG_PhoneNumber ph = _mapper.Map<SSG_PhoneNumber>(phone);
                ph.SearchRequest = request;
                ph.InformationSource = personCompletedEvent.ProviderProfile.DynamicsID();
                await _searchRequestService.CreatePhoneNumber(ph, concellationToken);
            }
            return true;
        }

        private async Task<bool> UploadNames(SSG_SearchRequest request, PersonSearchCompleted personCompletedEvent, CancellationToken concellationToken)
        {
            if (personCompletedEvent.MatchedPerson.Names == null) return true;
            foreach (var name in personCompletedEvent.MatchedPerson.Names)
            {
                SSG_Aliase n = _mapper.Map<SSG_Aliase>(name);
                n.SearchRequest = request;
                n.InformationSource = personCompletedEvent.ProviderProfile.DynamicsID();
                await _searchRequestService.CreateName(n, concellationToken);
            }
            return true;
        }
    }
}
