using AutoMapper;
using DynamicsAdapter.Web.PersonSearch.Models;
using Fams3Adapter.Dynamics;
using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.TaxIncomeInformation;
using Fams3Adapter.Dynamics.AssetOwner;
using Fams3Adapter.Dynamics.BankInfo;
using Fams3Adapter.Dynamics.CompensationClaim;
using Fams3Adapter.Dynamics.Email;
using Fams3Adapter.Dynamics.Employment;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.InsuranceClaim;
using Fams3Adapter.Dynamics.Name;
using Fams3Adapter.Dynamics.OtherAsset;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.PhoneNumber;
using Fams3Adapter.Dynamics.RelatedPerson;
using Fams3Adapter.Dynamics.ResultTransaction;
using Fams3Adapter.Dynamics.SafetyConcern;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Vehicle;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.PersonSearch
{
    public interface ISearchResultService
    {
        Task<bool> ProcessPersonFound(
            Person person, 
            ProviderProfile providerProfile, 
            SSG_SearchRequest searchRequest,
            Guid? searchApiRequestId,
            CancellationToken cancellationToken,
            SSG_Identifier sourceIdentifier=null);

        Task<SSG_SearchRequest> GetSearchRequest(string fileId, CancellationToken cancellationToken);
    }

    public class SearchResultService : ISearchResultService
    {
        private readonly ILogger<SearchResultService> _logger;
        private readonly ISearchRequestService _searchRequestService;
        private readonly IMapper _mapper;

        private SSG_Person _returnedPerson;
        private SSG_Identifier _sourceIdentifier;
        private int? _providerDynamicsID;
        private SSG_SearchRequest _searchRequest;
        private SSG_SearchApiRequest _searchApiRequest;
        private Person _foundPerson;
        private CancellationToken _cancellationToken;

        public SearchResultService(ISearchRequestService searchRequestService, ILoggerFactory loggerFactory, IMapper mapper)
        {
            _searchRequestService = searchRequestService;
            _logger = loggerFactory.CreateLogger<SearchResultService>();
            _mapper = mapper;
            _returnedPerson = null;
            _sourceIdentifier = null;
            _providerDynamicsID = null;
            _searchApiRequest = null;
            _searchRequest = null;
            _foundPerson = null;
        }

        public async Task<bool> ProcessPersonFound(
            Person person, 
            ProviderProfile providerProfile, 
            SSG_SearchRequest searchRequest, 
            Guid? searchApiRequestId, 
            CancellationToken cancellationToken, 
            SSG_Identifier sourceIdentifier = null)
        {
            if (person == null) return true;

            if (person.TaxIncomeInformations != null && person.TaxIncomeInformations.Any())
            {
                var taxCodes = await _searchRequestService.GetTaxCodes(cancellationToken);
                foreach (var taxIncomeInformation in person.TaxIncomeInformations)
                {
                    taxIncomeInformation.Description = taxCodes
                        .FirstOrDefault(x => x.TaxCode == taxIncomeInformation.TaxCode.Code)?
                        .Description;
                }
                person.MiddleName = person.TaxIncomeInformations.FirstOrDefault().MiddleName;
                person.OtherName = person.TaxIncomeInformations.FirstOrDefault().OtherName;
                person.FirstName = person.TaxIncomeInformations.FirstOrDefault().FirstName;// ?? person.FirstName;
                person.LastName = person.TaxIncomeInformations.FirstOrDefault().LastName;// ?? person.LastName;
                person.DateOfBirth = person.TaxIncomeInformations.FirstOrDefault().DateOfBirth;// ?? person.DateOfBirth;
                person.Date1 = DateTime.Now;
            }
            person.SuppliedBySystem = Constants.JcaSystem;
            _foundPerson = person;
            
            _providerDynamicsID = providerProfile.DynamicsID();
            _searchRequest = searchRequest;
            _sourceIdentifier = sourceIdentifier;
            _searchApiRequest = searchApiRequestId == null ? null : new SSG_SearchApiRequest() { SearchApiRequestId = (Guid)searchApiRequestId };
            _cancellationToken = cancellationToken;

            _returnedPerson = await UploadPerson();

            await UploadSafetyConcern();

            await UploadIdentifiers( );

            await UploadAddresses();

            await UploadTaxIncomeInformations(cancellationToken);

            await UploadPhoneNumbers();

            await UploadNames( );

            await UploadEmployment( );

            await UploadRelatedPersons();

            await UploadBankInfos();

            await UploadVehicles();

            await UploadOtherAssets();

            await UploadCompensationClaims();

            await UploadInsuranceClaims();

            await UploadEmails();

            return true;
        }

        public async Task<SSG_SearchRequest> GetSearchRequest(string fileId, CancellationToken token)
        {
            return await _searchRequestService.GetSearchRequest(fileId, _cancellationToken);
        }

        private async Task<bool> CreateResultTransaction(DynamicsEntity o)
        {
            if (_sourceIdentifier != null)
            {
                SSG_SearchRequestResultTransaction trans = new SSG_SearchRequestResultTransaction()
                {
                    SourceIdentifier = _sourceIdentifier,
                    SearchApiRequest = _searchApiRequest,
                    InformationSource = _providerDynamicsID
                };

                if (o != null)
                {
                    switch (o.GetType().Name)
                    {
                        case "SSG_Person": trans.Person = (SSG_Person)o; break;
                        case "SSG_Identifier": trans.ResultIdentifier = (SSG_Identifier)o; break;
                        case "SSG_Address": trans.Address = (SSG_Address)o; break;
                        case "SSG_PhoneNumber": trans.PhoneNumber = (SSG_PhoneNumber)o; break;
                        case "SSG_Aliase": trans.Name = (SSG_Aliase)o; break;
                        case "SSG_Employment": trans.Employment = (SSG_Employment)o; break;
                        case "SSG_Identity": trans.RelatedPerson = (SSG_Identity)o; break;
                        case "SSG_Asset_BankingInformation": trans.BankInfo = (SSG_Asset_BankingInformation)o; break;
                        case "SSG_Asset_Vehicle": trans.Vehicle = (SSG_Asset_Vehicle)o; break;
                        case "SSG_Asset_Other": trans.OtherAsset = (SSG_Asset_Other)o; break;
                        case "SSG_Asset_WorkSafeBcClaim": trans.CompensationClaim = (SSG_Asset_WorkSafeBcClaim)o; break;
                        case "SSG_Asset_ICBCClaim": trans.InsuranceClaim = (SSG_Asset_ICBCClaim)o; break;
                        case "SSG_SafetyConcernDetail": trans.SafetyConcern = (SSG_SafetyConcernDetail)o; break;
                        case "SSG_Email": trans.Email = (SSG_Email)o; break;
                        case "SSG_Taxincomeinformation": trans.Taxincomeinformation = (SSG_TaxIncomeInformation)o; break;
                        default: return false;
                    }
                }

                await _searchRequestService.CreateTransaction(trans, _cancellationToken);
                return true;
            }
            return false;
        }
        private async Task<SSG_Person> UploadPerson()
        {
            _logger.LogDebug($"Attempting to create the found person record for SearchRequest[{_searchRequest.SearchRequestId}]");
            PersonEntity ssg_person = _mapper.Map<PersonEntity>(_foundPerson);
            ssg_person.SearchRequest = _searchRequest;
            ssg_person.InformationSource = _providerDynamicsID;
            SSG_Person returnedPerson = await _searchRequestService.SavePerson(ssg_person, _cancellationToken);
            await CreateResultTransaction(returnedPerson);
            _logger.LogInformation($"Successfully created person {returnedPerson.PersonId}.");
            return returnedPerson;
        }

        private async Task<bool> UploadSafetyConcern()
        {
            if (string.IsNullOrEmpty(_foundPerson.CautionFlag)) return false;
            try 
            { 
                SafetyConcernEntity entity = _mapper.Map<SafetyConcernEntity>(_foundPerson);
                entity.SearchRequest = _searchRequest;
                entity.InformationSource = _providerDynamicsID;
                entity.Person = _returnedPerson;
                SSG_SafetyConcernDetail safetyConcern = await _searchRequestService.CreateSafetyConcern(entity, _cancellationToken);
                await CreateResultTransaction(safetyConcern);
                _logger.LogInformation($"Successfully created Safety Concern Detail {safetyConcern.SafetyConcernDetailId}.");
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return false;
            }
        }

        private async Task<bool> UploadIdentifiers()
        {
            if (_foundPerson.Identifiers == null) return true;
            try
            {
                _logger.LogDebug($"Attempting to create found identifier records for SearchRequest[{_searchRequest.SearchRequestId}]");

                foreach (var matchFoundPersonId in _foundPerson.Identifiers)
                {
                    IdentifierEntity identifier = _mapper.Map<IdentifierEntity>(matchFoundPersonId);
                    identifier.SearchRequest = _searchRequest;
                    identifier.InformationSource = _providerDynamicsID;
                    identifier.Person = _returnedPerson;
                    SSG_Identifier newIdentifier = await _searchRequestService.CreateIdentifier(identifier, _cancellationToken);
                    await CreateResultTransaction(newIdentifier);
                    _logger.LogInformation($"Successfully created identifier {newIdentifier.IdentifierId}.");
                }
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return false;
            }
        }

        private async Task<bool> UploadAddresses( )
        {
            if (_foundPerson.Addresses == null) return true;
            try
            {
                _logger.LogDebug($"Attempting to create found adddress records for SearchRequest[{_searchRequest.SearchRequestId}]");

                foreach (var address in _foundPerson.Addresses)
                {
                    AddressEntity addr = _mapper.Map<AddressEntity>(address);
                    addr.SearchRequest = _searchRequest;
                    addr.InformationSource = _providerDynamicsID;
                    addr.Person = _returnedPerson;
                    SSG_Address uploadedAddr = await _searchRequestService.CreateAddress(addr, _cancellationToken);
                    await CreateResultTransaction(uploadedAddr);
                    _logger.LogInformation($"Successfully created address {uploadedAddr.AddressId}.");
                }
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return false;
            }
        }

        private async Task<bool> UploadTaxIncomeInformations(CancellationToken cancellationToken)
        {
            if (_foundPerson.TaxIncomeInformations == null) 
                return true;

            try
            {
                _logger.LogDebug($"Attempting to create found tax income information records for SearchRequest[{_searchRequest.SearchRequestId}]");

                foreach (var taxinfo in _foundPerson.TaxIncomeInformations)
                {
                    // null guard clause
                    if (taxinfo?.TaxCode?.Code == null)
                        continue;

                        var txin = _mapper.Map<TaxIncomeInformationEntity>(taxinfo);
                        txin.SearchRequest = _searchRequest;
                        txin.InformationSource = _providerDynamicsID;
                        txin.Person = _returnedPerson;
                    var firstName = taxinfo.FirstName ?? txin.Person.FirstName;
                    var lastName = taxinfo.LastName ?? txin.Person.LastName;
                    var dateOfBirth = taxinfo.DateOfBirth.HasValue ? taxinfo.DateOfBirth.Value.DateTime : txin.Person.DateOfBirth;
                    txin.Person.FirstName = firstName;
                    txin.Person.LastName = lastName;
                    txin.Person.DateOfBirth = dateOfBirth;
                    txin.FullName = $"{firstName} {lastName}";
                    txin.Description = taxinfo.Description ?? taxinfo.TaxCode.Code;
                    txin.InformationSource = Constants.JcaSystem;    // Option Set -> Information Sources (System Set) -> JCA
                    txin.Date1 = DateTime.Now;
                        var uploadedTxin = await _searchRequestService.CreateTaxIncomeInformation(txin, _cancellationToken);
                        if (uploadedTxin != null)
                        {
                            await CreateResultTransaction(uploadedTxin);
                            _logger.LogInformation($"Successfully created tax income information {uploadedTxin.TaxincomeinformationId}.");
                        }
                    }
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return false;
            }
        }

        private async Task<bool> UploadPhoneNumbers()
        {
            if (_foundPerson.Phones == null) return true;
            try
            {
                _logger.LogDebug($"Attempting to create found phone records for SearchRequest[{_searchRequest.SearchRequestId}]");

                foreach (var phone in _foundPerson.Phones)
                {
                    PhoneNumberEntity ph = _mapper.Map<PhoneNumberEntity>(phone);
                    ph.SearchRequest = _searchRequest;
                    ph.InformationSource = _providerDynamicsID;
                    ph.Person = _returnedPerson;
                    SSG_PhoneNumber phoneNumber = await _searchRequestService.CreatePhoneNumber(ph, _cancellationToken);
                    await CreateResultTransaction(phoneNumber);
                    _logger.LogInformation($"Successfully created phone number {phoneNumber.PhoneNumberId}.");
                }
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return false;
            }
        }
        private async Task<bool> UploadNames( )
        {
            if (_foundPerson.Names == null) return true;
            try
            {
                _logger.LogDebug($"Attempting to create found name records for SearchRequest[{_searchRequest.SearchRequestId}]");
                foreach (var name in _foundPerson.Names)
                {
                    AliasEntity n = _mapper.Map<AliasEntity>(name);
                    n.SearchRequest = _searchRequest;
                    n.InformationSource = _providerDynamicsID;
                    n.Person = _returnedPerson;
                    SSG_Aliase alias = await _searchRequestService.CreateName(n, _cancellationToken);
                    await CreateResultTransaction(alias);
                    _logger.LogInformation($"Successfully created alias {alias.AliasId}.");
                }
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return false;
            }
        }
        private async Task<bool> UploadEmployment( )
        {
            if (_foundPerson.Employments == null) return true;
            try
            {
                _logger.LogDebug($"Attempting to create found employment records for SearchRequest[{_searchRequest.SearchRequestId}]");

                foreach (var employment in _foundPerson.Employments)
                {
                    EmploymentEntity e = _mapper.Map<EmploymentEntity>(employment);
                    e.SearchRequest = _searchRequest;
                    e.InformationSource = _providerDynamicsID;
                    e.Person = _returnedPerson;
                    SSG_Employment ssg_employment = await _searchRequestService.CreateEmployment(e, _cancellationToken);

                    //FAMS3-3742-OpenShift to stop creating Employment Contact records
                    //if (employment.Employer != null)
                    //{
                    //    foreach (var phone in employment.Employer.Phones)
                    //    {
                    //        EmploymentContactEntity p = _mapper.Map<EmploymentContactEntity>(phone);
                    //        p.Employment = ssg_employment;
                    //        await _searchRequestService.CreateEmploymentContact(p, _cancellationToken);
                    //    }
                    //}

                    await CreateResultTransaction(ssg_employment);
                    _logger.LogInformation($"Successfully created employment {ssg_employment.EmploymentId}.");
                }
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return false;
            }
        }

        private async Task<bool> UploadRelatedPersons()
        {
            if (_foundPerson.RelatedPersons == null) return true;
            try
            {
                _logger.LogDebug($"Attempting to create found related person records for SearchRequest[{_searchRequest.SearchRequestId}]");

                foreach (var relatedPerson in _foundPerson.RelatedPersons)
                {
                    RelatedPersonEntity n = _mapper.Map<RelatedPersonEntity>(relatedPerson);
                    n.SearchRequest = _searchRequest;
                    n.InformationSource = _providerDynamicsID;
                    n.Person = _returnedPerson;
                    SSG_Identity relate = await _searchRequestService.CreateRelatedPerson(n, _cancellationToken);
                    await CreateResultTransaction(relate);
                    _logger.LogInformation($"Successfully created related person (ssg_identity) '{relate.RelatedPersonId}'.");
                }
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return false;
            }
        }

        private async Task<bool> UploadBankInfos()
        {
            if (_foundPerson.BankInfos == null) return true;
            try
            {
                _logger.LogDebug($"Attempting to create bank info records for SearchRequest[{_searchRequest.SearchRequestId}]");
                foreach (var bankInfo in _foundPerson.BankInfos)
                {
                    BankingInformationEntity bank = _mapper.Map<BankingInformationEntity>(bankInfo);
                    bank.SearchRequest = _searchRequest;
                    bank.InformationSource = _providerDynamicsID;
                    bank.Person = _returnedPerson;
                    SSG_Asset_BankingInformation ssgBank = await _searchRequestService.CreateBankInfo(bank, _cancellationToken);
                    await CreateResultTransaction(ssgBank);
                    _logger.LogInformation($"Successfully created bank information {ssgBank.BankingInformationId}.");
                }
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return false;
            }
        }

        private async Task<bool> UploadVehicles()
        {
            if (_foundPerson.Vehicles == null) return true;
            try
            {
                _logger.LogDebug($"Attempting to create vehicles records for SearchRequest[{_searchRequest.SearchRequestId}]");

                foreach (var v in _foundPerson.Vehicles)
                {
                    VehicleEntity vehicle = _mapper.Map<VehicleEntity>(v);
                    vehicle.SearchRequest = _searchRequest;
                    vehicle.InformationSource = _providerDynamicsID;
                    vehicle.Person = _returnedPerson;
                    SSG_Asset_Vehicle ssgVehicle = await _searchRequestService.CreateVehicle(vehicle, _cancellationToken);
                    if (v.Owners != null)
                    {
                        foreach (var owner in v.Owners)
                        {
                            AssetOwnerEntity assetOwner = _mapper.Map<AssetOwnerEntity>(owner);
                            assetOwner.Vehicle = ssgVehicle;
                            var assetOwnerResult = await _searchRequestService.CreateAssetOwner(assetOwner, _cancellationToken);
                            _logger.LogInformation($"Successfully created asset owner {assetOwnerResult.AssetOwnerId} for vehicle.");
                        }
                    }
                    await CreateResultTransaction(ssgVehicle);
                    _logger.LogInformation($"Successfully created vehicle {ssgVehicle.VehicleId}.");
                }
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return false;
            }
        }

        private async Task<bool> UploadOtherAssets()
        {
            if (_foundPerson.OtherAssets == null) return true;
            try
            {
                _logger.LogDebug($"Attempting to create other assets records for SearchRequest[{_searchRequest.SearchRequestId}]");

                foreach (OtherAsset asset in _foundPerson.OtherAssets)
                {
                    AssetOtherEntity other = _mapper.Map<AssetOtherEntity>(asset);
                    other.SearchRequest = _searchRequest;
                    other.InformationSource = _providerDynamicsID;
                    other.Person = _returnedPerson;
                    SSG_Asset_Other ssgOtherAsset = await _searchRequestService.CreateOtherAsset(other, _cancellationToken);
                    if (asset.Owners != null)
                    {
                        foreach (var owner in asset.Owners)
                        {
                            AssetOwnerEntity assetOwner = _mapper.Map<AssetOwnerEntity>(owner);
                            assetOwner.OtherAsset = ssgOtherAsset;
                            var assetOwnerResult = await _searchRequestService.CreateAssetOwner(assetOwner, _cancellationToken);
                            _logger.LogInformation($"Successfully created asset owner {assetOwnerResult.AssetOwnerId} for other asset.");
                        }
                    }

                    await CreateResultTransaction(ssgOtherAsset);
                    _logger.LogInformation($"Successfully created other asset {ssgOtherAsset.AssetOtherId}.");
                }
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return false;
            }
        }

        private async Task<bool> UploadCompensationClaims()
        {
            if (_foundPerson.CompensationClaims == null) return true;
            try
            {
                _logger.LogDebug($"Attempting to create compnsation claims records for SearchRequest[{_searchRequest.SearchRequestId}]");

                foreach (CompensationClaim claim in _foundPerson.CompensationClaims)
                {
                    BankingInformationEntity bankEntity=null;
                    if (claim.BankInfo != null)
                    {
                        bankEntity = _mapper.Map<BankingInformationEntity>(claim.BankInfo);
                        bankEntity.InformationSource = _providerDynamicsID;
                    }

                    EmploymentEntity employmentEntity = null;
                    if (claim.Employer != null)
                    {
                        employmentEntity = _mapper.Map<EmploymentEntity>(claim.Employer);
                        employmentEntity.InformationSource = _providerDynamicsID;
                        employmentEntity.Date1 = claim.ReferenceDates?.SingleOrDefault(m => m.Index == 0)?.Value.DateTime;
                        employmentEntity.Date1Label = claim.ReferenceDates?.SingleOrDefault(m => m.Index == 0)?.Key;
                        //FAMS3-3742: OpenShift to stop creating Employment Contact records
                        //List<EmploymentContactEntity> contacts = new List<EmploymentContactEntity>();
                        //if (claim.Employer.Phones != null)
                        //{
                        //    foreach (var phone in claim.Employer.Phones)
                        //    {
                        //        EmploymentContactEntity p = _mapper.Map<EmploymentContactEntity>(phone);
                        //        contacts.Add(p);
                        //    }
                        //}
                        //employmentEntity.EmploymentContactEntities = contacts.ToArray();
                    }

                    CompensationClaimEntity ssg_claim = _mapper.Map<CompensationClaimEntity>(claim);
                    ssg_claim.SearchRequest = _searchRequest;
                    ssg_claim.InformationSource = _providerDynamicsID;
                    ssg_claim.Person =  _returnedPerson;
                    ssg_claim.BankInformationEntity = bankEntity;
                    ssg_claim.EmploymentEntity = employmentEntity;

                    SSG_Asset_WorkSafeBcClaim ssg_Claim = await _searchRequestService.CreateCompensationClaim(ssg_claim, _cancellationToken);
                    await CreateResultTransaction(ssg_Claim);
                    _logger.LogInformation($"Successfully created Worksafe BC claim {ssg_Claim.CompensationClaimId}.");
                }
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return false;
            }
        }

        private async Task<bool> UploadInsuranceClaims()
        {
            if (_foundPerson.InsuranceClaims == null) return true;
            try
            {
                _logger.LogDebug($"Attempting to create insurance claims records for SearchRequest[{_searchRequest.SearchRequestId}]");

                foreach (InsuranceClaim claim in _foundPerson.InsuranceClaims)
                {
                    ICBCClaimEntity icbcClaim = _mapper.Map<ICBCClaimEntity>(claim);
                    icbcClaim.SearchRequest = _searchRequest;
                    icbcClaim.InformationSource = _providerDynamicsID;
                    icbcClaim.Person = _returnedPerson;
                    SSG_Asset_ICBCClaim ssg_claim = await _searchRequestService.CreateInsuranceClaim(icbcClaim, _cancellationToken);
                    await CreateResultTransaction(ssg_claim);
                    _logger.LogInformation($"Successfully created ICBC claim {ssg_claim.ICBCClaimId}.");

                    if (claim.ClaimCentre != null && claim.ClaimCentre.ContactNumber!=null)
                    {
                        foreach(Phone phone in claim.ClaimCentre.ContactNumber)
                        {
                            SimplePhoneNumberEntity phoneForAsset = _mapper.Map<SimplePhoneNumberEntity>(phone);
                            phoneForAsset.SSG_Asset_ICBCClaim = ssg_claim;
                            var phoneForAssertResult = await _searchRequestService.CreateSimplePhoneNumber(phoneForAsset, _cancellationToken);
                            _logger.LogInformation($"Successfully created simple phone number {phoneForAssertResult.SimplePhoneNumberId}.");
                        }
                    }

                    if (claim.InsuredParties != null)
                    {
                        foreach(InvolvedParty party in claim.InsuredParties)
                        {
                            InvolvedPartyEntity involvedParty = _mapper.Map<InvolvedPartyEntity>(party);
                            involvedParty.SSG_Asset_ICBCClaim = ssg_claim;
                            var involvedPartyResult = await _searchRequestService.CreateInvolvedParty(involvedParty, _cancellationToken);
                            _logger.LogInformation($"Successfully created involved party {involvedPartyResult.InvolvedPartyId}.");
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return false;
            }
        }

        private async Task<bool> UploadEmails()
        {
            if (_foundPerson.Emails == null) return true;
            try
            {
                _logger.LogDebug($"Attempting to create emails records for SearchRequest[{_searchRequest.SearchRequestId}]");

                foreach (var e in _foundPerson.Emails)
                {
                    EmailEntity email = _mapper.Map<EmailEntity>(e);
                    email.SearchRequest = _searchRequest;
                    //email.SupplierTypeCode = _providerDynamicsID;
                    email.Person = _returnedPerson;
                    SSG_Email ssgEmail = await _searchRequestService.CreateEmail(email, _cancellationToken);
                    await CreateResultTransaction(ssgEmail);
                    _logger.LogInformation($"Successfully created email {ssgEmail.EmailId}.");
                }
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return false;
            }
        }

        private void LogException(Exception ex)
        {
            var webRequestException = ex as Simple.OData.Client.WebRequestException;
            if (webRequestException != null)
            {
                var properties = new Dictionary<string, object>();
                properties.Add("AbsoluteUri", webRequestException.RequestUri?.AbsoluteUri);
                properties.Add("Response", webRequestException.Response);
                using (_logger.BeginScope(properties)) 
                {
                    _logger.LogError(ex.ToString());
                    _logger.LogError(ex.Message);
                    _logger.LogError(ex.StackTrace);
                } ;
                
            }
            else
            {
                _logger.LogError(ex.ToString());
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
            }
        }
    }
}
