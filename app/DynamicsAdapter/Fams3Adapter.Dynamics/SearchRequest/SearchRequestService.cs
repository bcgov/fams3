using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.AssetOwner;
using Fams3Adapter.Dynamics.BankInfo;
using Fams3Adapter.Dynamics.Duplicate;
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
using System.Linq;
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
        Task<SSG_AssetOwner> CreateAssetOwner(AssetOwnerEntity owner, CancellationToken cancellationToken);
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
            if ( identifier.Person.IsDuplicated )
            {
                Guid duplicatedIdentifierId = await _duplicateDetectService.Exists(identifier.Person, identifier);
                if(duplicatedIdentifierId != Guid.Empty)
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

            if(p == null)
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

            if (address.Person.IsDuplicated)
            {
                Guid duplicatedAddressId = await _duplicateDetectService.Exists(address.Person, address);
                if (duplicatedAddressId != Guid.Empty)
                    return new SSG_Address() { AddressId = duplicatedAddressId };
            }
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
            if(owner.Vehicle!=null && owner.Vehicle.IsDuplicated)
            {
                Guid duplicatedOwnerId = await _duplicateDetectService.Exists(owner.Vehicle, owner);
                if (duplicatedOwnerId != Guid.Empty)
                    return new SSG_AssetOwner() { AssetOwnerId = duplicatedOwnerId };
            }
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

    }
}