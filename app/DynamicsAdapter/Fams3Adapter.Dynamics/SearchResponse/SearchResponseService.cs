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
using Fams3Adapter.Dynamics.Vehicle;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fams3Adapter.Dynamics.SearchResponse
{
    public interface ISearchResponseService
    {
        Task<SSG_SearchRequestResponse> GetSearchResponse(Guid responseId, CancellationToken cancellationToken);
    }

    /// <summary>
    /// The SearchRequestService expose Search Request Functionality
    /// </summary>
    public class SearchResponseService : ISearchResponseService
    {
        private readonly IODataClient _oDataClient;

        public SearchResponseService(IODataClient oDataClient)
        {
            this._oDataClient = oDataClient;
        }



        public async Task<SSG_SearchRequestResponse> GetSearchResponse(Guid responseId, CancellationToken cancellationToken)
        {
            SSG_SearchRequestResponse ssgSearchResponse = await _oDataClient
                .For<SSG_SearchRequestResponse>()
                .Key(responseId)
                .Expand(x => x.SSG_BankInfos)
                .Expand(x => x.SSG_Asset_Others)
                .Expand(x => x.SSG_Addresses)
                .Expand(x => x.SSG_Aliases)
                .Expand(x => x.SSG_Asset_ICBCClaims)
                .Expand(x => x.SSG_Asset_Vehicles)
                .Expand(x => x.SSG_Asset_WorkSafeBcClaims)
                .Expand(x => x.SSG_Employments)
                .Expand(x => x.SSG_Identifiers)
                .Expand(x => x.SSG_Identities)
                .Expand(x => x.SSG_Noteses)
                .Expand(x => x.SSG_Persons)
                .Expand(x => x.SSG_PhoneNumbers)
                .Expand(x => x.SSG_SearchRequests)
                .Expand(x => x.SSG_Asset_Investments)
                .Expand(x => x.SSG_SafetyConcernDetails)
                .Expand(x => x.SSG_Asset_PensionDisablilitys)
                .Expand(x => x.SSG_Asset_RealEstatePropertys)
                .FindEntryAsync(cancellationToken);

            if (ssgSearchResponse.SSG_Addresses != null)
            {
                foreach (SSG_Address address in ssgSearchResponse.SSG_Addresses)
                {
                    SSG_Address addr = await _oDataClient.For<SSG_Address>()
                        .Key(address.AddressId)
                        .Expand(x => x.CountrySubdivision)
                        .FindEntryAsync(cancellationToken);

                    if (addr.CountrySubdivision != null)
                    {
                        address.CountrySubdivisionText = addr.CountrySubdivision.ProvinceCode;
                    }

                }
            }

            if (ssgSearchResponse.SSG_Asset_ICBCClaims != null)
            {
                foreach (SSG_Asset_ICBCClaim claim in ssgSearchResponse.SSG_Asset_ICBCClaims)
                {
                    SSG_Asset_ICBCClaim expandedClaim = await _oDataClient.For<SSG_Asset_ICBCClaim>()
                        .Key(claim.ICBCClaimId)
                        .Expand(x => x.CountrySubdivision)
                        .FindEntryAsync(cancellationToken);

                    if (expandedClaim.CountrySubdivision != null)
                    {
                        claim.SupplierCountrySubdivisionCode = expandedClaim.CountrySubdivision.ProvinceCode;
                    }

                }
            }

            if(ssgSearchResponse.SSG_Employments != null)
            {
                foreach (SSG_Employment e in ssgSearchResponse.SSG_Employments)
                {
                    SSG_Employment expandedEmployment = await _oDataClient.For<SSG_Employment>()
                        .Key(e.EmploymentId)
                        .Expand(x => x.CountrySubdivision)
                        .FindEntryAsync(cancellationToken);

                    if (expandedEmployment.CountrySubdivision != null)
                    {
                        e.CountrySubdivisionText = expandedEmployment.CountrySubdivision.ProvinceCode;
                    }
                }
            }

            return ssgSearchResponse;
        }


    }
}