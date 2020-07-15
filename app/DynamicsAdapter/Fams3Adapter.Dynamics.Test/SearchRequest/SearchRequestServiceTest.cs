using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.AssetOwner;
using Fams3Adapter.Dynamics.BankInfo;
using Fams3Adapter.Dynamics.CompensationClaim;
using Fams3Adapter.Dynamics.Duplicate;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.InsuranceClaim;
using Fams3Adapter.Dynamics.OtherAsset;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.ResultTransaction;
using Fams3Adapter.Dynamics.SearchRequest;
using Moq;
using NUnit.Framework;
using Simple.OData.Client;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Fams3Adapter.Dynamics.Test.SearchRequest
{
    public class SearchRequestServiceTest
    {
        private Mock<IODataClient> odataClientMock;
        private Mock<IDuplicateDetectionService> _duplicateServiceMock;

        private readonly Guid testId = Guid.Parse("6AE89FE6-9909-EA11-B813-00505683FBF4");
        private readonly Guid testPersonId = Guid.Parse("6AE89FE6-9909-EA11-1111-00505683FBF4");
        private readonly Guid testAssetOtherId = Guid.Parse("77789FE6-9909-EA11-1901-000056837777");

        private SearchRequestService _sut;

        [SetUp]
        public void SetUp()
        {
            odataClientMock = new Mock<IODataClient>();
            _duplicateServiceMock = new Mock<IDuplicateDetectionService>();

            odataClientMock.Setup(x => x.For<SSG_Country>(null)
             .Filter(It.IsAny<Expression<Func<SSG_Country, bool>>>())
             .FindEntryAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.FromResult<SSG_Country>(new SSG_Country()
             {
                 CountryId = Guid.NewGuid(),
                 Name = "Canada"
             }));

            odataClientMock.Setup(x => x.For<SSG_CountrySubdivision>(null)
                .Filter(It.IsAny<Expression<Func<SSG_CountrySubdivision, bool>>>())
                .FindEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_CountrySubdivision>(new SSG_CountrySubdivision()
                {
                    CountrySubdivisionId = Guid.NewGuid(),
                    Name = "British Columbia"
                }));

            odataClientMock.Setup(x => x.For<SSG_Asset_ICBCClaim>(null).Set(It.IsAny<ICBCClaimEntity>())
             .InsertEntryAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.FromResult(new SSG_Asset_ICBCClaim()
             {
                 ClaimNumber = "icbcClaim"
             })
             );

            odataClientMock.Setup(x => x.For<SSG_SimplePhoneNumber>(null).Set(It.IsAny<SimplePhoneNumberEntity>())
             .InsertEntryAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.FromResult(new SSG_SimplePhoneNumber()
             {
                 PhoneNumber = "phone"
             })
             );

            odataClientMock.Setup(x => x.For<SSG_InvolvedParty>(null).Set(It.IsAny<InvolvedPartyEntity>())
            .InsertEntryAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new SSG_InvolvedParty()
            {
                OrganizationName = "party"
            })
            );

            odataClientMock.Setup(x => x.For<SSG_SearchRequestResultTransaction>(null).Set(It.IsAny<SSG_SearchRequestResultTransaction>())
            .InsertEntryAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new SSG_SearchRequestResultTransaction()
            {
                SourceIdentifier = new SSG_Identifier() { Identification = "11111" }
            })
            );

            _sut = new SearchRequestService(odataClientMock.Object, _duplicateServiceMock.Object);
        }





        [Test]
        public async Task upload_ICBCClaimEntity_should_success()
        {
            var claim = new ICBCClaimEntity()
            {
                ClaimNumber = "icbcClaim",
                SearchRequest = new SSG_SearchRequest() { SearchRequestId = testId },
                Person = new SSG_Person() { PersonId = testPersonId }

            };

            var result = await _sut.CreateInsuranceClaim(claim, CancellationToken.None);

            Assert.AreEqual("icbcClaim", result.ClaimNumber);
        }

        [Test]
        public async Task upload_SimplePhoneNumber_should_success()
        {
            var phone = new SimplePhoneNumberEntity()
            {
                PhoneNumber = "phone"
            };

            var result = await _sut.CreateSimplePhoneNumber(phone, CancellationToken.None);

            Assert.AreEqual("phone", result.PhoneNumber);
        }

        [Test]
        public async Task upload_InvolvedParty_should_success()
        {
            var party = new InvolvedPartyEntity()
            {
                OrganizationName = "party"
            };

            var result = await _sut.CreateInvolvedParty(party, CancellationToken.None);

            Assert.AreEqual("party", result.OrganizationName);
        }


        [Test]
        public async Task upload_ResultTransaction_should_success()
        {
            var trans = new SSG_SearchRequestResultTransaction() { };

            var result = await _sut.CreateTransaction(trans, CancellationToken.None);

            Assert.AreEqual("11111", result.SourceIdentifier.Identification);
        }
    }
}
