using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.TaxIncomeInformation;
using Fams3Adapter.Dynamics.FinancialOtherIncome;
using Fams3Adapter.Dynamics.Agency;
using Fams3Adapter.Dynamics.APICall;
using Fams3Adapter.Dynamics.AssetOwner;
using Fams3Adapter.Dynamics.BankInfo;
using Fams3Adapter.Dynamics.CompensationClaim;
using Fams3Adapter.Dynamics.Duplicate;
using Fams3Adapter.Dynamics.Email;
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
using Microsoft.Extensions.Logging;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Entry = System.Collections.Generic.Dictionary<string, object>;

namespace Fams3Adapter.Dynamics.SearchRequest
{
    public interface ISearchRequestService
    {
        Task<SSG_Identifier> CreateIdentifier(IdentifierEntity identifier, CancellationToken cancellationToken);
        Task<SSG_Address> CreateAddress(AddressEntity address, CancellationToken cancellationToken);
        Task<SSG_TaxIncomeInformation> CreateTaxIncomeInformation(TaxIncomeInformationEntity taxinfo, CancellationToken cancellationToken);
        Task<FAMS_FinancialOtherIncome> CreateFinancialOtherIncome(FinancialOtherIncomeEntity finOtherIncome, CancellationToken cancellationToken);
        Task<SSG_PhoneNumber> CreatePhoneNumber(PhoneNumberEntity phoneNumber, CancellationToken cancellationToken);
        Task<SSG_Email> CreateEmail(EmailEntity email, CancellationToken cancellationToken);
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
        Task<SSG_SearchRequest> SystemCancelSearchRequest(SSG_SearchRequest searchRequest, CancellationToken cancellationToken);
        Task<SSG_SearchRequest> GetSearchRequest(string fileId, CancellationToken cancellationToken);
        Task<SSG_SearchRequestReason> GetSearchReason(string reasonCode, CancellationToken cancellationToken);
        Task<SSG_AgencyLocation> GetSearchAgencyLocation(string locationCode, string agencyCode, CancellationToken cancellationToken);
        Task<SSG_Person> GetPerson(Guid personId, CancellationToken cancellationToken);
        Task<SSG_Employment> GetEmployment(Guid personId, CancellationToken cancellationToken);
        Task<IEnumerable<FAMS_TaxCode>> GetTaxCodes(CancellationToken cancellationToken);
        Task<SSG_Notese> CreateNotes(NotesEntity searchRequest, CancellationToken cancellationToken);
        Task<SSG_SearchRequest> UpdateSearchRequest(Guid requestId, IDictionary<string, object> updatedFields, CancellationToken cancellationToken);
        //Task<SSG_Person> UpdatePerson(Guid personId, IDictionary<string, object> updatedFields, PersonEntity newPerson, CancellationToken cancellationToken);
        Task<SSG_Person> UpdatePerson(SSG_Person existedPerson, IDictionary<string, object> updatedFields, PersonEntity newPerson, CancellationToken cancellationToken);
        Task<SSG_Identity> UpdateRelatedPerson(Guid personId, IDictionary<string, object> updatedFields, CancellationToken cancellationToken);
        Task<SSG_SafetyConcernDetail> UpdateSafetyConcern(Guid safetyId, IDictionary<string, object> updatedFields, CancellationToken cancellationToken);
        Task<SSG_Employment> UpdateEmployment(Guid employmentId, IDictionary<string, object> updatedFields, CancellationToken cancellationToken);
        Task<SSG_Identifier> UpdateIdentifier(Guid identifierId, IDictionary<string, object> updatedFields, CancellationToken cancellationToken);
        Task<SSG_Country> GetEmploymentCountry(string countryText, CancellationToken cancellationToken);
        Task<SSG_CountrySubdivision> GetEmploymentSubdivision(string subDivisionText, CancellationToken cancellationToken);
        Task<bool> SubmitToQueue(Guid searchRequestId);
        Task<bool> DeleteSearchRequest(string fileId, CancellationToken cancellationToken);
        Task<bool> UpdateApiCall(Guid apiCallGuid, bool success, string notes, CancellationToken cancellationToken);
        Task<SSG_SearchRequest> GetCurrentSearchRequest(Guid searchRequestId);
        Task<IEnumerable<SSG_SearchRequest>> GetAutoCloseSearchRequestAsync(CancellationToken cancellationToken);
        Task<bool> SearchRequestCreateCouldNotAutoCloseNote(Guid searchRequestId);
        Task<SSG_SearchRequest> UpdateSearchRequestStatusAutoClosed(Guid searchRequestId, CancellationToken cancellationToken);
    }

    /// <summary>
    /// The SearchRequestService expose Search Request Functionality
    /// </summary>
    public class SearchRequestService : ISearchRequestService
    {
        private readonly IODataClient _oDataClient;
        private readonly IDuplicateDetectionService _duplicateDetectService;
        private readonly ILogger<SearchRequestService> _logger;

