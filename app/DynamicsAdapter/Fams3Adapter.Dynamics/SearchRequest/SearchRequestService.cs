using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.Agency;
using Fams3Adapter.Dynamics.AssetOwner;
using Fams3Adapter.Dynamics.BankInfo;
using Fams3Adapter.Dynamics.CompensationClaim;
using Fams3Adapter.Dynamics.Duplicate;
using Fams3Adapter.Dynamics.Employment;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.InsuranceClaim;
using Fams3Adapter.Dynamics.Name;
using Fams3Adapter.Dynamics.Notes;
using Fams3Adapter.Dynamics.OtherAsset;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.PhoneNumber;
using Fams3Adapter.Dynamics.RelatedPerson;
using Fams3Adapter.Dynamics.ResultTransaction;
using Fams3Adapter.Dynamics.SafetyConcern;
using Fams3Adapter.Dynamics.Vehicle;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Entry = System.Collections.Generic.Dictionary<string, object>;

namespace Fams3Adapter.Dynamics.SearchRequest
{
    public interface ISearchRequestService
    {
        Task<SSG_Identifier> CreateIdentifier(IdentifierEntity identifier, CancellationToken cancellationToken);
        Task<SSG_Address> CreateAddress(AddressEntity address, CancellationToken cancellationToken);
        Task<SSG_PhoneNumber> CreatePhoneNumber(PhoneNumberEntity phoneNumber, CancellationToken cancellationToken);
        Task<SSG_SafetyConcernDetail> CreateSafetyConcern(SafetyConcernEntity safety, CancellationToken cancellationToken);
        Task<SSG_Aliase> CreateName(AliasEntity name, CancellationToken cancellationToken);
        Task<SSG_Identity> CreateRelatedPerson(RelatedPersonEntity name, CancellationToken cancellationToken);
        Task<SSG_Person> SavePerson(PersonEntity person, CancellationToken cancellationToken);
        Task<SSG_Employment> CreateEmployment(EmploymentEntity employment, CancellationToken cancellationToken);
        Task<SSG_EmploymentContact> CreateEmploymentContact(EmploymentContactEntity employmentContact, CancellationToken cancellationToken);
        Task<SSG_Asset_BankingInformation> CreateBankInfo(BankingInformationEntity bankInfo, CancellationToken cancellationToken);
        Task<SSG_Asset_Vehicle> CreateVehicle(VehicleEntity vehicle, CancellationToken cancellationToken);
        Task<SSG_AssetOwner> CreateAssetOwner(AssetOwnerEntity owner, CancellationToken cancellationToken);
        Task<SSG_Asset_Other> CreateOtherAsset(AssetOtherEntity asset, CancellationToken cancellationToken);
        Task<SSG_Asset_WorkSafeBcClaim> CreateCompensationClaim(CompensationClaimEntity claim, CancellationToken cancellationToken);
        Task<SSG_Asset_ICBCClaim> CreateInsuranceClaim(ICBCClaimEntity claim, CancellationToken cancellationToken);
        Task<SSG_SimplePhoneNumber> CreateSimplePhoneNumber(SimplePhoneNumberEntity phone, CancellationToken cancellationToken);
        Task<SSG_InvolvedParty> CreateInvolvedParty(InvolvedPartyEntity involvedParty, CancellationToken cancellationToken);
        Task<SSG_SearchRequestResultTransaction> CreateTransaction(SSG_SearchRequestResultTransaction transaction, CancellationToken cancellationToken);
        Task<SSG_SearchRequest> CreateSearchRequest(SearchRequestEntity searchRequest, CancellationToken cancellationToken);
        Task<SSG_SearchRequest> CancelSearchRequest(string fileId, string cancelComments, CancellationToken cancellationToken);
        Task<SSG_SearchRequest> GetSearchRequest(string fileId, CancellationToken cancellationToken);
        Task<SSG_SearchRequestReason> GetSearchReason(string reasonCode, CancellationToken cancellationToken);
        Task<SSG_AgencyLocation> GetSearchAgencyLocation(string locationCode, string agencyCode, CancellationToken cancellationToken);
        Task<SSG_Person> GetPerson(Guid personId, CancellationToken cancellationToken);
        Task<SSG_Employment> GetEmployment(Guid personId, CancellationToken cancellationToken);
        Task<SSG_Notese> CreateNotes(NotesEntity searchRequest, CancellationToken cancellationToken);
        Task<SSG_SearchRequest> UpdateSearchRequest(Guid requestId, IDictionary<string, object> updatedFields, CancellationToken cancellationToken);
        Task<SSG_Person> UpdatePerson(Guid personId, IDictionary<string, object> updatedFields, PersonEntity newPerson, CancellationToken cancellationToken);
        Task<SSG_Identity> UpdateRelatedPerson(Guid personId, IDictionary<string, object> updatedFields, CancellationToken cancellationToken);
        Task<SSG_SafetyConcernDetail> UpdateSafetyConcern(Guid safetyId, IDictionary<string, object> updatedFields, CancellationToken cancellationToken);
        Task<SSG_Employment> UpdateEmployment(Guid employmentId, IDictionary<string, object> updatedFields, CancellationToken cancellationToken);
        Task<SSG_Identifier> UpdateIdentifier(Guid identifierId, IDictionary<string, object> updatedFields, CancellationToken cancellationToken);
        Task<SSG_Country> GetEmploymentCountry(string countryText, CancellationToken cancellationToken);
        Task<SSG_CountrySubdivision> GetEmploymentSubdivision(string subDivisionText, CancellationToken cancellationToken);
        Task<bool> SubmitToQueue(Guid searchRequestId);
    }

