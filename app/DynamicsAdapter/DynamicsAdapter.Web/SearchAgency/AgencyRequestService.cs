using AutoMapper;
using BcGov.Fams3.Utils.Object;
using Fams3Adapter.Dynamics.Update;
using DynamicsAdapter.Web.SearchAgency.Models;
using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.Employment;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.Name;
using Fams3Adapter.Dynamics.Notes;
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
using System.Collections.Generic;
using Fams3Adapter.Dynamics.Agency;
using DynamicsAdapter.Web.SearchAgency.Exceptions;
using Fams3Adapter.Dynamics.SafetyConcern;
using Polly;
using ContractNotificationType = BcGov.Fams3.SearchApi.Contracts.SearchRequest.NotificationType;
using ContractStatus = BcGov.Fams3.SearchApi.Contracts.SearchRequest.NotificationStatusEnum;

namespace DynamicsAdapter.Web.SearchAgency
{
    public interface IAgencyRequestService
    {
        Task<SSG_SearchRequest> ProcessSearchRequestOrdered(SearchRequestOrdered searchRequestOrdered);
        Task<SSG_SearchRequest> ProcessCancelSearchRequest(SearchRequestOrdered cancelSearchRequest);
        Task<SSG_SearchRequest> ProcessUpdateSearchRequest(SearchRequestOrdered updateSearchRequest);
        SSG_SearchRequest GetSSGSearchRequest();
        Task<bool> SystemCancelSSGSearchRequest(SSG_SearchRequest searchRequest);
        Task<bool> ProcessNotificationAcknowledgement(Acknowledgement ack, Guid ApiCallGuid, bool isAmendment);
        Task<SSG_SearchRequest> RefreshSearchRequest(Guid searchRequestId);
        Task SubmitSearchRequestToQueue(Guid searchRequestId);
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
            searchRequestEntity.SendNotificationOnCreation = true;
            _uploadedSearchRequest = await _searchRequestService.CreateSearchRequest(searchRequestEntity, cts.Token);
            if (_uploadedSearchRequest == null) return null;
            _logger.LogInformation("Create Search Request successfully");

            PersonEntity personEntity = _mapper.Map<PersonEntity>(_personSought);
            personEntity.SearchRequest = _uploadedSearchRequest;
            personEntity.InformationSource = InformationSourceType.Request.Value;
            personEntity.IsCreatedByAgency = true;
            personEntity.IsPrimary = true;
            _uploadedPerson = await _searchRequestService.SavePerson(personEntity, _cancellationToken);
            _logger.LogInformation("Create Person successfully");

            await UploadIdentifiers();
            await UploadAddresses();
            await UploadPhones();
            await UploadEmployment();
            await UploadRelatedPersons();
            await UploadRelatedApplicant(_uploadedSearchRequest.ApplicantFirstName, _uploadedSearchRequest.ApplicantLastName);
            await UploadAliases();
            await UploadSafetyConcern();
            return _uploadedSearchRequest;
        }

        public async Task SubmitSearchRequestToQueue(Guid searchRequestId)
        {
            await SubmitToQueue(searchRequestId);
        }

        public async Task<SSG_SearchRequest> RefreshSearchRequest(Guid searchRequestId) 
        {
            return await _searchRequestService.GetCurrentSearchRequest(searchRequestId);
        }

        public async Task<SSG_SearchRequest> ProcessCancelSearchRequest(SearchRequestOrdered searchRequestOrdered)
        {
            var cts = new CancellationTokenSource();
            _cancellationToken = cts.Token;
            await VerifySearchRequest(searchRequestOrdered);
            return await _searchRequestService.CancelSearchRequest(searchRequestOrdered.SearchRequestKey, searchRequestOrdered?.Person?.Agency?.Notes, _cancellationToken);
        }

