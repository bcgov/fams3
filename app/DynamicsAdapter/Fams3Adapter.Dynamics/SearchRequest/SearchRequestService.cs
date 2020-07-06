using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.AssetOwner;
using Fams3Adapter.Dynamics.BankInfo;
using Fams3Adapter.Dynamics.Config;
using Fams3Adapter.Dynamics.Employment;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.InsuranceClaim;
using Fams3Adapter.Dynamics.Name;
using Fams3Adapter.Dynamics.OtherAsset;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.PhoneNumber;
using Fams3Adapter.Dynamics.RelatedPerson;
using Fams3Adapter.Dynamics.ResultTransaction;
using Fams3Adapter.Dynamics.Vehicle;
using Newtonsoft.Json;
using Simple.OData.Client;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Fams3Adapter.Dynamics.SearchRequest
{
    public interface ISearchRequestService
    {
        Task<SSG_Identifier> CreateIdentifier(IdentifierEntity identifier, CancellationToken cancellationToken);
        Task<SSG_Address> CreateAddress(AddressEntity address, CancellationToken cancellationToken);
        Task<SSG_PhoneNumber> CreatePhoneNumber(PhoneNumberEntity phoneNumber, CancellationToken cancellationToken);
        Task<SSG_Aliase> CreateName(AliasEntity name, CancellationToken cancellationToken);
        Task<SSG_Identity> CreateRelatedPerson(RelatedPersonEntity name, CancellationToken cancellationToken);

        Task<SSG_Person> SavePerson(PersonEntity person, CancellationToken cancellationToken);
        Task<SSG_Employment> CreateEmployment(EmploymentEntity employment, CancellationToken cancellationToken);
        Task<SSG_EmploymentContact> CreateEmploymentContact(SSG_EmploymentContact employmentContact, CancellationToken cancellationToken);
        Task<SSG_Asset_BankingInformation> CreateBankInfo(BankingInformationEntity bankInfo, CancellationToken cancellationToken);
        Task<SSG_Asset_Vehicle> CreateVehicle(VehicleEntity vehicle, CancellationToken cancellationToken);
        Task<SSG_AssetOwner> CreateAssetOwner(SSG_AssetOwner owner, CancellationToken cancellationToken);
        Task<SSG_Asset_Other> CreateOtherAsset(AssetOtherEntity asset, CancellationToken cancellationToken);
        Task<SSG_Asset_WorkSafeBcClaim> CreateCompensationClaim(CompensationClaimEntity claim, CancellationToken cancellationToken);
        Task<SSG_Asset_ICBCClaim> CreateInsuranceClaim(ICBCClaimEntity claim, CancellationToken cancellationToken);
        Task<SSG_SimplePhoneNumber> CreateSimplePhoneNumber(SSG_SimplePhoneNumber phone, CancellationToken cancellationToken);
        Task<SSG_InvolvedParty> CreateInvolvedParty(SSG_InvolvedParty involvedParty, CancellationToken cancellationToken);
        Task<SSG_SearchRequestResultTransaction> CreateTransaction(SSG_SearchRequestResultTransaction transaction, CancellationToken cancellationToken);
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
            try
            {
                identifier.DuplicateDetectHash = await _duplicateDetectService.GetDuplicateDetectHashData(identifier);
                return await this._oDataClient.For<SSG_Identifier>().Set(identifier).InsertEntryAsync(cancellationToken);

            }
            catch (WebRequestException ex)
            {
                if (IsDuplicateFoundException(ex))
                {
                    return null;
                }
                else
                {
                    throw ex;
                }
            }
        }

        public async Task<SSG_Person> SavePerson(PersonEntity person, CancellationToken cancellationToken)
        {
            try
            {
                person.DuplicateDetectHash = await _duplicateDetectService.GetDuplicateDetectHashData(person);
                return await this._oDataClient.For<SSG_Person>().Set(person).InsertEntryAsync(cancellationToken);
            }catch(WebRequestException ex)
            {
                if (IsDuplicateFoundException(ex))
                {
                    string hashData = person.DuplicateDetectHash;
                    SSG_Person p = await this._oDataClient.For<SSG_Person>().Filter(x => x.DuplicateDetectHash == hashData).FindEntryAsync(cancellationToken);
                    return p;
                }
                else
                {
                    throw ex;
                }
            }
        }

        public async Task<SSG_PhoneNumber> CreatePhoneNumber(PhoneNumberEntity phone, CancellationToken cancellationToken)
        {
            try
            {
                phone.DuplicateDetectHash = await _duplicateDetectService.GetDuplicateDetectHashData(phone);
                return await this._oDataClient.For<SSG_PhoneNumber>().Set(phone).InsertEntryAsync(cancellationToken);
            }
            catch (WebRequestException ex)
            {
                if (IsDuplicateFoundException(ex))
                {
                    return null;
                }
                else
                {
                    throw ex;
                }
            }            
        }

        public async Task<SSG_SearchRequestResultTransaction> CreateTransaction(SSG_SearchRequestResultTransaction transaction, CancellationToken cancellationToken)
        {
            return await this._oDataClient.For<SSG_SearchRequestResultTransaction>().Set(transaction).InsertEntryAsync(cancellationToken);
        }

        public async Task<SSG_Address> CreateAddress(AddressEntity address, CancellationToken cancellationToken)
        {
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

            try
            {
                address.DuplicateDetectHash = await _duplicateDetectService.GetDuplicateDetectHashData(address);
                return await this._oDataClient.For<SSG_Address>().Set(address).InsertEntryAsync(cancellationToken);
            }
            catch (WebRequestException ex)
            {
                if (IsDuplicateFoundException(ex))
                {
                    return null;
                }
                else
                {
                    throw ex;
                }
            }
        }

        public async Task<SSG_Aliase> CreateName(AliasEntity name, CancellationToken cancellationToken)
        {
            return await this._oDataClient.For<SSG_Aliase>().Set(name).InsertEntryAsync(cancellationToken);
        }

        public async Task<SSG_Employment> CreateEmployment(EmploymentEntity employment, CancellationToken cancellationToken)
        {
            var countryText = employment.CountryText;
            var country = await _oDataClient.For<SSG_Country>()
                                            .Filter(x => x.Name == countryText)
                                            .FindEntryAsync(cancellationToken);
            employment.Country = country;

            var subDivisionText = employment.CountrySubdivisionText;
            var subdivision = await _oDataClient.For<SSG_CountrySubdivision>()
                                      .Filter(x => x.Name == subDivisionText)
                                      .FindEntryAsync(cancellationToken);
            employment.CountrySubdivision = subdivision;


            return await this._oDataClient.For<SSG_Employment>().Set(employment).InsertEntryAsync(cancellationToken);
        }

        public async Task<SSG_EmploymentContact> CreateEmploymentContact(SSG_EmploymentContact employmentContact, CancellationToken cancellationToken)
        {
            return await this._oDataClient.For<SSG_EmploymentContact>().Set(employmentContact).InsertEntryAsync(cancellationToken);
        }

        public async Task<SSG_Identity> CreateRelatedPerson(RelatedPersonEntity relatedPerson, CancellationToken cancellationToken)
        {
            return await this._oDataClient.For<SSG_Identity>().Set(relatedPerson).InsertEntryAsync(cancellationToken);
        }

        public async Task<SSG_Asset_BankingInformation> CreateBankInfo(BankingInformationEntity bankInfo, CancellationToken cancellationToken)
        {
            return await this._oDataClient.For<SSG_Asset_BankingInformation>().Set(bankInfo).InsertEntryAsync(cancellationToken);
        }

        public async Task<SSG_Asset_Vehicle> CreateVehicle(VehicleEntity vehicle, CancellationToken cancellationToken)
        {
            return await this._oDataClient.For<SSG_Asset_Vehicle>().Set(vehicle).InsertEntryAsync(cancellationToken);
        }

        public async Task<SSG_AssetOwner> CreateAssetOwner(SSG_AssetOwner owner, CancellationToken cancellationToken)
        {
            return await this._oDataClient.For<SSG_AssetOwner>().Set(owner).InsertEntryAsync(cancellationToken);
        }

        public async Task<SSG_Asset_Other> CreateOtherAsset(AssetOtherEntity otherAsset, CancellationToken cancellationToken)
        {
            return await this._oDataClient.For<SSG_Asset_Other>().Set(otherAsset).InsertEntryAsync(cancellationToken);
        }

        public async Task<SSG_Asset_WorkSafeBcClaim> CreateCompensationClaim(CompensationClaimEntity claim, CancellationToken cancellationToken)
        {
            return await this._oDataClient.For<SSG_Asset_WorkSafeBcClaim>().Set(claim).InsertEntryAsync(cancellationToken);
        }

        public async Task<SSG_Asset_ICBCClaim> CreateInsuranceClaim(ICBCClaimEntity claim, CancellationToken cancellationToken)
        {
            return await this._oDataClient.For<SSG_Asset_ICBCClaim>().Set(claim).InsertEntryAsync(cancellationToken);
        }

        public async Task<SSG_SimplePhoneNumber> CreateSimplePhoneNumber(SSG_SimplePhoneNumber phone, CancellationToken cancellationToken)
        {
            return await this._oDataClient.For<SSG_SimplePhoneNumber>().Set(phone).InsertEntryAsync(cancellationToken);
        }

        public async Task<SSG_InvolvedParty> CreateInvolvedParty(SSG_InvolvedParty involvedParty, CancellationToken cancellationToken)
        {
            return await this._oDataClient.For<SSG_InvolvedParty>().Set(involvedParty).InsertEntryAsync(cancellationToken);
        }

        private bool IsDuplicateFoundException(WebRequestException ex)
        {
            if (ex.Code == HttpStatusCode.PreconditionFailed && ex.Response.Contains(Keys.DUPLICATE_DETECTED_ERROR_CODE) )
            {
                return true;
            }
            return false;
        }
    }
}