    /// <summary>
    /// The SearchRequestService expose Search Request Functionality
    /// </summary>
    public class SearchRequestService : ISearchRequestService
    {
        private readonly IODataClient _oDataClient;
        private readonly IDuplicateDetectionService _duplicateDetectService;

        public SearchRequestService(IODataClient oDataClient, IDuplicateDetectionService duplicateDetectService)
        {
            this._oDataClient = oDataClient;
            this._duplicateDetectService = duplicateDetectService;
        }

        /// <summary>
        /// Gets all the search request in `Ready for Search` status
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<SSG_Identifier> CreateIdentifier(IdentifierEntity identifier, CancellationToken cancellationToken)
        {
            if (identifier.Person.IsDuplicated)
            {
                Guid duplicatedIdentifierId = await _duplicateDetectService.Exists(identifier.Person, identifier);
                if (duplicatedIdentifierId != Guid.Empty)
                    return new SSG_Identifier() { IdentifierId = duplicatedIdentifierId };
            }
            return await this._oDataClient.For<SSG_Identifier>().Set(identifier).InsertEntryAsync(cancellationToken);
        }

        public async Task<SSG_Person> SavePerson(PersonEntity person, CancellationToken cancellationToken)
        {
            person.DuplicateDetectHash = await _duplicateDetectService.GetDuplicateDetectHashData(person);
            string hashData = person.DuplicateDetectHash;
            var p = await this._oDataClient.For<SSG_Person>()
                    .Filter(x => x.DuplicateDetectHash == hashData)
                    .FindEntryAsync(cancellationToken);

            if (p == null)
                return await this._oDataClient.For<SSG_Person>().Set(person).InsertEntryAsync(cancellationToken);
            else
            {
                var duplicatedPerson = await _oDataClient.For<SSG_Person>()
                        .Key(p.PersonId)
                        .Expand(x => x.SSG_Addresses)
                        .Expand(x => x.SSG_Identifiers)
                        .Expand(x => x.SSG_Aliases)
                        .Expand(x => x.SSG_Asset_BankingInformations)
                        .Expand(x => x.SSG_Asset_ICBCClaims)
                        .Expand(x => x.SSG_Asset_Others)
                        .Expand(x => x.SSG_Asset_Vehicles)
                        .Expand(x => x.SSG_Asset_WorkSafeBcClaims)
                        .Expand(x => x.SSG_Employments)
                        .Expand(x => x.SSG_Identities)
                        .Expand(x => x.SSG_PhoneNumbers)
                        .Expand(x => x.SearchRequest)
                        .FindEntryAsync(cancellationToken);
                duplicatedPerson.IsDuplicated = true;
                return duplicatedPerson;
            }
        }

