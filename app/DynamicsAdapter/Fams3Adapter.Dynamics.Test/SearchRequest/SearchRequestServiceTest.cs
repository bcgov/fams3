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
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Types;
using Fams3Adapter.Dynamics.Vehicle;
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
        private readonly Guid testBankInfoId = Guid.Parse("00089FE6-9909-EA11-1901-000056837777");

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

            odataClientMock.Setup(x => x.For<SSG_Identity>(null).Set(It.Is<RelatedPersonEntity>(x => x.FirstName == "First"))
        .InsertEntryAsync(It.IsAny<CancellationToken>()))
        .Returns(Task.FromResult(new SSG_Identity()
        {
            FirstName = "FirstName"
        })
        );

         odataClientMock.Setup(x => x.For<SSG_Employment>(null).Set(It.Is<EmploymentEntity>(x => x.BusinessOwner == "Business Owner"))
         .InsertEntryAsync(It.IsAny<CancellationToken>()))
         .Returns(Task.FromResult(new SSG_Employment()
         {
             BusinessOwner = "Business Owner"
         })
         );

            odataClientMock.Setup(x => x.For<SSG_EmploymentContact>(null).Set(It.Is<SSG_EmploymentContact>(x => x.PhoneNumber == "12345678"))
            .InsertEntryAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new SSG_EmploymentContact()
            {
             PhoneNumber = "12345678"
            })
            );

            odataClientMock.Setup(x => x.For<SSG_Asset_BankingInformation>(null).Set(It.Is<BankingInformationEntity>(x => x.BankName == "bank"))
            .InsertEntryAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new SSG_Asset_BankingInformation()
            {
              BankName = "bank",
            })
            );

            odataClientMock.Setup(x => x.For<SSG_AssetOwner>(null).Set(It.Is<AssetOwnerEntity>(x => x.FirstName == "firstName"))
             .InsertEntryAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.FromResult(new SSG_AssetOwner()
             {
                 FirstName = "firstName"
             })
             );

            odataClientMock.Setup(x => x.For<SSG_Asset_Other>(null).Set(It.Is<AssetOtherEntity>(x => x.AssetDescription == "asset description"))
             .InsertEntryAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.FromResult(new SSG_Asset_Other()
             {
                 AssetOtherId = testAssetOtherId
             })
             );

            odataClientMock.Setup(x => x.For<SSG_Asset_WorkSafeBcClaim>(null).Set(It.Is<CompensationClaimEntity>(x => x.ClaimNumber == "compensationClaimNumber"))
             .InsertEntryAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.FromResult(new SSG_Asset_WorkSafeBcClaim()
             {
                 ClaimNumber = "compensationClaimNumber"
             })
             );

            odataClientMock.Setup(x => x.For<SSG_Asset_ICBCClaim>(null).Set(It.IsAny<ICBCClaimEntity>())
             .InsertEntryAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.FromResult(new SSG_Asset_ICBCClaim()
             {
                 ClaimNumber = "icbcClaim"
             })
             );

            odataClientMock.Setup(x => x.For<SSG_SimplePhoneNumber>(null).Set(It.IsAny<SSG_SimplePhoneNumber>())
             .InsertEntryAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.FromResult(new SSG_SimplePhoneNumber()
             {
                 PhoneNumber= "phone"
             })
             );

            odataClientMock.Setup(x => x.For<SSG_InvolvedParty>(null).Set(It.IsAny<SSG_InvolvedParty>())
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
                SourceIdentifier = new SSG_Identifier() { Identification="11111"}
            })
            );

             _sut = new SearchRequestService(odataClientMock.Object, _duplicateServiceMock.Object);
        }

        [Test]
        public async Task with_correct_searchRequestid_upload_related_person_should_success()
        {
            var relatedPerson = new RelatedPersonEntity()
            {
                FirstName = "First",
                LastName = "lastName",
                MiddleName = "middleName",
                ThirdGivenName = "otherName",
                Type = PersonRelationType.Friend.Value,
                Notes = "notes",
                SupplierRelationType = "friend",
                Date1 = new DateTime(2001, 1, 1),
                Date1Label = "date1lable",
                Date2 = new DateTime(2005, 1, 1),
                Date2Label = "date2lable",
                Gender = GenderType.Female.Value,
                Description = "description",
                SearchRequest = new SSG_SearchRequest() { SearchRequestId = testId },
                Person = new SSG_Person() { PersonId = testPersonId }
            };

            var result = await _sut.CreateRelatedPerson(relatedPerson, CancellationToken.None);

            Assert.AreEqual("FirstName", result.FirstName);
        }


        [Test]
        public async Task with_correct_searchRequestid_upload_employment_should_succed()
        {
            var employment = new EmploymentEntity()
            {
                BusinessOwner= "Business Owner",
                BusinessName = "Business Name",
                Notes = "notes",
                Date1 = new DateTime(2001, 1, 1),
                Date1Label = "date1lable",
                Date2 = new DateTime(2005, 1, 1),
                Date2Label = "date2lable",
        
                SearchRequest = new SSG_SearchRequest() { SearchRequestId = testId },
                Person = new SSG_Person() { PersonId = testPersonId }
            };

            var result = await _sut.CreateEmployment(employment, CancellationToken.None);

            Assert.AreEqual("Business Owner", result.BusinessOwner);
        }

        [Test]
        public async Task with_correct_employmentid_upload_employmentcontact_should_succed()
        {
            var employmentContact = new SSG_EmploymentContact()
            {
                Employment = new SSG_Employment() { EmploymentId = testId },
                PhoneNumber = "12345678"
            };

            var result = await _sut.CreateEmploymentContact(employmentContact, CancellationToken.None);

            Assert.AreEqual("12345678", result.PhoneNumber);
        }

        [Test]
        public async Task with_correct_searchRequestid_upload_bank_info_should_success()
        {
            var bankInfo = new BankingInformationEntity()
            {
                BankName = "bank",
                TransitNumber = "123456",
                AccountNumber = "123456",
                Branch = "branch",
                Notes = "notes",
                Date1 = new DateTime(2001, 1, 1),
                Date1Label = "date1lable",
                Date2 = new DateTime(2005, 1, 1),
                Date2Label = "date2lable",
                SearchRequest = new SSG_SearchRequest() { SearchRequestId = testId },
                Person = new SSG_Person() { PersonId = testPersonId }
            };

            var result = await _sut.CreateBankInfo(bankInfo, CancellationToken.None);

            Assert.AreEqual("bank", result.BankName);
        }

        [Test]
        public async Task with_correct_searchRequestid_upload_otherasset_should_success()
        {
            var assetOther = new AssetOtherEntity()
            {
                AssetDescription = "asset description",
                Description = "description",
                TypeDescription="asset type description",
                Notes = "notes",
                Date1 = new DateTime(2001, 1, 1),
                Date1Label = "date1lable",
                Date2 = new DateTime(2005, 1, 1),
                Date2Label = "date2lable",
                SearchRequest = new SSG_SearchRequest() { SearchRequestId = testId },
                Person = new SSG_Person() { PersonId = testPersonId }
            };

            var result = await _sut.CreateOtherAsset(assetOther, CancellationToken.None);

            Assert.AreEqual(testAssetOtherId, result.AssetOtherId);
        }

        [Test]
        public async Task with_correct_bankInfomationId_upload_worksafebcClaim_should_success()
        {
            var claim = new CompensationClaimEntity()
            {
                ClaimNumber = "compensationClaimNumber",
                BankingInformation = new SSG_Asset_BankingInformation() { BankingInformationId = testBankInfoId },
                Notes = "notes",
                Date1 = new DateTime(2001, 1, 1),
                Date1Label = "date1lable",
                Date2 = new DateTime(2005, 1, 1),
                Date2Label = "date2lable",
                SearchRequest = new SSG_SearchRequest() { SearchRequestId = testId },
                Person = new SSG_Person() { PersonId = testPersonId }
               
            };

            var result = await _sut.CreateCompensationClaim(claim, CancellationToken.None);

            Assert.AreEqual("compensationClaimNumber", result.ClaimNumber);
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
            var phone = new SSG_SimplePhoneNumber()
            {
                PhoneNumber="phone"
            };

            var result = await _sut.CreateSimplePhoneNumber(phone, CancellationToken.None);

            Assert.AreEqual("phone", result.PhoneNumber);
        }

        [Test]
        public async Task upload_InvolvedParty_should_success()
        {
            var party = new SSG_InvolvedParty()
            {
                OrganizationName = "party"
            };

            var result = await _sut.CreateInvolvedParty(party, CancellationToken.None);

            Assert.AreEqual("party", result.OrganizationName);
        }


        [Test]
        public async Task upload_ResultTransaction_should_success()
        {
            var trans = new SSG_SearchRequestResultTransaction(){};

            var result = await _sut.CreateTransaction(trans, CancellationToken.None);

            Assert.AreEqual("11111", result.SourceIdentifier.Identification);
        }
    }
}
