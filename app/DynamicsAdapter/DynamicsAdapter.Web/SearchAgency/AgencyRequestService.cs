using AutoMapper;
using DynamicsAdapter.Web.SearchAgency.Models;
using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.Employment;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.PhoneNumber;
using Fams3Adapter.Dynamics.RelatedPerson;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Types;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.SearchAgency
{
    public interface IAgencyRequestService
    {
        Task<SSG_SearchRequest> ProcessSearchRequestOrdered(SearchRequestOrdered searchRequestOrdered);
    }

    public class AgencyRequestService : IAgencyRequestService
    {
        private readonly ILogger<AgencyRequestService> _logger;
        private readonly ISearchRequestService _searchRequestService;
        private readonly IMapper _mapper;
        private Person _personSought;
        private SSG_Person _uploadedPerson;
        private SSG_SearchRequest _uploadedSearchRequest;
        private CancellationToken _cancellationToken;

        public AgencyRequestService(ISearchRequestService searchRequestService, ILogger<AgencyRequestService> logger, IMapper mapper)
        {
            _searchRequestService = searchRequestService;
            _logger = logger;
            _mapper = mapper;
            _personSought = null;
            _uploadedPerson = null;
            _uploadedSearchRequest = null;
        }

        public async Task<SSG_SearchRequest> ProcessSearchRequestOrdered(SearchRequestOrdered searchRequestOrdered)
        {
            _personSought = searchRequestOrdered.Person;
            var cts = new CancellationTokenSource();
            _cancellationToken = cts.Token;

            SearchRequestEntity searchRequestEntity = _mapper.Map<SearchRequestEntity>(searchRequestOrdered);
            searchRequestEntity.CreatedByApi = true;
            _uploadedSearchRequest = await _searchRequestService.CreateSearchRequest(searchRequestEntity, cts.Token);

            PersonEntity personEntity = _mapper.Map<PersonEntity>(_personSought);
            personEntity.SearchRequest = _uploadedSearchRequest;
            personEntity.InformationSource = InformationSourceType.Request.Value;
            _uploadedPerson = await _searchRequestService.SavePerson(personEntity, _cancellationToken);

            await UploadIdentifiers();
            await UploadAddresses();
            await UploadPhones();
            await UploadEmployment();
            await UploadRelatedPersons();
            return _uploadedSearchRequest;
        }

        private async Task<bool> UploadIdentifiers()
        {
            if (_personSought.Identifiers == null) return true;
            _logger.LogDebug($"Attempting to create identifier records for SearchRequest.");

            foreach (var personId in _personSought.Identifiers.Where(m => m.Owner == OwnerType.PersonSought))
            {
                IdentifierEntity identifier = _mapper.Map<IdentifierEntity>(personId);
                identifier.SearchRequest = _uploadedSearchRequest;
                identifier.InformationSource = InformationSourceType.Request.Value;
                identifier.Person = _uploadedPerson;
                SSG_Identifier newIdentifier = await _searchRequestService.CreateIdentifier(identifier, _cancellationToken);
            }
            return true;
        }

        private async Task<bool> UploadAddresses()
        {
            if (_personSought.Addresses == null) return true;

            _logger.LogDebug($"Attempting to create adddress for SoughtPerson");

            foreach (var address in _personSought.Addresses.Where(m => m.Owner == OwnerType.PersonSought))
            {
                AddressEntity addr = _mapper.Map<AddressEntity>(address);
                addr.SearchRequest = _uploadedSearchRequest;
                addr.InformationSource = InformationSourceType.Request.Value;
                addr.Person = _uploadedPerson;
                SSG_Address uploadedAddr = await _searchRequestService.CreateAddress(addr, _cancellationToken);
            }
            return true;
        }

        private async Task<bool> UploadPhones()
        {
            if (_personSought.Phones == null) return true;

            _logger.LogDebug($"Attempting to create Phones for SoughtPerson");

            foreach (var phone in _personSought.Phones.Where(m => m.Owner == OwnerType.PersonSought))
            {
                PhoneNumberEntity ph = _mapper.Map<PhoneNumberEntity>(phone);
                ph.SearchRequest = _uploadedSearchRequest;
                ph.InformationSource = InformationSourceType.Request.Value;
                ph.Person = _uploadedPerson;
                SSG_PhoneNumber uploadedPhone = await _searchRequestService.CreatePhoneNumber(ph, _cancellationToken);
            }
            return true;
        }

        private async Task<bool> UploadEmployment()
        {
            if (_personSought.Employments == null) return true;

            _logger.LogDebug($"Attempting to create employment records for PersonSought.");

            foreach (var employment in _personSought.Employments)
            {
                EmploymentEntity e = _mapper.Map<EmploymentEntity>(employment);
                e.SearchRequest = _uploadedSearchRequest;
                e.InformationSource = InformationSourceType.Request.Value;
                e.Person = _uploadedPerson;
                SSG_Employment ssg_employment = await _searchRequestService.CreateEmployment(e, _cancellationToken);

                if (employment.Employer != null)
                {
                    foreach (var phone in employment.Employer.Phones)
                    {
                        EmploymentContactEntity p = _mapper.Map<EmploymentContactEntity>(phone);
                        p.Employment = ssg_employment;
                        await _searchRequestService.CreateEmploymentContact(p, _cancellationToken);
                    }
                }
            }

            return true;
        }

        private async Task<bool> UploadRelatedPersons()
        {
            if (_personSought.RelatedPersons == null) return true;

            _logger.LogDebug($"Attempting to create related person records person sought.");

            foreach (var relatedPerson in _personSought.RelatedPersons)
            {
                RelatedPersonEntity n = _mapper.Map<RelatedPersonEntity>(relatedPerson);
                n.SearchRequest = _uploadedSearchRequest;
                n.InformationSource = InformationSourceType.Request.Value;
                n.Person = _uploadedPerson;
                SSG_Identity relate = await _searchRequestService.CreateRelatedPerson(n, _cancellationToken);

            }
            return true;
        }
    }
}