        public async Task<SSG_PhoneNumber> CreatePhoneNumber(PhoneNumberEntity phone, CancellationToken cancellationToken)
        {
            if (phone.Person.IsDuplicated)
            {
                Guid duplicatedPhoneId = await _duplicateDetectService.Exists(phone.Person, phone);
                if (duplicatedPhoneId != Guid.Empty)
                    return new SSG_PhoneNumber() { PhoneNumberId = duplicatedPhoneId };
            }
            return await this._oDataClient.For<SSG_PhoneNumber>().Set(phone).InsertEntryAsync(cancellationToken);
        }

        public async Task<SSG_SearchRequestResultTransaction> CreateTransaction(SSG_SearchRequestResultTransaction transaction, CancellationToken cancellationToken)
        {
            return await this._oDataClient.For<SSG_SearchRequestResultTransaction>().Set(transaction).InsertEntryAsync(cancellationToken);
        }

        public async Task<SSG_Address> CreateAddress(AddressEntity address, CancellationToken cancellationToken)
        {
            if (address.Person.IsDuplicated)
            {
                Guid duplicatedAddressId = await _duplicateDetectService.Exists(address.Person, address);
                if (duplicatedAddressId != Guid.Empty)
                    return new SSG_Address() { AddressId = duplicatedAddressId };
            }

            string countryName = address.CountryText;
            var country = await _oDataClient.For<SSG_Country>()
                                         .Filter(x => x.Name == countryName)
                                         .FindEntryAsync(cancellationToken);
            address.Country = country;

            string subdivisionName = address.CountrySubdivisionText;
            var subdivision = await _oDataClient.For<SSG_CountrySubdivision>()
                                         .Filter(x => x.Name == subdivisionName)
                                         .FindEntryAsync(cancellationToken);
            address.CountrySubdivision = subdivision;

            return await this._oDataClient.For<SSG_Address>().Set(address).InsertEntryAsync(cancellationToken);

        }

        public async Task<SSG_Aliase> CreateName(AliasEntity name, CancellationToken cancellationToken)
        {
            if (name.Person.IsDuplicated)
            {
                Guid duplicatedNameId = await _duplicateDetectService.Exists(name.Person, name);
                if (duplicatedNameId != Guid.Empty)
                    return new SSG_Aliase() { AliasId = duplicatedNameId };
            }
            return await this._oDataClient.For<SSG_Aliase>().Set(name).InsertEntryAsync(cancellationToken);
        }

        public async Task<SSG_Employment> CreateEmployment(EmploymentEntity employment, CancellationToken cancellationToken)
        {
            if (employment.Person != null && employment.Person.IsDuplicated)
            {
                Guid duplicatedEmploymentId = await _duplicateDetectService.Exists(employment.Person, employment);
                if (duplicatedEmploymentId != Guid.Empty)
                {
                    var duplicatedEmployment = await _oDataClient.For<SSG_Employment>()
                                .Key(duplicatedEmploymentId)
                                .Expand(x => x.SSG_EmploymentContacts)
                                .FindEntryAsync(cancellationToken);
                    duplicatedEmployment.IsDuplicated = true;
                    return duplicatedEmployment;
                }
            }

            EmploymentEntity linkedEmploy = await LinkEmploymentRef(employment, cancellationToken);

            return await this._oDataClient.For<SSG_Employment>().Set(linkedEmploy).InsertEntryAsync(cancellationToken);
        }

