using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.SearchResponse;
using Moq;
using NUnit.Framework;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fams3Adapter.Dynamics.Test.SearchResponse
{
    public class SearchResponseServiceTest
    {
        private Mock<IODataClient> _odataClientMock;
        private SearchResponseService _sut;
        private Guid _responseId;

        [SetUp]
        public void SetUp()
        {
            _responseId = Guid.NewGuid();
            _odataClientMock = new Mock<IODataClient>();
            _odataClientMock.Setup(
                x => x.For<SSG_SearchRequestResponse>(null).Key(It.IsAny<Guid>())
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
                .Expand(x => x.SSG_Emails)
                .Expand(x => x.SSG_Electronicas)
                .Expand(x => x.SSG_Taxincomeinformations)
                .FindEntryAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new SSG_SearchRequestResponse()
            {
                SearchRequestResponseId = _responseId
            })
            );
            _sut = new SearchResponseService(_odataClientMock.Object);
        }

        [Test]
        public async Task GetSearchResponse_should_success()
        {
            var result = await _sut.GetSearchResponse(_responseId, CancellationToken.None);

            Assert.AreEqual(_responseId, result.SearchRequestResponseId);
        }

        [Test]
        public async Task GetSearchResponse_address_province_use_code_success()
        {
            Guid subDivisionId = Guid.NewGuid();
            Guid addrId = Guid.NewGuid();
            _odataClientMock.Setup(
                 x => x.For<SSG_SearchRequestResponse>(null).Key(It.IsAny<Guid>())
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
                 .Expand(x => x.SSG_Emails)
                .Expand(x => x.SSG_Electronicas)
                .Expand(x => x.SSG_Taxincomeinformations)
                 .FindEntryAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.FromResult(new SSG_SearchRequestResponse()
             {
                 SearchRequestResponseId = _responseId,
                 SSG_Addresses = new List<SSG_Address>()
                 {
                     new SSG_Address{ AddressId=addrId, CountrySubdivision = new SSG_CountrySubdivision{ CountrySubdivisionId=subDivisionId}}
                 }.ToArray()
             })
             );

            _odataClientMock.Setup(
                x => x.For<SSG_Address>(null).Key(It.Is<Guid>(m => m == addrId))
                .Expand(x=>x.CountrySubdivision)
                .FindEntryAsync(It.IsAny<CancellationToken>()))
            .Returns(
                Task.FromResult(new SSG_Address()
                {
                    CountrySubdivision=new SSG_CountrySubdivision { ProvinceCode="BC"}
                })
                );
            var result = await _sut.GetSearchResponse(_responseId, CancellationToken.None);

            Assert.AreEqual(_responseId, result.SearchRequestResponseId);
            Assert.AreEqual("BC", result.SSG_Addresses[0].CountrySubdivisionText);
        }

    }
}