        public async Task<SSG_SearchRequest> ProcessUpdateSearchRequest(SearchRequestOrdered searchRequestOrdered)
        {
            var cts = new CancellationTokenSource();
            _cancellationToken = cts.Token;
            SSG_SearchRequest existedSearchRequest = await VerifySearchRequest(searchRequestOrdered);
            if (existedSearchRequest == null) return null;
            existedSearchRequest.IsDuplicated = true;
            _uploadedSearchRequest = existedSearchRequest;

            //get existedPersonSought
            SSG_Person existedSoughtPerson = existedSearchRequest?.SSG_Persons?.FirstOrDefault(
                    m => m.IsPrimary == true);
            if (existedSoughtPerson == null)
            {
                string error = "the updating personSought does not exist. something is wrong.";
                _logger.LogError(error);
                throw new Exception(error);
            }
            existedSoughtPerson = await _searchRequestService.GetPerson(existedSoughtPerson.PersonId, _cancellationToken);
            existedSoughtPerson.IsDuplicated = true;
            _uploadedPerson = existedSoughtPerson;


            SearchRequestEntity newSearchRequest = _mapper.Map<SearchRequestEntity>(searchRequestOrdered);
            if (newSearchRequest == null)
            {
                string error = "cannot do updating as newSearchRequest is null";
                _logger.LogError(error);
                throw new Exception(error);
            }

            //update searchRequestEntity
            await UpdateSearchRequest(newSearchRequest);

            //update notesEntity
            if (!String.IsNullOrEmpty(newSearchRequest.Notes)
                && !String.Equals(existedSearchRequest.Notes, newSearchRequest.Notes, StringComparison.InvariantCultureIgnoreCase))
            {
                await UploadNotes(newSearchRequest, existedSearchRequest);
            }

            //update PersonEntity
            if (searchRequestOrdered.Person == null)
            {
                string error = "the searchRequestOrdered does not contain Person. The request is wrong.";
                _logger.LogError(error);
                throw new Exception(error);
            }
            _personSought = searchRequestOrdered.Person;

            await UpdatePersonSought();
            await UpdateSafetyConcern();

            //update RelatedPerson applicant
            await UpdateRelatedApplicant((string.IsNullOrEmpty(newSearchRequest.ApplicantFirstName) && string.IsNullOrEmpty(newSearchRequest.ApplicantLastName)) ? null : new RelatedPersonEntity()
            {
                FirstName = newSearchRequest.ApplicantFirstName,
                LastName = newSearchRequest.ApplicantLastName,
                StatusCode = 1
            });

            //update identifiers
            //await UpdateIdentifiers();
            await UploadIdentifiers(true);

            //update employment
            await UploadEmployment(true);

            //for phones, addresses, relatedPersons, names are same as creation, as if different, add new one, if same, ignore
            await UploadAddresses(true);
            await UploadPhones(true);
            await UploadRelatedPersons(true);
            await UploadAliases(true);



            return _uploadedSearchRequest;
        }

        public SSG_SearchRequest GetSSGSearchRequest()
        {
            return _uploadedSearchRequest;
        }

        public async Task<bool> SystemCancelSSGSearchRequest(SSG_SearchRequest request)
        {
            await Policy.HandleResult<bool>(r => r == false)
                   .Or<Exception>()
                   .WaitAndRetryAsync(3,retryAttempt => TimeSpan.FromMinutes(2)) //retry 3 times and pause 1min between each call
                   .ExecuteAndCaptureAsync(()=> SystemCancelSearchRequest(request));
            return true;
        }

        public async Task<bool> ProcessNotificationAcknowledgement(Acknowledgement ack, Guid ApiCallGuid, bool isAmendment)
        {
            try
            {
                var cts = new CancellationTokenSource();               
                bool success=false;
                string notes = null;
                if(ack.NotificationType== ContractNotificationType.RequestClosed)
                {
                    success = ack.Status == ContractStatus.SUCCESS;
                    string name = isAmendment ? "amendment" : "search";
                    if (success) 
                        notes = $"{ack.ProviderProfile?.Name} received {name} response successfully.";
                    else
                        notes = $"{ack.ProviderProfile?.Name} did not receive {name} response successfully.";
                    return await _searchRequestService.UpdateApiCall(ApiCallGuid, success, notes, cts.Token);
                }
                return false;
            }
            catch (Exception e)
            {
                _logger.LogInformation($"set apiCall {ApiCallGuid} failed" + e.Message);
                return false;
            }
        }