        public async Task<SSG_EmploymentContact> CreateEmploymentContact(EmploymentContactEntity employmentContact, CancellationToken cancellationToken)
        {
            if (employmentContact.Employment != null && employmentContact.Employment.IsDuplicated)
            {
                Guid duplicatedContactId = await _duplicateDetectService.Exists(employmentContact.Employment, employmentContact);
                if (duplicatedContactId != Guid.Empty)
                    return new SSG_EmploymentContact() { EmploymentContactId = duplicatedContactId };
            }

            return await this._oDataClient.For<SSG_EmploymentContact>().Set(employmentContact).InsertEntryAsync(cancellationToken);
        }

        public async Task<SSG_Identity> CreateRelatedPerson(RelatedPersonEntity relatedPerson, CancellationToken cancellationToken)
        {
            if (relatedPerson.Person.IsDuplicated)
            {
                Guid duplicatedRelatedPersonId = await _duplicateDetectService.Exists(relatedPerson.Person, relatedPerson);
                if (duplicatedRelatedPersonId != Guid.Empty)
                    return new SSG_Identity() { RelatedPersonId = duplicatedRelatedPersonId };
            }
            return await this._oDataClient.For<SSG_Identity>().Set(relatedPerson).InsertEntryAsync(cancellationToken);
        }

        public async Task<SSG_Asset_BankingInformation> CreateBankInfo(BankingInformationEntity bankInfo, CancellationToken cancellationToken)
        {
            if (bankInfo.Person != null && bankInfo.Person.IsDuplicated)
            {
                Guid duplicatedBankInfoId = await _duplicateDetectService.Exists(bankInfo.Person, bankInfo);
                if (duplicatedBankInfoId != Guid.Empty)
                    return new SSG_Asset_BankingInformation() { BankingInformationId = duplicatedBankInfoId };
            }
            return await this._oDataClient.For<SSG_Asset_BankingInformation>().Set(bankInfo).InsertEntryAsync(cancellationToken);
        }

        public async Task<SSG_Asset_Vehicle> CreateVehicle(VehicleEntity vehicle, CancellationToken cancellationToken)
        {
            if (vehicle.Person.IsDuplicated)
            {
                Guid duplicatedVehicleId = await _duplicateDetectService.Exists(vehicle.Person, vehicle);
                if (duplicatedVehicleId != Guid.Empty)
                {
                    var duplicatedVehicle = await _oDataClient.For<SSG_Asset_Vehicle>()
                                .Key(duplicatedVehicleId)
                                .Expand(x => x.SSG_AssetOwners)
                                .FindEntryAsync(cancellationToken);
                    duplicatedVehicle.IsDuplicated = true;
                    return duplicatedVehicle;
                }
            }
            return await this._oDataClient.For<SSG_Asset_Vehicle>().Set(vehicle).InsertEntryAsync(cancellationToken);
        }

        public async Task<SSG_AssetOwner> CreateAssetOwner(AssetOwnerEntity owner, CancellationToken cancellationToken)
        {
            if (owner.Vehicle != null && owner.Vehicle.IsDuplicated)
            {
                Guid duplicatedOwnerId = await _duplicateDetectService.Exists(owner.Vehicle, owner);
                if (duplicatedOwnerId != Guid.Empty)
                    return new SSG_AssetOwner() { AssetOwnerId = duplicatedOwnerId };
            }
            return await this._oDataClient.For<SSG_AssetOwner>().Set(owner).InsertEntryAsync(cancellationToken);
        }

        public async Task<SSG_Asset_Other> CreateOtherAsset(AssetOtherEntity otherAsset, CancellationToken cancellationToken)
        {
            if (otherAsset.Person.IsDuplicated)
            {
                Guid duplicatedOtherAssetId = await _duplicateDetectService.Exists(otherAsset.Person, otherAsset);
                if (duplicatedOtherAssetId != Guid.Empty)
                    return new SSG_Asset_Other() { AssetOtherId = duplicatedOtherAssetId };
            }
            return await this._oDataClient.For<SSG_Asset_Other>().Set(otherAsset).InsertEntryAsync(cancellationToken);
        }