        public SearchRequestService(IODataClient oDataClient, IDuplicateDetectionService duplicateDetectService, ILogger<SearchRequestService> logger)
        {
            this._oDataClient = oDataClient;
            this._duplicateDetectService = duplicateDetectService;
            this._logger = logger;
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

            SSG_Identifier insertedIdentRecord = null;
            try
            {
                insertedIdentRecord = await this._oDataClient
                    .For<SSG_Identifier>()
                    .Set(identifier)
                    .InsertEntryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw; // rethrow so caller can react
            }

            _logger.LogDebug("Successfully inserted Identifier with Id={IdentifierId} for PersonId={PersonId}",
                insertedIdentRecord.IdentifierId,
                identifier.Person.PersonId);

            return insertedIdentRecord;
        }

        public async Task<SSG_Person> SavePerson(PersonEntity person, CancellationToken cancellationToken)
        {
            //ssg_duplicatedetectionhash is set to be Unique in DB, so we can use this to detect duplicates.
            person.DuplicateDetectHash = await _duplicateDetectService.GetDuplicateDetectHashData(person);

            try
            {
                return await this._oDataClient
                    .For<SSG_Person>()
                    .Set(person)
                    .InsertEntryAsync(cancellationToken);
            }
            catch (WebRequestException e) when (e.IsDuplicateHashError())
            {
                string hashData = person.DuplicateDetectHash;
                var existedPerson = await this._oDataClient.For<SSG_Person>()
                    .Filter(x => x.DuplicateDetectHash == hashData)
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
                    .Expand(x => x.SSG_SafetyConcernDetails)
                    .Expand(x => x.SSG_Emails)
                    .Expand(x => x.SearchRequest)
                    .Expand(x => x.SSG_TaxIncomeInformations)
                    .FindEntryAsync(CancellationToken.None);

                existedPerson.IsDuplicated = true;
                return existedPerson;
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
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

            SSG_PhoneNumber insertedPhNumRecord = null;
            try
            {
                insertedPhNumRecord = await this._oDataClient
                    .For<SSG_PhoneNumber>()
                    .Set(phone)
                    .InsertEntryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }

            _logger.LogDebug("Successfully inserted PhoneNumber with Id={PhoneId} for PersonId={PersonId}",
                insertedPhNumRecord.PhoneNumberId,
                phone.Person.PersonId);

            return insertedPhNumRecord;
        }

        public async Task<SSG_Email> CreateEmail(EmailEntity email, CancellationToken cancellationToken)
        {
            if (email.Person.IsDuplicated)
            {
                Guid duplicatedEmailId = await _duplicateDetectService.Exists(email.Person, email);
                if (duplicatedEmailId != Guid.Empty)
                    return new SSG_Email() { EmailId = duplicatedEmailId };
            }

            SSG_Email insertedEmailRecord = null;
            try
            {
                insertedEmailRecord = await this._oDataClient
                    .For<SSG_Email>()
                    .Set(email)
                    .InsertEntryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }

            _logger.LogDebug("Successfully inserted Email with Id={EmailId} for PersonId={PersonId}",
                insertedEmailRecord.EmailId,
                email.Person.PersonId);

            return insertedEmailRecord;
        }

        public async Task<SSG_SearchRequestResultTransaction> CreateTransaction(
            SSG_SearchRequestResultTransaction transaction,
            CancellationToken cancellationToken)
        {
            SSG_SearchRequestResultTransaction txResult = null;

            try
            {
                txResult = await this._oDataClient
                    .For<SSG_SearchRequestResultTransaction>()
                    .Set(transaction)
                    .InsertEntryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }

            return txResult;
        }

        public async Task<SSG_Address> CreateAddress(AddressEntity address, CancellationToken cancellationToken)
        {
            if (address.Person.IsDuplicated)
            {
                Guid duplicatedAddressId = await _duplicateDetectService.Exists(address.Person, address);
                if (duplicatedAddressId != Guid.Empty)
                {
                    _logger.LogDebug(
                        "Duplicate address detected. Using existing AddressId={DuplicatedId} for PersonId={PersonId}",
                        duplicatedAddressId,
                        address.Person.PersonId);

                    return new SSG_Address() { AddressId = duplicatedAddressId };
                }
            }

            string countryName = address.CountryText;
            var country = await _oDataClient.For<SSG_Country>()
                                    .Filter(x => x.Name == countryName)
                                    .FindEntryAsync(cancellationToken);
            if (country == null)
            {
                _logger.LogDebug(
                    "Country lookup failed for '{CountryName}'. Address insert may fail validation.",
                    countryName);
            }
            address.Country = country;

            string subdivisionName = address.CountrySubdivisionText;
            var subdivision = await _oDataClient.For<SSG_CountrySubdivision>()
                                        .Filter(x => x.Name == subdivisionName)
                                        .FindEntryAsync(cancellationToken);
            if (subdivision == null)
            {
                _logger.LogDebug(
                    "Subdivision lookup failed for '{SubdivisionName}'. Address insert may fail validation.",
                    subdivisionName);
            }
            address.CountrySubdivision = subdivision;

            SSG_Address insertedAddress = null;
            try
            {
                insertedAddress = await _oDataClient
                    .For<SSG_Address>()
                    .Set(address)
                    .InsertEntryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }

            _logger.LogDebug(
                "Successfully inserted Address with Id={InsertedId} for PersonId={PersonId}",
                insertedAddress.AddressId,
                address.Person.PersonId);

            return insertedAddress;
        }

        public async Task<SSG_TaxIncomeInformation> CreateTaxIncomeInformation(TaxIncomeInformationEntity taxinfo, CancellationToken cancellationToken)
        {
            if (taxinfo.Person.IsDuplicated)
            {
                Guid duplicatedTaxInfoId = await _duplicateDetectService.Exists(taxinfo.Person, taxinfo);
                if (duplicatedTaxInfoId != Guid.Empty)
                {
                    _logger.LogDebug(
                        "Duplicate TaxIncomeInformation detected. Using existing record with Id={DuplicatedId} for PersonId={PersonId}",
                        duplicatedTaxInfoId,
                        taxinfo.Person.PersonId);

                    return new SSG_TaxIncomeInformation() { TaxincomeinformationId = duplicatedTaxInfoId };
                }
            }

            SSG_TaxIncomeInformation insertedRecord = null;
            try
            {
                insertedRecord = await this._oDataClient
                    .For<SSG_TaxIncomeInformation>()
                    .Set(taxinfo)
                    .InsertEntryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw; // allow caller to see failure too
            }

            _logger.LogDebug("Successfully inserted TaxIncomeInformation with Id={InsertedId} for PersonId={PersonId}",
                insertedRecord.TaxincomeinformationId,
                taxinfo.Person.PersonId);

            return insertedRecord;
        }

        public async Task<FAMS_FinancialOtherIncome> CreateFinancialOtherIncome(FinancialOtherIncomeEntity finOtherIncome, CancellationToken cancellationToken)
        {
            // Validate Person reference
            if (finOtherIncome.Person == null || finOtherIncome.Person.PersonId == Guid.Empty)
            {
                throw new ArgumentException("FinancialOtherIncome must have a valid Person reference before insertion.");
            }

            // Validate SearchRequest reference
            if (finOtherIncome.SearchRequest == null || finOtherIncome.SearchRequest.SearchRequestId == Guid.Empty)
            {
                throw new ArgumentException("FinancialOtherIncome must have a valid SearchRequest reference before insertion.");
            }

            // Check for duplicates
            if (finOtherIncome.Person.IsDuplicated)
            {
                Guid duplicatedFinOtherIncomeId = await _duplicateDetectService.Exists(finOtherIncome.Person, finOtherIncome);
                if (duplicatedFinOtherIncomeId != Guid.Empty)
                {
                    _logger.LogDebug(
                        "Duplicate FinancialOtherIncomeId detected. Using existing record with Id={DuplicatedId} for PersonId={PersonId}",
                        duplicatedFinOtherIncomeId,
                        finOtherIncome.Person.PersonId);

                    return new FAMS_FinancialOtherIncome() { FinancialOtherIncomeId = duplicatedFinOtherIncomeId };
                }
            }

            FAMS_FinancialOtherIncome insertedRecord = null;
            try
            {
                _logger.LogDebug(
                    "Preparing to insert FinancialOtherIncome for PersonId={PersonId}, " +
                    "Person.FirstName='{FirstName}', Person.LastName='{LastName}', Person.DOB={DOB}, " +
                    "SearchRequestId={SearchRequestId}",
                    finOtherIncome.Person?.PersonId,
                    finOtherIncome.Person?.FirstName ?? "null",
                    finOtherIncome.Person?.LastName ?? "null",
                    finOtherIncome.Person?.DateOfBirth?.ToString("yyyy-MM-dd") ?? "null",
                    finOtherIncome.SearchRequest?.SearchRequestId);
                    
                insertedRecord = await this._oDataClient
                    .For<FAMS_FinancialOtherIncome>()
                    .Set(finOtherIncome)
                    .InsertEntryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw; // allow caller to see failure too
            }

            _logger.LogDebug("Successfully inserted FinancialOtherIncome with Id={InsertedId} for PersonId={PersonId}",
                insertedRecord.FinancialOtherIncomeId,
                finOtherIncome.Person.PersonId);

            return insertedRecord;
        }

        private void LogDynamicsError(Exception ex)
        {
            _logger.LogError(ex, "❌ Dynamics insert failed");

            var error = ex.InnerException;
            while (error != null)
            {
                _logger.LogError("⛔ InnerException: {Message}", error.Message);
                error = error.InnerException;
            }

            // Attempt to extract OData error JSON if present
            if (ex is WebRequestException webEx && webEx.Response != null)
            {
                try
                {
                    var body = webEx.Response;
                    _logger.LogError("📝 OData Response Content:\n{Response}", body);
                }
                catch { }
            }
        }

        private IEnumerable<FAMS_TaxCode> _taxCodes { get; set; }
        public async Task<IEnumerable<FAMS_TaxCode>> GetTaxCodes(CancellationToken cancellationToken)
        {
            if (_taxCodes == null)
                _taxCodes = await _oDataClient.For<FAMS_TaxCode>().FindEntriesAsync(cancellationToken);
            return _taxCodes;
        }

        public async Task<SSG_Aliase> CreateName(AliasEntity name, CancellationToken cancellationToken)
        {
            if (name.Person.IsDuplicated)
            {
                Guid duplicatedNameId = await _duplicateDetectService.Exists(name.Person, name);
                if (duplicatedNameId != Guid.Empty)
                    return new SSG_Aliase() { AliasId = duplicatedNameId };
            }

            SSG_Aliase insertedAliaseRecord = null;
            try
            {
                insertedAliaseRecord = await this._oDataClient
                    .For<SSG_Aliase>()
                    .Set(name)
                    .InsertEntryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }

            return insertedAliaseRecord;
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

            SSG_Employment insertedEmploymentRecord = null;
            try
            {
                insertedEmploymentRecord = await this._oDataClient
                    .For<SSG_Employment>()
                    .Set(linkedEmploy)
                    .InsertEntryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }

            return insertedEmploymentRecord;
        }

        public async Task<SSG_EmploymentContact> CreateEmploymentContact(EmploymentContactEntity employmentContact, CancellationToken cancellationToken)
        {
            if (employmentContact.Employment != null && employmentContact.Employment.IsDuplicated)
            {
                Guid duplicatedContactId = await _duplicateDetectService.Exists(employmentContact.Employment, employmentContact);
                if (duplicatedContactId != Guid.Empty)
                    return new SSG_EmploymentContact() { EmploymentContactId = duplicatedContactId };
            }

            SSG_EmploymentContact insertedEmplContactRecord = null;
            try
            {
                insertedEmplContactRecord = await this._oDataClient
                    .For<SSG_EmploymentContact>()
                    .Set(employmentContact)
                    .InsertEntryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }

            return insertedEmplContactRecord;
        }

        public async Task<SSG_Identity> CreateRelatedPerson(RelatedPersonEntity relatedPerson, CancellationToken cancellationToken)
        {
            if (relatedPerson.Person.IsDuplicated)
            {
                Guid duplicatedRelatedPersonId = await _duplicateDetectService.Exists(relatedPerson.Person, relatedPerson);
                if (duplicatedRelatedPersonId != Guid.Empty)
                    return new SSG_Identity() { RelatedPersonId = duplicatedRelatedPersonId };
            }

            SSG_Identity insertedRelatedPersonRecord = null;
            try
            {
                insertedRelatedPersonRecord = await this._oDataClient
                    .For<SSG_Identity>()
                    .Set(relatedPerson)
                    .InsertEntryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }

            return insertedRelatedPersonRecord;
        }

        public async Task<SSG_Asset_BankingInformation> CreateBankInfo(BankingInformationEntity bankInfo, CancellationToken cancellationToken)
        {
            if (bankInfo.Person != null && bankInfo.Person.IsDuplicated)
            {
                Guid duplicatedBankInfoId = await _duplicateDetectService.Exists(bankInfo.Person, bankInfo);
                if (duplicatedBankInfoId != Guid.Empty)
                    return new SSG_Asset_BankingInformation() { BankingInformationId = duplicatedBankInfoId };
            }

            SSG_Asset_BankingInformation insertedBankInfoRecord = null;
            try
            {
                insertedBankInfoRecord = await this._oDataClient
                    .For<SSG_Asset_BankingInformation>()
                    .Set(bankInfo)
                    .InsertEntryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }

            return insertedBankInfoRecord;
        }

        public async Task<SSG_Asset_Vehicle> CreateVehicle(VehicleEntity vehicle, CancellationToken cancellationToken)
        {
            if (vehicle.Person.IsDuplicated)
            {
                Guid duplicatedVehicleId = await _duplicateDetectService.Exists(vehicle.Person, vehicle);
                if (duplicatedVehicleId != Guid.Empty)
                {
                    SSG_Asset_Vehicle duplicatedVehicle = null;
                    try
                    {
                        duplicatedVehicle = await _oDataClient
                            .For<SSG_Asset_Vehicle>()
                            .Key(duplicatedVehicleId)
                            .Expand(x => x.SSG_AssetOwners)
                            .FindEntryAsync(cancellationToken);

                        duplicatedVehicle.IsDuplicated = true;
                    }
                    catch (Exception ex)
                    {
                        LogDynamicsError(ex);
                        throw;
                    }

                    return duplicatedVehicle;
                }
            }

            SSG_Asset_Vehicle insertedVehicleRecord = null;
            try
            {
                insertedVehicleRecord = await _oDataClient
                    .For<SSG_Asset_Vehicle>()
                    .Set(vehicle)
                    .InsertEntryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }

            return insertedVehicleRecord;
        }

        public async Task<SSG_AssetOwner> CreateAssetOwner(AssetOwnerEntity owner, CancellationToken cancellationToken)
        {
            if (owner.Vehicle != null && owner.Vehicle.IsDuplicated)
            {
                Guid duplicatedOwnerId = await _duplicateDetectService.Exists(owner.Vehicle, owner);
                if (duplicatedOwnerId != Guid.Empty)
                    return new SSG_AssetOwner() { AssetOwnerId = duplicatedOwnerId };
            }

            SSG_AssetOwner insertedOwnerRecord = null;
            try
            {
                insertedOwnerRecord = await _oDataClient
                    .For<SSG_AssetOwner>()
                    .Set(owner)
                    .InsertEntryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }

            return insertedOwnerRecord;
        }

        public async Task<SSG_Asset_Other> CreateOtherAsset(AssetOtherEntity otherAsset, CancellationToken cancellationToken)
        {
            if (otherAsset.Person.IsDuplicated)
            {
                Guid duplicatedOtherAssetId = await _duplicateDetectService.Exists(otherAsset.Person, otherAsset);
                if (duplicatedOtherAssetId != Guid.Empty)
                    return new SSG_Asset_Other() { AssetOtherId = duplicatedOtherAssetId };
            }

            SSG_Asset_Other insertedOtherAssetRecord = null;
            try
            {
                insertedOtherAssetRecord = await this._oDataClient
                    .For<SSG_Asset_Other>()
                    .Set(otherAsset)
                    .InsertEntryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }

            return insertedOtherAssetRecord;
        }

        private async Task<SSG_Asset_WorkSafeBcClaim> GetDuplicatedCompensation(CompensationClaimEntity claim, CancellationToken cancellationToken)
        {
            if (claim.Person != null && claim.Person.IsDuplicated)
            {
                Guid duplicatedCompensationId = await _duplicateDetectService.Exists(claim.Person, claim);
                if (duplicatedCompensationId != Guid.Empty)
                {
                    SSG_Asset_WorkSafeBcClaim duplicatedClaim = null;
                    try
                    {
                        duplicatedClaim = await _oDataClient.For<SSG_Asset_WorkSafeBcClaim>()
                            .Key(duplicatedCompensationId)
                            .Expand(x => x.BankingInformation)
                            .Expand(x => x.Employment)
                            .FindEntryAsync(cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        LogDynamicsError(ex);
                        throw;
                    }

                    if (await _duplicateDetectService.Same(claim.BankInformationEntity, duplicatedClaim.BankingInformation))
                    {
                        if (await _duplicateDetectService.Same(claim.EmploymentEntity, duplicatedClaim.Employment))
                        {
                            duplicatedClaim.IsDuplicated = true;
                            if (duplicatedClaim.Employment != null)
                            {
                                try
                                {
                                    Guid duplicatedEmploymentId = duplicatedClaim.Employment.EmploymentId;
                                    duplicatedClaim.Employment = await _oDataClient.For<SSG_Employment>()
                                        .Key(duplicatedEmploymentId)
                                        .Expand(x => x.SSG_EmploymentContacts)
                                        .FindEntryAsync(cancellationToken);
                                    duplicatedClaim.Employment.IsDuplicated = true;
                                }
                                catch (Exception ex)
                                {
                                    LogDynamicsError(ex);
                                    throw;
                                }
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
                try
                {
                    returnedClaim = await this._oDataClient.For<SSG_Asset_WorkSafeBcClaim>().Set(claim).InsertEntryAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    LogDynamicsError(ex);
                    throw;
                }
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
                    try
                    {
                        var duplicatedInsurance = await _oDataClient.For<SSG_Asset_ICBCClaim>()
                            .Key(duplicatedInsuranceId)
                            .Expand(x => x.SSG_InvolvedParties)
                            .Expand(x => x.SSG_SimplePhoneNumbers)
                            .FindEntryAsync(cancellationToken);

                        duplicatedInsurance.IsDuplicated = true;
                        return duplicatedInsurance;
                    }
                    catch (Exception ex)
                    {
                        LogDynamicsError(ex);
                        throw;
                    }
                }
            }

            try
            {
                return await this._oDataClient.For<SSG_Asset_ICBCClaim>()
                    .Set(insurance)
                    .InsertEntryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }
        }

        public async Task<SSG_SimplePhoneNumber> CreateSimplePhoneNumber(SimplePhoneNumberEntity phone, CancellationToken cancellationToken)
        {
            if (phone.SSG_Asset_ICBCClaim != null && phone.SSG_Asset_ICBCClaim.IsDuplicated)
            {
                Guid duplicatedPhoneId = await _duplicateDetectService.Exists(phone.SSG_Asset_ICBCClaim, phone);
                if (duplicatedPhoneId != Guid.Empty)
                    return new SSG_SimplePhoneNumber() { SimplePhoneNumberId = duplicatedPhoneId };
            }

            try
            {
                return await this._oDataClient.For<SSG_SimplePhoneNumber>()
                    .Set(phone)
                    .InsertEntryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }
        }

        public async Task<SSG_InvolvedParty> CreateInvolvedParty(InvolvedPartyEntity involvedParty, CancellationToken cancellationToken)
        {
            if (involvedParty.SSG_Asset_ICBCClaim != null && involvedParty.SSG_Asset_ICBCClaim.IsDuplicated)
            {
                Guid duplicatedPartyId = await _duplicateDetectService.Exists(involvedParty.SSG_Asset_ICBCClaim, involvedParty);
                if (duplicatedPartyId != Guid.Empty)
                    return new SSG_InvolvedParty() { InvolvedPartyId = duplicatedPartyId };
            }

            try
            {
                return await this._oDataClient.For<SSG_InvolvedParty>()
                    .Set(involvedParty)
                    .InsertEntryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }
        }

        public async Task<SSG_SearchRequest> CreateSearchRequest(SearchRequestEntity searchRequest, CancellationToken cancellationToken)
        {
            SearchRequestEntity linkedSearchRequest = await LinkSearchRequestRef(searchRequest, cancellationToken);
            try
            {
                return await this._oDataClient.For<SSG_SearchRequest>()
                    .Set(linkedSearchRequest)
                    .InsertEntryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }
        }

        public async Task<SSG_SafetyConcernDetail> CreateSafetyConcern(SafetyConcernEntity safety, CancellationToken cancellationToken)
        {
            if (safety.Person.IsDuplicated)
            {
                Guid duplicatedSafetyId = await _duplicateDetectService.Exists(safety.Person, safety);
                if (duplicatedSafetyId != Guid.Empty)
                    return new SSG_SafetyConcernDetail() { SafetyConcernDetailId = duplicatedSafetyId };
            }

            try
            {
                return await this._oDataClient.For<SSG_SafetyConcernDetail>()
                    .Set(safety)
                    .InsertEntryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }
        }

        public async Task<SSG_SearchRequest> CancelSearchRequest(string fileId, string cancelComments, CancellationToken cancellationToken)
        {
            try
            {
                SSG_SearchRequest searchRequest = await _oDataClient
                    .For<SSG_SearchRequest>()
                    .Filter(x => x.FileId == fileId)
                    .FindEntryAsync(cancellationToken);

                if (searchRequest == null)
                {
                    return null;
                }

                var updatedRequest = await _oDataClient
                    .For<SSG_SearchRequest>()
                    .Key(searchRequest.SearchRequestId)
                    .Set(new Entry
                    {
                        { Keys.DYNAMICS_STATUS_CODE_FIELD, SearchRequestStatusCode.AgencyCancelled.Value },
                        { Keys.DYNAMICS_SEARCH_REQUEST_CANCEL_COMMENTS_FIELD, cancelComments }
                    })
                    .UpdateEntryAsync(cancellationToken);

                return updatedRequest;
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }
        }

        public async Task<SSG_SearchRequest> GetCurrentSearchRequest(Guid searchRequestId)
        {
            try
            {
                var searchRequest = await _oDataClient
                    .For<SSG_SearchRequest>()
                    .Key(searchRequestId)
                    .FindEntryAsync(CancellationToken.None);

                if (searchRequest == null)
                {
                    _logger.LogDebug("No SearchRequest found with Id={SearchRequestId}", searchRequestId);
                    return null;
                }

                return searchRequest;
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }
        }

        public async Task<SSG_SearchRequest> SystemCancelSearchRequest(SSG_SearchRequest searchRequest, CancellationToken cancellationToken)
        {
            if (searchRequest == null)
            {
                _logger.LogDebug("SystemCancelSearchRequest called with null SearchRequest.");
                return null;
            }

            try
            {
                var updatedRequest = await _oDataClient
                    .For<SSG_SearchRequest>()
                    .Key(searchRequest.SearchRequestId)
                    .Set(new Entry
                    {
                        { Keys.DYNAMICS_STATE_CODE_FIELD, 1 },
                        { Keys.DYNAMICS_STATUS_CODE_FIELD, SearchRequestStatusCode.SystemCancelled.Value },
                        { Keys.DYNAMICS_SEARCH_REQUEST_CANCEL_COMMENTS_FIELD, "Incomplete Search Request" }
                    })
                    .UpdateEntryAsync(cancellationToken);

                _logger.LogDebug("System-cancelled SearchRequest Id={SearchRequestId} successfully.", searchRequest.SearchRequestId);
                return updatedRequest;
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }
        }

        public async Task<bool> DeleteSearchRequest(string fileId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(fileId))
            {
                _logger.LogDebug("DeleteSearchRequest called with an empty or null fileId.");
                return false;
            }

            try
            {
                _logger.LogDebug("Attempting to delete SearchRequest with FileId={FileId}", fileId);

                await _oDataClient
                    .For<SSG_SearchRequest>()
                    .Filter(m => m.FileId == fileId)
                    .DeleteEntryAsync(cancellationToken);

                _logger.LogDebug("Successfully deleted SearchRequest with FileId={FileId}", fileId);
                return true;
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                _logger.LogError(ex, "Failed to delete SearchRequest with FileId={FileId}", fileId);
                throw;
            }
        }

        public async Task<SSG_SearchRequest> GetSearchRequest(string fileId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(fileId))
            {
                _logger.LogDebug("GetSearchRequest called with an empty or null fileId.");
                return null;
            }

            try
            {
                _logger.LogDebug("Retrieving SearchRequest with FileId={FileId}", fileId);

                SSG_SearchRequest ssgSearchRequest = await _oDataClient
                    .For<SSG_SearchRequest>()
                    .Select(x => x.SearchRequestId)
                    .Filter(x => x.FileId == fileId)
                    .FindEntryAsync(cancellationToken);

                if (ssgSearchRequest == null)
                {
                    _logger.LogDebug("No SearchRequest found for FileId={FileId}", fileId);
                    return null;
                }

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

                if (dataSearchRequest != null)
                {
                    dataSearchRequest.AgencyCode = dataSearchRequest.Agency?.AgencyCode;
                    dataSearchRequest.AgencyOfficeLocationText = dataSearchRequest.AgencyLocation?.LocationCode;
                    dataSearchRequest.SearchReasonCode = dataSearchRequest.SearchReason?.ReasonCode;
                }

                _logger.LogDebug("Successfully retrieved SearchRequest with FileId={FileId}", fileId);
                return dataSearchRequest;
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                _logger.LogError(ex, "Error retrieving SearchRequest with FileId={FileId}", fileId);
                throw;
            }
        }

        public async Task<SSG_Person> GetPerson(Guid personId, CancellationToken cancellationToken)
        {
            try
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
                    .Expand(x => x.SSG_SafetyConcernDetails)
                    .FindEntryAsync(cancellationToken);

                if (person == null)
                {
                    _logger.LogDebug("GetPerson, no person found with Id={PersonId}", personId);
                }
                return person;
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }
        }

        public async Task<SSG_Employment> GetEmployment(Guid employmentId, CancellationToken cancellationToken)
        {
            try
            {
                SSG_Employment employment = await _oDataClient
                    .For<SSG_Employment>()
                    .Key(employmentId)
                    .Expand(x => x.SSG_EmploymentContacts)
                    .FindEntryAsync(cancellationToken);

                if (employment == null)
                {
                    _logger.LogDebug("GetEmployment, no employment found with Id={EmploymentId}", employmentId);
                }

                return employment;
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }
        }

        public async Task<SSG_SearchRequest> UpdateSearchRequest(Guid requestId, IDictionary<string, object> updatedFields, CancellationToken cancellationToken)
        {
            updatedFields.Add(new KeyValuePair<string, object>("ssg_updatedbyagency", true));
            try
            {
                return await this._oDataClient
                    .For<SSG_SearchRequest>()
                    .Key(requestId)
                    .Set(updatedFields)
                    .UpdateEntryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }
        }

        public async Task<SSG_Person> UpdatePerson(SSG_Person existedPerson, IDictionary<string, object> updatedFields, PersonEntity newPerson, CancellationToken cancellationToken)
        {
            string duplicateDetectHash = await _duplicateDetectService.GetDuplicateDetectHashData(newPerson);
            if (duplicateDetectHash != existedPerson.DuplicateDetectHash)
            {
                updatedFields.Add("ssg_duplicatedetectionhash", duplicateDetectHash);
            }

            updatedFields.Add(new KeyValuePair<string, object>("ssg_updatedbyagency", true));
            try
            {
                return await this._oDataClient.For<SSG_Person>().Key(existedPerson.PersonId).Set(updatedFields).UpdateEntryAsync(cancellationToken);
            }
            catch (WebRequestException e) when (e.IsDuplicateHashError())
            {
                _logger.LogError(e, "Update Person failed with DuplicationHash [{hash}]", duplicateDetectHash);
                return null;
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }
        }

        public async Task<SSG_Identity> UpdateRelatedPerson(Guid relatedPersonId, IDictionary<string, object> updatedFields, CancellationToken cancellationToken)
        {
            updatedFields.Add(new KeyValuePair<string, object>("ssg_updatedbyagency", true));
            try
            {
                return await this._oDataClient
                    .For<SSG_Identity>()
                    .Key(relatedPersonId)
                    .Set(updatedFields)
                    .UpdateEntryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }
        }

        public async Task<SSG_Employment> UpdateEmployment(Guid employmentId, IDictionary<string, object> updatedFields, CancellationToken cancellationToken)
        {
            try
            {
                return await this._oDataClient
                    .For<SSG_Employment>()
                    .Key(employmentId)
                    .Set(updatedFields)
                    .UpdateEntryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }
        }

        public async Task<SSG_Identifier> UpdateIdentifier(Guid identifierId, IDictionary<string, object> updatedFields, CancellationToken cancellationToken)
        {
            try
            {
                return await this._oDataClient
                    .For<SSG_Identifier>()
                    .Key(identifierId)
                    .Set(updatedFields)
                    .UpdateEntryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }
        }

        public async Task<SSG_SafetyConcernDetail> UpdateSafetyConcern(Guid safetyId, IDictionary<string, object> updatedFields, CancellationToken cancellationToken)
        {
            updatedFields.Add(new KeyValuePair<string, object>("ssg_updatedbyagency", true));
            try
            {
                return await this._oDataClient
                    .For<SSG_SafetyConcernDetail>()
                    .Key(safetyId)
                    .Set(updatedFields)
                    .UpdateEntryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }
        }

        public async Task<SSG_Notese> CreateNotes(NotesEntity note, CancellationToken cancellationToken)
        {
            if (note.SearchRequest != null && note.SearchRequest.IsDuplicated)
            {
                Guid duplicatedNoteId = await _duplicateDetectService.Exists(note.SearchRequest, note);
                if (duplicatedNoteId != Guid.Empty)
                    return new SSG_Notese() { NotesId = duplicatedNoteId };
            }
            try
            {
                return await this._oDataClient
                    .For<SSG_Notese>()
                    .Set(note)
                    .InsertEntryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }
        }

        private async Task<SearchRequestEntity> LinkSearchRequestRef(SearchRequestEntity searchRequest, CancellationToken cancellationToken)
        {
            //find agencyCode in ssg_agency entity
            string code = searchRequest.AgencyCode;
            SSG_Agency agency = null;
            try
            {
                agency = await _oDataClient.For<SSG_Agency>()
                                    .Filter(x => x.AgencyCode == code)
                                    .FindEntryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }

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
            try
            {
                return await _oDataClient.For<SSG_SearchRequestReason>()
                                .Filter(x => x.ReasonCode == reasonCode)
                                .FindEntryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }
        }

        public async Task<SSG_AgencyLocation> GetSearchAgencyLocation(string locationCode, string agencyCode, CancellationToken cancellationToken)
        {
            try
            {
                SSG_AgencyLocation officeLocation = null;
                if (locationCode?.Length > 5)
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
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                return null;
            }
        }

        public async Task<SSG_Country> GetEmploymentCountry(string countryText, CancellationToken cancellationToken)
        {
            try
            {
                return await _oDataClient
                                .For<SSG_Country>()
                                .Filter(x => x.Name == countryText)
                                .FindEntryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }
        }

        public async Task<SSG_CountrySubdivision> GetEmploymentSubdivision(string subDivisionText, CancellationToken cancellationToken)
        {
            try
            {
                return await _oDataClient
                                .For<SSG_CountrySubdivision>()
                                .Filter(x => x.Name == subDivisionText)
                                .FindEntryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }
        }

        public async Task<bool> SubmitToQueue(Guid searchRequestId)
        {
            try
            {
                await _oDataClient
                    .For<SSG_SearchRequest>()
                    .Key(searchRequestId)
                    .Action("ssg_SearchRequestSubmittoQueueActions")
                    .ExecuteAsSingleAsync();
                return true;
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }
        }

        public async Task<bool> UpdateApiCall(Guid apiCallGuid, bool success, string notes, CancellationToken cancellationToken)
        {
            IDictionary<string, object> updatedFields = new Dictionary<string, object>
            {
                { "statuscode", success ? APICallStatusCode.Completed.Value : APICallStatusCode.Failed.Value},
                { "statecode", 1 },
                { "ssg_notes", notes }
            };

            try
            {
                await _oDataClient
                    .For<SSG_APICall>()
                    .Key(apiCallGuid)
                    .Set(updatedFields)
                    .UpdateEntryAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }
        }

        public async Task<IEnumerable<SSG_SearchRequest>> GetAutoCloseSearchRequestAsync(CancellationToken cancellationToken)
        {
            try
            {
                int noCPMatch = SearchRequestAutoCloseStatusCode.NoCPMatch.Value;
                int cpMissingData = SearchRequestAutoCloseStatusCode.CPMissingData.Value;
                int readyToClose = SearchRequestAutoCloseStatusCode.ReadyToClose.Value;
                int autoclosed = SearchRequestStatusCode.SearchRequestAutoClosed.Value;

                IEnumerable<SSG_SearchRequest> searchRequests = await _oDataClient.For<SSG_SearchRequest>()
                    .Filter(x => x.AutoCloseStatus == noCPMatch
                            || x.AutoCloseStatus == cpMissingData
                            || x.AutoCloseStatus == readyToClose)
                    .Filter(x => x.StatusCode != autoclosed)
                    .FindEntriesAsync(cancellationToken);

                return searchRequests;
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }
        }

        private async Task<EmploymentEntity> LinkEmploymentRef(EmploymentEntity employment, CancellationToken cancellationToken)
        {
            employment.Country = await GetEmploymentCountry(employment.CountryText, cancellationToken);
            employment.CountrySubdivision = await GetEmploymentSubdivision(employment.CountrySubdivisionText, cancellationToken);
            return employment;
        }

        private async Task<SSG_Person> FindDuplicatedPerson(PersonEntity newPerson)
        {
            newPerson.DuplicateDetectHash = await _duplicateDetectService.GetDuplicateDetectHashData(newPerson);
            string hashData = newPerson.DuplicateDetectHash;
            try
            {
                var existedPerson = await this._oDataClient.For<SSG_Person>()
                    .Filter(x => x.DuplicateDetectHash == hashData)
                    .FindEntryAsync(CancellationToken.None);

                return existedPerson;
            }
            catch (WebRequestException e) when (e.Code == HttpStatusCode.NotFound)
            {
                _logger.LogDebug("FindDuplicatedPerson, no duplicated person found for hash [{HashData}]", hashData);
                return null;
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }
        }

        public async Task<bool> SearchRequestCreateCouldNotAutoCloseNote(Guid searchRequestId)
        {
            try
            {
                await _oDataClient
                    .For<SSG_SearchRequest>()
                    .Key(searchRequestId)
                    .Action("ssg_SearchRequestCreateCouldNotAutoCloseNote")
                    .ExecuteAsSingleAsync();

                return true;
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }
        }

        public async Task<SSG_SearchRequest> UpdateSearchRequestStatusAutoClosed(Guid searchRequestId, CancellationToken cancellationToken)
        {
            try
            {
                return await _oDataClient
                    .For<SSG_SearchRequest>()
                    .Key(searchRequestId)
                    .Set(new Entry
                    {
                        { Keys.DYNAMICS_STATE_CODE_FIELD, 1 },
                        { Keys.DYNAMICS_STATUS_CODE_FIELD, SearchRequestStatusCode.SearchRequestAutoClosed.Value }
                    })
                    .UpdateEntryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogDynamicsError(ex);
                throw;
            }
        }
    }
}