        private async Task<bool> SystemCancelSearchRequest(SSG_SearchRequest request)
        {
            try
            {
                SSG_SearchRequest sr = await _searchRequestService.SystemCancelSearchRequest(request, _cancellationToken);
                if (sr.StatusCode != SearchRequestStatusCode.SystemCancelled.Value) return false;
            }catch(Exception e)
            {
                _logger.LogInformation($"{request.FileId} File System Cancel failed."+e.Message);
                return false;
            }
            _logger.LogInformation($"{request.FileId} File has been cancelled successfully.");
            return true;
        }

        private async Task<SSG_SearchRequest> VerifySearchRequest(SearchRequestOrdered searchRequestOrdered)
        {
            //get existedSearchRequest
            SSG_SearchRequest existedSearchRequest = await _searchRequestService.GetSearchRequest(searchRequestOrdered.SearchRequestKey, _cancellationToken);
            if (existedSearchRequest == null)
            {
                string error = "the search request does not exist.";
                _logger.LogInformation(error);
                throw new Exception(error);
            }
            if (existedSearchRequest.StatusCode == SearchRequestStatusCode.SearchRequestCancelled.Value)
            {
                throw new AgencyRequestException("fileCancelled", new Exception($"File {searchRequestOrdered.SearchRequestKey} is cancelled."));
            }
            if (existedSearchRequest.StatusCode == SearchRequestStatusCode.SearchRequestClosed.Value)
            {
                throw new AgencyRequestException("fileClosed", new Exception($"File {searchRequestOrdered.SearchRequestKey} is closed."));
            }
            if (existedSearchRequest.Agency == null || existedSearchRequest.Agency.AgencyCode != searchRequestOrdered?.Person?.Agency?.Code)
            {
                throw new AgencyRequestException("wrongAgency", new Exception($"Wrong Agency Code."));
            }
            return existedSearchRequest;
        }

        private async Task<bool> UploadIdentifiers(bool inUpdateProcess = false)
        {
            if (_personSought.Identifiers == null) return true;
            _logger.LogDebug($"Attempting to create identifier records for SearchRequest.");

            foreach (var personId in _personSought.Identifiers.Where(m => m.Owner == OwnerType.PersonSought))
            {
                IdentifierEntity identifier = _mapper.Map<IdentifierEntity>(personId);
                identifier.SearchRequest = _uploadedSearchRequest;
                identifier.InformationSource = InformationSourceType.Request.Value;
                identifier.Person = _uploadedPerson;
                identifier.IsCreatedByAgency = true;
                identifier.UpdatedByApi = inUpdateProcess;
                if (inUpdateProcess)
                    identifier.UpdateDetails = "New Identifier";
                SSG_Identifier newIdentifier = await _searchRequestService.CreateIdentifier(identifier, _cancellationToken);
            }

            //following is for alias person has identifiers, this situation never happened before. But the data structure is there.
            if (_personSought.Names == null) return true;
            foreach (Name personName in _personSought.Names.Where(m => m.Owner == OwnerType.PersonSought))
            {
                if (personName.Identifiers != null)
                {
                    foreach (var personId in personName.Identifiers)
                    {
                        IdentifierEntity identifier = _mapper.Map<IdentifierEntity>(personId);
                        identifier.SearchRequest = _uploadedSearchRequest;
                        identifier.InformationSource = InformationSourceType.Request.Value;
                        identifier.Person = _uploadedPerson;
                        identifier.IsCreatedByAgency = true;
                        identifier.UpdatedByApi = inUpdateProcess;
                        if (inUpdateProcess)
                            identifier.UpdateDetails = "New Identifier";
                        SSG_Identifier newIdentifier = await _searchRequestService.CreateIdentifier(identifier, _cancellationToken);
                    }
                }
            }
            _logger.LogInformation("Create identifier records for SearchRequest successfully");
            return true;
        }