        private async Task<SSG_Asset_WorkSafeBcClaim> GetDuplicatedCompensation(CompensationClaimEntity claim, CancellationToken cancellationToken)
        {
            if (claim.Person != null && claim.Person.IsDuplicated)
            {
                Guid duplicatedCompensationId = await _duplicateDetectService.Exists(claim.Person, claim);
                if (duplicatedCompensationId != Guid.Empty)
                {
                    var duplicatedClaim = await _oDataClient.For<SSG_Asset_WorkSafeBcClaim>()
                                .Key(duplicatedCompensationId)
                                .Expand(x => x.BankingInformation)
                                .Expand(x => x.Employment)
                                .FindEntryAsync(cancellationToken);
                    if (await _duplicateDetectService.Same(claim.BankInformationEntity, duplicatedClaim.BankingInformation))
                    {
                        if (await _duplicateDetectService.Same(claim.EmploymentEntity, duplicatedClaim.Employment))
                        {
                            duplicatedClaim.IsDuplicated = true;
                            if (duplicatedClaim.Employment != null)
                            {
                                Guid duplicatedEmploymentId = duplicatedClaim.Employment.EmploymentId;
                                duplicatedClaim.Employment = await _oDataClient.For<SSG_Employment>()
                                    .Key(duplicatedEmploymentId)
                                    .Expand(x => x.SSG_EmploymentContacts)
                                    .FindEntryAsync(cancellationToken);
                                duplicatedClaim.Employment.IsDuplicated = true;
                            }
                            return duplicatedClaim;
                        }
                    }
                }
            }
            return null;
        }

        public async Task<SSG_Asset_WorkSafeBcClaim> CreateCompensationClaim(CompensationClaimEntity claim, CancellationToken cancellationToken)
        {
            SSG_Asset_WorkSafeBcClaim returnedClaim = null;
            SSG_Asset_WorkSafeBcClaim duplicatedClaim = await GetDuplicatedCompensation(claim, cancellationToken);

            SSG_Employment ssg_employment = null;
            if (duplicatedClaim != null && duplicatedClaim.IsDuplicated)
            {
                ssg_employment = duplicatedClaim.Employment;
                returnedClaim = duplicatedClaim;
            }
            else
            {
                SSG_Asset_BankingInformation ssg_bank = claim.BankInformationEntity == null ? null : await CreateBankInfo(claim.BankInformationEntity, cancellationToken);
                ssg_employment = claim.EmploymentEntity == null ? null : await CreateEmployment(claim.EmploymentEntity, cancellationToken);
                claim.BankingInformation = ssg_bank;
                claim.Employment = ssg_employment;
                SSG_Asset_WorkSafeBcClaim ssg_Claim = await this._oDataClient.For<SSG_Asset_WorkSafeBcClaim>().Set(claim).InsertEntryAsync(cancellationToken);
                returnedClaim = ssg_Claim;
            }

            if (claim.EmploymentEntity != null && claim.EmploymentEntity.EmploymentContactEntities != null)
            {
                foreach (EmploymentContactEntity contact in claim.EmploymentEntity.EmploymentContactEntities)
                {
                    contact.Employment = ssg_employment;
                    await CreateEmploymentContact(contact, cancellationToken);
                }
            }
            return returnedClaim;
        }

        public async Task<SSG_Asset_ICBCClaim> CreateInsuranceClaim(ICBCClaimEntity insurance, CancellationToken cancellationToken)
        {
            if (insurance.Person != null && insurance.Person.IsDuplicated)
            {
                Guid duplicatedInsuranceId = await _duplicateDetectService.Exists(insurance.Person, insurance);
                if (duplicatedInsuranceId != Guid.Empty)
                {
                    var duplicatedInsurance = await _oDataClient.For<SSG_Asset_ICBCClaim>()
                                .Key(duplicatedInsuranceId)
                                .Expand(x => x.SSG_InvolvedParties)
                                .Expand(x => x.SSG_SimplePhoneNumbers)
                                .FindEntryAsync(cancellationToken);
                    duplicatedInsurance.IsDuplicated = true;
                    return duplicatedInsurance;
                }
            }
            return await this._oDataClient.For<SSG_Asset_ICBCClaim>().Set(insurance).InsertEntryAsync(cancellationToken);
        }

