using AutoMapper;
using DynamicsAdapter.Web.PersonSearch.Models;
using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.AssetOwner;
using Fams3Adapter.Dynamics.BankInfo;
using Fams3Adapter.Dynamics.Employment;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.InsuranceClaim;
using Fams3Adapter.Dynamics.Name;
using Fams3Adapter.Dynamics.OtherAsset;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.PhoneNumber;
using Fams3Adapter.Dynamics.RelatedPerson;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Vehicle;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.PersonSearch
{
    public interface ISearchResultService
    {
        Task<bool> ProcessPersonFound(Person person, ProviderProfile providerProfile, SSG_SearchRequest searchRequest, CancellationToken cancellationToken);
    }

    public class SearchResultService : ISearchResultService
    {
        private readonly ILogger<SearchResultService> _logger;
        private readonly ISearchRequestService _searchRequestService;
        private readonly IMapper _mapper;

        public SearchResultService(ISearchRequestService searchRequestService, ILogger<SearchResultService> logger, IMapper mapper)
        {
            _searchRequestService = searchRequestService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<bool> ProcessPersonFound(Person person, ProviderProfile providerProfile, SSG_SearchRequest request, CancellationToken concellationToken)
        {
            if (person == null) return true;

            int? providerDynamicsID = providerProfile.DynamicsID();
            PersonEntity ssg_person = _mapper.Map<PersonEntity>(person);
            ssg_person.SearchRequest = request;
            ssg_person.InformationSource = providerDynamicsID;
            _logger.LogDebug($"Attempting to create the found person record for SearchRequest[{request.SearchRequestId}]");
            SSG_Person returnedPerson = await _searchRequestService.SavePerson(ssg_person, concellationToken);

            _logger.LogDebug($"Attempting to create found identifier records for SearchRequest[{request.SearchRequestId}]");
            await UploadIdentifiers(person, request, returnedPerson, providerDynamicsID, concellationToken);

            _logger.LogDebug($"Attempting to create found adddress records for SearchRequest[{request.SearchRequestId}]");
            await UploadAddresses(person, request, returnedPerson, providerDynamicsID, concellationToken);

            _logger.LogDebug($"Attempting to create found phone records for SearchRequest[{request.SearchRequestId}]");
            await UploadPhoneNumbers(person, request, returnedPerson, providerDynamicsID, concellationToken);

            _logger.LogDebug($"Attempting to create found name records for SearchRequest[{request.SearchRequestId}]");
            await UploadNames(person, request, returnedPerson, providerDynamicsID, concellationToken);

            _logger.LogDebug($"Attempting to create found employment records for SearchRequest[{request.SearchRequestId}]");
            await UploadEmployment(person, request, returnedPerson, providerDynamicsID, concellationToken);

            _logger.LogDebug($"Attempting to create found related person records for SearchRequest[{request.SearchRequestId}]");
            await UploadRelatedPersons(person, request, returnedPerson, providerDynamicsID, concellationToken);

            _logger.LogDebug($"Attempting to create bank info records for SearchRequest[{request.SearchRequestId}]");
            await UploadBankInfos(person, request, returnedPerson, providerDynamicsID, concellationToken);

            _logger.LogDebug($"Attempting to create vehicles records for SearchRequest[{request.SearchRequestId}]");
            await UploadVehicles(person, request, returnedPerson, providerDynamicsID, concellationToken);

            _logger.LogDebug($"Attempting to create other assets records for SearchRequest[{request.SearchRequestId}]");
            await UploadOtherAssets(person, request, returnedPerson, providerDynamicsID, concellationToken);

            _logger.LogDebug($"Attempting to create compnsation claims records for SearchRequest[{request.SearchRequestId}]");
            await UploadCompensationClaims(person, request, returnedPerson, providerDynamicsID, concellationToken);

            _logger.LogDebug($"Attempting to create insurance claims records for SearchRequest[{request.SearchRequestId}]");
            await UploadInsuranceClaims(person, request, returnedPerson, providerDynamicsID, concellationToken);

            return true;
        }

        private async Task<bool> UploadIdentifiers(Person person, SSG_SearchRequest request, SSG_Person ssg_person, int? providerDynamicsID, CancellationToken concellationToken)
        {
            if (person.Identifiers == null) return true;
            try
            {
                foreach (var matchFoundPersonId in person.Identifiers)
                {
                    IdentifierEntity identifier = _mapper.Map<IdentifierEntity>(matchFoundPersonId);
                    identifier.SearchRequest = request;
                    identifier.InformationSource = providerDynamicsID;
                    identifier.Person = ssg_person;
                    var identifer = await _searchRequestService.CreateIdentifier(identifier, concellationToken);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        private async Task<bool> UploadAddresses(Person person, SSG_SearchRequest request, SSG_Person ssg_person, int? providerDynamicsID, CancellationToken concellationToken)
        {
            if (person.Addresses == null) return true;
            try
            {
                foreach (var address in person.Addresses)
                {
                    AddressEntity addr = _mapper.Map<AddressEntity>(address);
                    addr.SearchRequest = request;
                    addr.InformationSource = providerDynamicsID;
                    addr.Person = ssg_person;
                    var uploadedAddr = await _searchRequestService.CreateAddress(addr, concellationToken);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        private async Task<bool> UploadPhoneNumbers(Person person, SSG_SearchRequest request, SSG_Person ssg_person, int? providerDynamicsID, CancellationToken concellationToken)
        {
            if (person.Phones == null) return true;
            try
            {
                foreach (var phone in person.Phones)
                {
                    SSG_PhoneNumber ph = _mapper.Map<SSG_PhoneNumber>(phone);
                    ph.SearchRequest = request;
                    ph.InformationSource = providerDynamicsID;
                    ph.Person = ssg_person;
                    await _searchRequestService.CreatePhoneNumber(ph, concellationToken);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }
        private async Task<bool> UploadNames(Person person, SSG_SearchRequest request, SSG_Person ssg_person, int? providerDynamicsID, CancellationToken concellationToken)
        {
            if (person.Names == null) return true;
            try
            {
                foreach (var name in person.Names)
                {
                    AliasEntity n = _mapper.Map<AliasEntity>(name);
                    n.SearchRequest = request;
                    n.InformationSource = providerDynamicsID;
                    n.Person = ssg_person;
                    await _searchRequestService.CreateName(n, concellationToken);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }
        private async Task<bool> UploadEmployment(Person person, SSG_SearchRequest request, SSG_Person ssg_person, int? providerDynamicsID, CancellationToken concellationToken)
        {
            if (person.Employments == null) return true;
            try
            {
                foreach (var employment in person.Employments)
                {
                    EmploymentEntity e = _mapper.Map<EmploymentEntity>(employment);
                    e.SearchRequest = request;
                    e.InformationSource = providerDynamicsID;
                    e.Person = ssg_person;
                    SSG_Employment ssg_employment = await _searchRequestService.CreateEmployment(e, concellationToken);

                    if (employment.Employer != null)
                    {
                        foreach (var phone in employment.Employer.Phones)
                        {
                            SSG_EmploymentContact p = _mapper.Map<SSG_EmploymentContact>(phone);
                            p.Employment = ssg_employment;
                            await _searchRequestService.CreateEmploymentContact(p, concellationToken);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        private async Task<bool> UploadRelatedPersons(Person person, SSG_SearchRequest request, SSG_Person ssg_person, int? providerDynamicsID, CancellationToken concellationToken)
        {
            if (person.RelatedPersons == null) return true;
            try
            {
                foreach (var relatedPerson in person.RelatedPersons)
                {
                    SSG_Identity n = _mapper.Map<SSG_Identity>(relatedPerson);
                    n.SearchRequest = request;
                    n.InformationSource = providerDynamicsID;
                    n.Person = ssg_person;
                    await _searchRequestService.CreateRelatedPerson(n, concellationToken);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        private async Task<bool> UploadBankInfos(Person person, SSG_SearchRequest request, SSG_Person ssg_person, int? providerDynamicsID, CancellationToken concellationToken)
        {
            if (person.BankInfos == null) return true;
            try
            {
                foreach (var bankInfo in person.BankInfos)
                {
                    BankingInformationEntity bank = _mapper.Map<BankingInformationEntity>(bankInfo);
                    bank.SearchRequest = request;
                    bank.InformationSource = providerDynamicsID;
                    bank.Person = ssg_person;
                    await _searchRequestService.CreateBankInfo(bank, concellationToken);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        private async Task<bool> UploadVehicles(Person person, SSG_SearchRequest request, SSG_Person ssg_person, int? providerDynamicsID, CancellationToken concellationToken)
        {
            if (person.Vehicles == null) return true;
            try
            {
                foreach (var v in person.Vehicles)
                {
                    VehicleEntity vehicle = _mapper.Map<VehicleEntity>(v);
                    vehicle.SearchRequest = request;
                    vehicle.InformationSource = providerDynamicsID;
                    vehicle.Person = ssg_person;
                    SSG_Asset_Vehicle ssgVehicle = await _searchRequestService.CreateVehicle(vehicle, concellationToken);
                    if (v.Owners != null)
                    {
                        foreach (var owner in v.Owners)
                        {
                            SSG_AssetOwner assetOwner = _mapper.Map<SSG_AssetOwner>(owner);
                            assetOwner.Vehicle = ssgVehicle;
                            await _searchRequestService.CreateAssetOwner(assetOwner, concellationToken);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        private async Task<bool> UploadOtherAssets(Person person, SSG_SearchRequest request, SSG_Person ssg_person, int? providerDynamicsID, CancellationToken concellationToken)
        {
            if (person.OtherAssets == null) return true;
            try
            {
                foreach (OtherAsset asset in person.OtherAssets)
                {
                    AssetOtherEntity other = _mapper.Map<AssetOtherEntity>(asset);
                    other.SearchRequest = request;
                    other.InformationSource = providerDynamicsID;
                    other.Person = ssg_person;
                    SSG_Asset_Other ssgOtherAsset = await _searchRequestService.CreateOtherAsset(other, concellationToken);
                    if (asset.Owners != null)
                    {
                        foreach (var owner in asset.Owners)
                        {
                            SSG_AssetOwner assetOwner = _mapper.Map<SSG_AssetOwner>(owner);
                            assetOwner.OtherAsset = ssgOtherAsset;
                            await _searchRequestService.CreateAssetOwner(assetOwner, concellationToken);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        private async Task<bool> UploadCompensationClaims(Person person, SSG_SearchRequest request, SSG_Person ssg_person, int? providerDynamicsID, CancellationToken concellationToken)
        {
            if (person.CompensationClaims == null) return true;
            try
            {
                foreach (CompensationClaim claim in person.CompensationClaims)
                {
                    SSG_Asset_BankingInformation ssg_bank = null;
                    if (claim.BankInfo != null)
                    {
                        BankingInformationEntity bank = _mapper.Map<BankingInformationEntity>(claim.BankInfo);
                        bank.InformationSource = providerDynamicsID;
                        ssg_bank = await _searchRequestService.CreateBankInfo(bank, concellationToken);
                    }

                    SSG_Employment ssg_employment = null;
                    if (claim.Employer != null)
                    {
                        EmploymentEntity employ = _mapper.Map<EmploymentEntity>(claim.Employer);
                        employ.InformationSource = providerDynamicsID;
                        employ.Date1 = claim.ReferenceDates?.SingleOrDefault(m => m.Index == 0)?.Value.DateTime;
                        employ.Date1Label = claim.ReferenceDates?.SingleOrDefault(m => m.Index == 0)?.Key;
                        ssg_employment = await _searchRequestService.CreateEmployment(employ, concellationToken);
                        if (claim.Employer.Phones != null)
                        {
                            foreach (var phone in claim.Employer.Phones)
                            {
                                SSG_EmploymentContact p = _mapper.Map<SSG_EmploymentContact>(phone);
                                p.Employment = ssg_employment;
                                await _searchRequestService.CreateEmploymentContact(p, concellationToken);
                            }
                        }
                    }

                    CompensationClaimEntity ssg_claim = _mapper.Map<CompensationClaimEntity>(claim);
                    ssg_claim.SearchRequest = request;
                    ssg_claim.InformationSource = providerDynamicsID;
                    ssg_claim.Person = ssg_person;
                    ssg_claim.BankingInformation = ssg_bank;
                    ssg_claim.Employment = ssg_employment;
                    await _searchRequestService.CreateCompensationClaim(ssg_claim, concellationToken);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        private async Task<bool> UploadInsuranceClaims(Person person, SSG_SearchRequest request, SSG_Person ssg_person, int? providerDynamicsID, CancellationToken concellationToken)
        {
            if (person.InsuranceClaims == null) return true;
            try
            {
                foreach (InsuranceClaim claim in person.InsuranceClaims)
                {
                    ICBCClaimEntity icbcClaim = _mapper.Map<ICBCClaimEntity>(claim);
                    icbcClaim.SearchRequest = request;
                    icbcClaim.InformationSource = providerDynamicsID;
                    icbcClaim.Person = ssg_person;
                    SSG_Asset_ICBCClaim ssg_claim = await _searchRequestService.CreateInsuranceClaim(icbcClaim, concellationToken);

                    if (claim.ClaimCentre != null && claim.ClaimCentre.ContactNumber!=null)
                    {
                        foreach(Phone phone in claim.ClaimCentre.ContactNumber)
                        {
                            SSG_SimplePhoneNumber phoneForAsset = _mapper.Map<SSG_SimplePhoneNumber>(phone);
                            phoneForAsset.SSG_Asset_ICBCClaim = ssg_claim;
                            await _searchRequestService.CreateSimplePhoneNumber(phoneForAsset, concellationToken);
                        }
                    }

                    if (claim.InsuredParties != null)
                    {
                        foreach(InvolvedParty party in claim.InsuredParties)
                        {
                            SSG_InvolvedParty involvedParty = _mapper.Map<SSG_InvolvedParty>(party);
                            involvedParty.SSG_Asset_ICBCClaim = ssg_claim;
                            await _searchRequestService.CreateInvolvedParty(involvedParty, concellationToken);
                        }
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

    }
}