        private async Task<bool> UploadAddresses(bool inUpdateProcess = false)
        {
            if (_personSought.Addresses == null) return true;

            _logger.LogDebug($"Attempting to create adddress for SoughtPerson");

            foreach (var address in _personSought.Addresses.Where(m => m.Owner == OwnerType.PersonSought))
            {
                AddressEntity addr = _mapper.Map<AddressEntity>(address);
                addr.SearchRequest = _uploadedSearchRequest;
                addr.InformationSource = InformationSourceType.Request.Value;
                addr.Person = _uploadedPerson;
                addr.IsCreatedByAgency = true;
                addr.UpdatedByApi = inUpdateProcess;
                if (inUpdateProcess) addr.UpdateDetails = "New Address";
                SSG_Address uploadedAddr = await _searchRequestService.CreateAddress(addr, _cancellationToken);
            }

            //following is for alias person has addresses, this situation never happened before. But the data structure is there.
            if (_personSought.Names == null) return true;
            foreach (Name personName in _personSought.Names?.Where(m => m.Owner == OwnerType.PersonSought))
            {
                if (personName.Addresses != null)
                {
                    foreach (var address in personName.Addresses)
                    {
                        AddressEntity addr = _mapper.Map<AddressEntity>(address);
                        addr.SearchRequest = _uploadedSearchRequest;
                        addr.InformationSource = InformationSourceType.Request.Value;
                        addr.Person = _uploadedPerson;
                        addr.IsCreatedByAgency = true;

                        SSG_Address uploadedAddr = await _searchRequestService.CreateAddress(addr, _cancellationToken);
                    }
                }
            }
            _logger.LogInformation("Create addresses records for SearchRequest successfully");
            return true;
        }

        private async Task<bool> UploadPhones(bool inUpdateProcess = false)
        {
            if (_personSought.Phones == null) return true;

            _logger.LogDebug($"Attempting to create Phones for SoughtPerson");

            foreach (var phone in _personSought.Phones.Where(m => m.Owner == OwnerType.PersonSought))
            {
                PhoneNumberEntity ph = _mapper.Map<PhoneNumberEntity>(phone);
                ph.SearchRequest = _uploadedSearchRequest;
                ph.InformationSource = InformationSourceType.Request.Value;
                ph.Person = _uploadedPerson;
                ph.IsCreatedByAgency = true;
                ph.UpdatedByApi = inUpdateProcess;
                if (inUpdateProcess)
                {
                    ph.UpdateDetails = "Create Phone";
                }
                SSG_PhoneNumber uploadedPhone = await _searchRequestService.CreatePhoneNumber(ph, _cancellationToken);
            }

            //following is for alias person has phones, this situation never happened before. But the data structure is there.
            if (_personSought.Names == null) return true;
            foreach (Name personName in _personSought.Names?.Where(m => m.Owner == OwnerType.PersonSought))
            {
                if (personName.Phones != null)
                {
                    foreach (var phone in personName.Phones)
                    {
                        PhoneNumberEntity phoneNumber = _mapper.Map<PhoneNumberEntity>(phone);
                        phoneNumber.SearchRequest = _uploadedSearchRequest;
                        phoneNumber.InformationSource = InformationSourceType.Request.Value;
                        phoneNumber.Person = _uploadedPerson;
                        phoneNumber.IsCreatedByAgency = true;
                        SSG_PhoneNumber uploadedPhone = await _searchRequestService.CreatePhoneNumber(phoneNumber, _cancellationToken);
                    }
                }
            }
            _logger.LogInformation("Create phones records for SearchRequest successfully");
            return true;
        }