        public async Task<SSG_SimplePhoneNumber> CreateSimplePhoneNumber(SimplePhoneNumberEntity phone, CancellationToken cancellationToken)
        {
            if (phone.SSG_Asset_ICBCClaim != null && phone.SSG_Asset_ICBCClaim.IsDuplicated)
            {
                Guid duplicatedPhoneId = await _duplicateDetectService.Exists(phone.SSG_Asset_ICBCClaim, phone);
                if (duplicatedPhoneId != Guid.Empty)
                    return new SSG_SimplePhoneNumber() { SimplePhoneNumberId = duplicatedPhoneId };
            }
            return await this._oDataClient.For<SSG_SimplePhoneNumber>().Set(phone).InsertEntryAsync(cancellationToken);
        }

        public async Task<SSG_InvolvedParty> CreateInvolvedParty(InvolvedPartyEntity involvedParty, CancellationToken cancellationToken)
        {
            if (involvedParty.SSG_Asset_ICBCClaim != null && involvedParty.SSG_Asset_ICBCClaim.IsDuplicated)
            {
                Guid duplicatedPartyId = await _duplicateDetectService.Exists(involvedParty.SSG_Asset_ICBCClaim, involvedParty);
                if (duplicatedPartyId != Guid.Empty)
                    return new SSG_InvolvedParty() { InvolvedPartyId = duplicatedPartyId };
            }
            return await this._oDataClient.For<SSG_InvolvedParty>().Set(involvedParty).InsertEntryAsync(cancellationToken);
        }

        public async Task<SSG_SearchRequest> CreateSearchRequest(SearchRequestEntity searchRequest, CancellationToken cancellationToken)
        {
            SearchRequestEntity linkedSearchRequest = await LinkSearchRequestRef(searchRequest, cancellationToken);
            return await this._oDataClient.For<SSG_SearchRequest>().Set(linkedSearchRequest).InsertEntryAsync(cancellationToken);
        }

        public async Task<SSG_SafetyConcernDetail> CreateSafetyConcern(SafetyConcernEntity safety, CancellationToken cancellationToken)
        {
            return await this._oDataClient.For<SSG_SafetyConcernDetail>().Set(safety).InsertEntryAsync(cancellationToken);
        }


        public async Task<SSG_SearchRequest> CancelSearchRequest(string fileId, string cancelComments, CancellationToken cancellationToken)
        {
            SSG_SearchRequest searchRequest = await _oDataClient.For<SSG_SearchRequest>().Filter(x => x.FileId == fileId).FindEntryAsync(cancellationToken);

            if (searchRequest == null) return null;

            return await _oDataClient
                        .For<SSG_SearchRequest>()
                        .Key(searchRequest.SearchRequestId)
                        .Set(new Entry {
                            { Keys.DYNAMICS_STATUS_CODE_FIELD, SearchRequestStatusCode.AgencyCancelled.Value },
                            { Keys.DYNAMICS_SEARCH_REQUEST_CANCEL_COMMENTS_FIELD, cancelComments }
                        })
                        .UpdateEntryAsync(cancellationToken);
        }

        public async Task<SSG_SearchRequest> GetSearchRequest(string fileId, CancellationToken cancellationToken)
        {
            SSG_SearchRequest ssgSearchRequest = await _oDataClient
                .For<SSG_SearchRequest>()
                .Select(x => x.SearchRequestId)
                .Filter(x => x.FileId == fileId)
                .FindEntryAsync(cancellationToken);
            if (ssgSearchRequest == null) return null;

            Guid key = ssgSearchRequest.SearchRequestId;
            SSG_SearchRequest dataSearchRequest = await _oDataClient
                .For<SSG_SearchRequest>()
                .Key(key)
                .Expand(x => x.Agency)
                .Expand(x => x.SearchReason)
                .Expand(x => x.AgencyLocation)
                .Expand(x => x.SSG_Persons)
                .Expand(x => x.SSG_Notes)
                .FindEntryAsync(cancellationToken);

            dataSearchRequest.AgencyCode = dataSearchRequest.Agency?.AgencyCode;
            dataSearchRequest.AgencyOfficeLocationText = dataSearchRequest.AgencyLocation?.LocationCode;
            dataSearchRequest.SearchReasonCode = dataSearchRequest.SearchReason?.ReasonCode;
            return dataSearchRequest;
        }

        public async Task<SSG_Person> GetPerson(Guid personId, CancellationToken cancellationToken)
        {
            SSG_Person person = await _oDataClient
                .For<SSG_Person>()
                .Key(personId)
                .Expand(x => x.SSG_Identities)
                .Expand(x => x.SSG_PhoneNumbers)
                .Expand(x => x.SSG_Identifiers)
                .Expand(x => x.SSG_Employments)
                .Expand(x => x.SSG_Addresses)
                .Expand(x => x.SSG_Aliases)
                .Expand(x => x.sSG_SafetyConcernDetails)
                .FindEntryAsync(cancellationToken);

            return person;
        }

        public async Task<SSG_Employment> GetEmployment(Guid employmentId, CancellationToken cancellationToken)
        {
            return await this._oDataClient.For<SSG_Employment>().Key(employmentId).Expand(x => x.SSG_EmploymentContacts).FindEntryAsync(cancellationToken);
        }

        public async Task<SSG_SearchRequest> UpdateSearchRequest(Guid requestId, IDictionary<string, object> updatedFields, CancellationToken cancellationToken)
        {
            updatedFields.Add(new KeyValuePair<string, object>("ssg_updatedbyagency", true));
            return await this._oDataClient.For<SSG_SearchRequest>().Key(requestId).Set(updatedFields).UpdateEntryAsync(cancellationToken);
        }

        public async Task<SSG_Person> UpdatePerson(Guid personId, IDictionary<string, object> updatedFields, PersonEntity newPerson, CancellationToken cancellationToken)
        {
            string duplicateDetectHash = await _duplicateDetectService.GetDuplicateDetectHashData(newPerson);
            updatedFields.Add("ssg_duplicatedetectionhash", duplicateDetectHash);
            return await this._oDataClient.For<SSG_Person>().Key(personId).Set(updatedFields).UpdateEntryAsync(cancellationToken);
        }

        public async Task<SSG_Identity> UpdateRelatedPerson(Guid relatedPersonId, IDictionary<string, object> updatedFields, CancellationToken cancellationToken)
        {
            return await this._oDataClient.For<SSG_Identity>().Key(relatedPersonId).Set(updatedFields).UpdateEntryAsync(cancellationToken);
        }

        public async Task<SSG_Employment> UpdateEmployment(Guid employmentId, IDictionary<string, object> updatedFields, CancellationToken cancellationToken)
        {
            //EmploymentEntity linkedEmploy = await LinkEmploymentRef(newEmploy, cancellationToken);
            return await this._oDataClient.For<SSG_Employment>().Key(employmentId).Set(updatedFields).UpdateEntryAsync(cancellationToken);
        }

        public async Task<SSG_Identifier> UpdateIdentifier(Guid identifierId, IDictionary<string, object> updatedFields, CancellationToken cancellationToken)
        {
            return await this._oDataClient.For<SSG_Identifier>().Key(identifierId).Set(updatedFields).UpdateEntryAsync(cancellationToken);
        }

        public async Task<SSG_SafetyConcernDetail> UpdateSafetyConcern(Guid safetyId, IDictionary<string, object> updatedFields, CancellationToken cancellationToken)
        {
            return await this._oDataClient.For<SSG_SafetyConcernDetail>().Key(safetyId).Set(updatedFields).UpdateEntryAsync(cancellationToken);
        }