         private async Task<bool> UploadSafetyConcern(bool inUpdateProcess = false)
        {
            if( string.IsNullOrEmpty(_personSought.CautionFlag) 
                && string.IsNullOrEmpty(_personSought.CautionReason) 
                && string.IsNullOrEmpty(_personSought.CautionNotes))
                return true;
            SafetyConcernEntity entity = _mapper.Map<SafetyConcernEntity>(_personSought);
            entity.SearchRequest = _uploadedSearchRequest;
            entity.InformationSource = InformationSourceType.Request.Value;
            entity.Person = _uploadedPerson;
            entity.IsCreatedByAgency = true;
            if (inUpdateProcess) entity.UpdateDetails = "New Safety Concern";
            await _searchRequestService.CreateSafetyConcern(entity, _cancellationToken);
            _logger.LogInformation("Create Safety Concern records for SearchRequest successfully");
            return true;
        }

        private async Task<bool> UploadEmployment(bool inUpdateProcess = false)
        {
            if (_personSought.Employments == null) return true;

            _logger.LogDebug($"Attempting to create employment records for PersonSought.");

            foreach (var employment in _personSought.Employments)
            {
                EmploymentEntity e = _mapper.Map<EmploymentEntity>(employment);
                e.SearchRequest = _uploadedSearchRequest;
                e.InformationSource = InformationSourceType.Request.Value;
                e.Person = _uploadedPerson;
                e.IsCreatedByAgency = true;
                e.UpdatedByApi = inUpdateProcess;
                if (inUpdateProcess) e.UpdateDetails = "New Employment";
                SSG_Employment ssg_employment = await _searchRequestService.CreateEmployment(e, _cancellationToken);

                //FAMS3-3742(OpenShift to stop creating Employment Contact records)
                //if (employment.Employer != null)
                //{
                //    foreach (var phone in employment.Employer.Phones)
                //    {
                //        EmploymentContactEntity p = _mapper.Map<EmploymentContactEntity>(phone);
                //        p.Employment = ssg_employment;
                //        if (inUpdateProcess) e.UpdateDetails = "New EmploymentContact";
                //        await _searchRequestService.CreateEmploymentContact(p, _cancellationToken);
                //    }
                //}
            }

            _logger.LogInformation("Create employment records for SearchRequest successfully");
            return true;
        }

        private async Task<bool> UploadRelatedPersons(bool inUpdateProcess = false)
        {
            if (_personSought.RelatedPersons == null) return true;

            _logger.LogDebug($"Attempting to create related person records person sought.");

            foreach (var relatedPerson in _personSought.RelatedPersons)
            {
                RelatedPersonEntity n = _mapper.Map<RelatedPersonEntity>(relatedPerson);
                n.SearchRequest = _uploadedSearchRequest;
                n.InformationSource = InformationSourceType.Request.Value;
                n.Person = _uploadedPerson;
                n.UpdatedByApi = inUpdateProcess;
                if (inUpdateProcess)
                {
                    n.UpdateDetails = "Create New Related Person";
                }
                n.IsCreatedByAgency = true;
                SSG_Identity relate = await _searchRequestService.CreateRelatedPerson(n, _cancellationToken);
            }

            _logger.LogInformation("Create RelatedPersons records for SearchRequest successfully");
            return true;
        }

        private async Task<bool> UploadRelatedApplicant(string firstName, string lastName)
        {
            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName)) return true;

            _logger.LogDebug($"Attempting to create related applicant.");

            RelatedPersonEntity n = new RelatedPersonEntity
            {
                FirstName = firstName,
                LastName = lastName,
                SearchRequest = _uploadedSearchRequest,
                InformationSource = InformationSourceType.Request.Value,
                Person = _uploadedPerson,
                IsCreatedByAgency = true,
                PersonType = RelatedPersonPersonType.Applicant.Value,
                StatusCode = 1
            };

            SSG_Identity relate = await _searchRequestService.CreateRelatedPerson(n, _cancellationToken);
            _logger.LogInformation("Create Related Applicant for SearchRequest successfully");
            return true;
        }

        private async Task<bool> UploadAliases(bool inUpdateProcess = false)
        {
            if (_personSought.Names == null) return true;

            _logger.LogDebug($"Attempting to create aliases for SoughtPerson");

            foreach (var name in _personSought.Names.Where(m => m.Owner == OwnerType.PersonSought))
            {
                AliasEntity aliasEntity = _mapper.Map<AliasEntity>(name);
                aliasEntity.SearchRequest = _uploadedSearchRequest;
                aliasEntity.InformationSource = InformationSourceType.Request.Value;
                aliasEntity.Person = _uploadedPerson;
                aliasEntity.IsCreatedByAgency = true;
                aliasEntity.UpdatedByApi = inUpdateProcess;
                if (inUpdateProcess)
                    aliasEntity.UpdateDetails = "New Alias";
                await _searchRequestService.CreateName(aliasEntity, _cancellationToken);
            }
            _logger.LogInformation("Create alias records for SearchRequest successfully");
            return true;
        }

        private async Task<bool> UpdateSearchRequest(SearchRequestEntity newSR)
        {
            string originNotes = _uploadedSearchRequest.Notes;
            SSG_SearchRequest clonedSR = _uploadedSearchRequest.Clone();

            Dictionary<string, object> updatedFields = (Dictionary<string, object>)clonedSR.GetUpdateEntries(newSR);

            if (newSR.SearchReasonCode != null && !newSR.SearchReasonCode.Equals(_uploadedSearchRequest.SearchReason?.ReasonCode, StringComparison.InvariantCultureIgnoreCase))
            {
                SSG_SearchRequestReason reason = await _searchRequestService.GetSearchReason(newSR.SearchReasonCode, _cancellationToken);
                updatedFields.Add("ssg_RequestCategoryText", reason);
            }

            if (newSR.AgencyOfficeLocationText != null && !newSR.AgencyOfficeLocationText.Equals(_uploadedSearchRequest.AgencyLocation.LocationCode, StringComparison.InvariantCultureIgnoreCase))
            {
                SSG_AgencyLocation location = await _searchRequestService.GetSearchAgencyLocation(
                                                        newSR.AgencyOfficeLocationText,
                                                        newSR.AgencyCode,
                                                        _cancellationToken);
                updatedFields.Add("ssg_AgencyLocation", location);
            }

            //comment out this as even there is nothing needed to change, we still need to set ssg_updatedbyagency to true to
            //trigger the Dynamics to send out the estimation date notification.
            //if (updatedFields.Count > 0) //except notes, there is something else changed.
            //{
            await _searchRequestService.UpdateSearchRequest(_uploadedSearchRequest.SearchRequestId, updatedFields, _cancellationToken);
                _logger.LogInformation("Update Search Request successfully");
            //}


            return true;
        }

        private async Task<bool> UpdatePersonSought()
        {
            PersonEntity newPersonEntity = _mapper.Map<PersonEntity>(_personSought);
            newPersonEntity.SearchRequest = _uploadedSearchRequest;
            
            Dictionary<string, object> updatedFields = (Dictionary<string, object>)_uploadedPerson.Clone().GetUpdateEntries(newPersonEntity);
            if (updatedFields.Count > 0)
            {
                await _searchRequestService.UpdatePerson(_uploadedPerson, updatedFields, newPersonEntity, _cancellationToken);
                _logger.LogInformation("Update Person successfully");
            }
            return true;
        }

        private async Task<bool> UpdateSafetyConcern()
        {
            SSG_SafetyConcernDetail originalSafeEntity = _uploadedPerson.SSG_SafetyConcernDetails?.FirstOrDefault(m => m.IsCreatedByAgency);
            if(originalSafeEntity == null)
            {
                await UploadSafetyConcern(true);
            }
            else
            {
                SafetyConcernEntity newSafeEntity = _mapper.Map<SafetyConcernEntity>(_personSought);
                newSafeEntity.UpdatedByApi = true;
                Dictionary<string, object> updatedFields = (Dictionary<string, object>)originalSafeEntity.Clone().GetUpdateEntries(newSafeEntity);
                if (updatedFields.Count > 0)
                {
                    await _searchRequestService.UpdateSafetyConcern(originalSafeEntity.SafetyConcernDetailId, updatedFields, _cancellationToken);
                    _logger.LogInformation("Update Person successfully");
                }
            }

            return true;
        }

        private async Task<bool> UpdateRelatedApplicant(RelatedPersonEntity newApplicantEntity)
        {
            if (newApplicantEntity == null) return true;

            //update or add relation relatedPerson
            SSG_Identity originalRelatedApplicant = _uploadedPerson.SSG_Identities?.FirstOrDefault(
            m => m.InformationSource == InformationSourceType.Request.Value
            && m.PersonType == RelatedPersonPersonType.Applicant.Value);

            if (originalRelatedApplicant == null)
            {
                await UploadRelatedApplicant(newApplicantEntity.FirstName, newApplicantEntity.LastName);
                _logger.LogInformation("Create Related Applicant for SearchRequest successfully");
            }
            else
            {
                Dictionary<string, object> updatedFields = (Dictionary<string, object>)originalRelatedApplicant.Clone().GetUpdateEntries(newApplicantEntity);
                if (updatedFields.Count > 0)
                {
                    await _searchRequestService.UpdateRelatedPerson(originalRelatedApplicant.RelatedPersonId, updatedFields, _cancellationToken);
                    _logger.LogInformation("Update Related Applicant records for SearchRequest successfully");
                }
            }

            return true;
        }

        //this function is never been used.
        private async Task<bool> UpdateEmployment()
        {
            if (_personSought.Employments == null) return true;

            _logger.LogDebug($"Attempting to update employment records for PersonSought.");

            SSG_Employment originalEmployment = _uploadedPerson.SSG_Employments?.FirstOrDefault(
                    m => m.InformationSource == InformationSourceType.Request.Value
                    && m.IsCreatedByAgency);

            if (_personSought.Employments.Count() > 0)
            {
                EmploymentEntity employ = _mapper.Map<EmploymentEntity>(_personSought.Employments.ElementAt(0));
                if (originalEmployment == null)
                {
                    await UploadEmployment();
                }
                else
                {
                    IDictionary<string, object> updatedFields = originalEmployment.Clone().GetUpdateEntries(employ);
                    if (updatedFields.ContainsKey("ssg_countrytext")) //country changed
                    {
                        SSG_Country country = await _searchRequestService.GetEmploymentCountry(employ.CountryText, _cancellationToken);
                        updatedFields.Add("ssg_LocationCountry", country);
                    }

                    if (updatedFields.ContainsKey("ssg_countrysubdivision_text")) //subdivision changed
                    {
                        SSG_CountrySubdivision subdivision = await _searchRequestService.GetEmploymentSubdivision(employ.CountrySubdivisionText, _cancellationToken);
                        updatedFields.Add("ssg_CountrySubDivision", subdivision);
                    }

                    if (updatedFields.Count > 0)
                    {
                        await _searchRequestService.UpdateEmployment(originalEmployment.EmploymentId, updatedFields, _cancellationToken);
                        _logger.LogInformation("Update Employment records for SearchRequest successfully");
                    }

                    Employer employer = _personSought.Employments.ElementAt(0).Employer;
                    if (employer != null && employer.Phones != null && employer.Phones.Count() > 0)
                    {
                        SSG_Employment existedEmployment = await _searchRequestService.GetEmployment(originalEmployment.EmploymentId, _cancellationToken);
                        existedEmployment.IsDuplicated = true;
                        foreach (var phone in employer.Phones)
                        {
                            EmploymentContactEntity p = _mapper.Map<EmploymentContactEntity>(phone);
                            p.Employment = existedEmployment;
                            await _searchRequestService.CreateEmploymentContact(p, _cancellationToken);
                        }
                    }
                }
            }
            return true;
        }

        //for identifier, we should not do update, as it links to the search result. 
        private async Task<bool> UpdateIdentifiers()
        {
            if (_personSought.Identifiers == null) return true;

            _logger.LogDebug($"Attempting to update identifier records for PersonSought.");

            foreach (PersonalIdentifier pi in _personSought.Identifiers.Where(m => m.Owner == OwnerType.PersonSought))
            {
                IdentifierEntity identifierEntity = _mapper.Map<IdentifierEntity>(pi);
                SSG_Identifier originalIdentifier = _uploadedPerson.SSG_Identifiers?.FirstOrDefault(
                   m => m.InformationSource == InformationSourceType.Request.Value
                        && m.IdentifierType == identifierEntity.IdentifierType
                        && m.IsCreatedByAgency);
                if (originalIdentifier == null)
                {
                    await UploadIdentifiers();
                }
                else
                {
                    identifierEntity.IsCreatedByAgency = true;
                    IDictionary<string, object> updatedFields = originalIdentifier.Clone().GetUpdateEntries(identifierEntity);
                    if (updatedFields.Count > 0)
                    {
                        await _searchRequestService.UpdateIdentifier(originalIdentifier.IdentifierId, updatedFields, _cancellationToken);
                        _logger.LogInformation("Update Identifier records for SearchRequest successfully");
                    }
                }
            }

            return true;
        }

        private async Task<bool> UploadNotes(SearchRequestEntity newSearchRequestEntity, SSG_SearchRequest existedSearchRequest)
        {
            string previousNoteStr = existedSearchRequest.Notes;
            if(existedSearchRequest.SSG_Notes!=null && existedSearchRequest.SSG_Notes?.Length > 0)
            {
                try
                {
                    previousNoteStr =
                        existedSearchRequest.SSG_Notes.Last(m => m.InformationSource == InformationSourceType.Request.Value).Description;
                }catch(Exception)
                {
                    _logger.LogInformation("notes does not contain notes from request. It is ok.");
                }
            }

            string newNotes = UpdateCurrentNote(newSearchRequestEntity.Notes, previousNoteStr);
            if (newNotes != null)
            {
                NotesEntity note = new NotesEntity
                {
                    StatusCode = 1,
                    Description = newNotes,
                    InformationSource = InformationSourceType.Request.Value,
                    SearchRequest = _uploadedSearchRequest,
                    UpdatedByApi=true
                };
                SSG_Notese ssgNote = await _searchRequestService.CreateNotes(note, _cancellationToken);

                if (ssgNote == null)
                {
                    _logger.LogError("Create new notes failed.");
                    return false;
                }
                _logger.LogInformation("Create new notes successfully.");
            }
            return true;
        }

        private async Task<bool> SubmitToQueue(Guid searchRequestId)
        {
            await _searchRequestService.SubmitToQueue(searchRequestId);
            return true;
        }

        public string UpdateCurrentNote(string currentNote, string previousNote)
        {
            if (string.IsNullOrEmpty(previousNote) || string.IsNullOrEmpty(currentNote))
                return currentNote;

            int personSoughtPos = previousNote.IndexOf($"Person Sought :");
            if (personSoughtPos > 0)
            {
                previousNote = previousNote.Substring(0, personSoughtPos).Trim();
            }
            int begin = previousNote.IndexOf("**UPDATE ");
            string cleanPreviousNote = previousNote;
            if (begin > 0)
            {
                int end = previousNote.IndexOf("**", begin+2);
                if (end > begin) {
                    cleanPreviousNote = (previousNote.Substring(0, begin) + previousNote.Substring(end+2)).Trim();
                }
            }

            
            currentNote = currentNote.Trim();
            string cleanCurrentNote = currentNote;
            int personSoughtPosCurrentNote = currentNote.IndexOf($"Person Sought :");
            if (personSoughtPosCurrentNote > 0)
            {
                cleanCurrentNote = currentNote.Substring(0, personSoughtPosCurrentNote).Trim();
            }

            string resultStr = currentNote;
            int updatePos = currentNote.IndexOf(cleanPreviousNote,StringComparison.InvariantCultureIgnoreCase);
            if ( updatePos == 0 && cleanCurrentNote.Length >= cleanPreviousNote.Length)
            {
                if (cleanCurrentNote.Length == cleanPreviousNote.Length)
                {
                    resultStr = null;
                }
                else
                {
                    string date = DateTime.Now.ToString("ddMMMyyy");
                    resultStr = currentNote.Insert(cleanPreviousNote.Length, $"**UPDATE {date}**");
                }
            }
            return resultStr;
        }

    }



}