        public async Task<SSG_Notese> CreateNotes(NotesEntity note, CancellationToken cancellationToken)
        {
            if (note.SearchRequest != null && note.SearchRequest.IsDuplicated)
            {
                Guid duplicatedNoteId = await _duplicateDetectService.Exists(note.SearchRequest, note);
                if (duplicatedNoteId != Guid.Empty)
                    return new SSG_Notese() { NotesId = duplicatedNoteId };
            }
            return await this._oDataClient.For<SSG_Notese>().Set(note).InsertEntryAsync(cancellationToken);
        }

        private async Task<SearchRequestEntity> LinkSearchRequestRef(SearchRequestEntity searchRequest, CancellationToken cancellationToken)
        {
            //find agencyCode in ssg_agency entity
            string code = searchRequest.AgencyCode;
            var agency = await _oDataClient.For<SSG_Agency>()
                                         .Filter(x => x.AgencyCode == code)
                                         .FindEntryAsync(cancellationToken);
            searchRequest.Agency = agency;

            //find reasoncode 
            var reason = await GetSearchReason(searchRequest.SearchReasonCode, cancellationToken);
            searchRequest.SearchReason = reason;

            var officeLocation = await GetSearchAgencyLocation(searchRequest.AgencyOfficeLocationText, code, cancellationToken);
            searchRequest.AgencyLocation = officeLocation;
            if (officeLocation != null)
                searchRequest.AgencyOfficeLocationText = null;
            return searchRequest;
        }

        public async Task<SSG_SearchRequestReason> GetSearchReason(string reasonCode, CancellationToken cancellationToken)
        {
            //find reasoncode 
            var reason = await _oDataClient.For<SSG_SearchRequestReason>()
                             .Filter(x => x.ReasonCode == reasonCode)
                             .FindEntryAsync(cancellationToken);
            return reason;
        }

        public async Task<SSG_AgencyLocation> GetSearchAgencyLocation(string locationCode, string agencyCode, CancellationToken cancellationToken)
        {
            try
            {
                SSG_AgencyLocation officeLocation = null;
                if (locationCode.Length > 5)
                {
                    //temp, as the sample files is not changed to code yet. so, if it is not code, run following code.
                    //We just currently use City as temp solution. expecting FMEP will provide office code later
                    string officeLocationCity = locationCode.Split(",")[1].Trim();
                    officeLocation = await _oDataClient.For<SSG_AgencyLocation>()
                         .Filter(x => x.City == officeLocationCity)
                         .FindEntryAsync(cancellationToken);
                }
                else
                {
                    //it is code, like B,R,C,K
                    officeLocation = await _oDataClient.For<SSG_AgencyLocation>()
                         .Filter(x => x.AgencyCode == agencyCode && x.LocationCode == locationCode)
                         .FindEntryAsync(cancellationToken);
                }

                return officeLocation;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<SSG_Country> GetEmploymentCountry(string countryText, CancellationToken cancellationToken)
        {
            var country = await _oDataClient.For<SSG_Country>()
                                .Filter(x => x.Name == countryText)
                                .FindEntryAsync(cancellationToken);
            return country;
        }

        public async Task<SSG_CountrySubdivision> GetEmploymentSubdivision(string subDivisionText, CancellationToken cancellationToken)
        {
            var subdivision = await _oDataClient.For<SSG_CountrySubdivision>()
                                      .Filter(x => x.Name == subDivisionText)
                                      .FindEntryAsync(cancellationToken);
            return subdivision;
        }

        public async Task<bool> SubmitToQueue(Guid searchRequestId)
        {
            await _oDataClient.For<SSG_SearchRequest>().Key(searchRequestId).Action("ssg_SearchRequestSubmittoQueueActions").ExecuteAsSingleAsync();
            return true;
        }

        private async Task<EmploymentEntity> LinkEmploymentRef(EmploymentEntity employment, CancellationToken cancellationToken)
        {
            employment.Country = await GetEmploymentCountry(employment.CountryText, cancellationToken);
            employment.CountrySubdivision = await GetEmploymentSubdivision(employment.CountrySubdivisionText, cancellationToken);
            return employment;
        }
    }
}