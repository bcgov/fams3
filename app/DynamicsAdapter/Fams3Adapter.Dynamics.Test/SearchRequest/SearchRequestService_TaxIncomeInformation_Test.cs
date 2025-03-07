using Fams3Adapter.Dynamics.Duplicate;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.TaxIncomeInformation;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Fams3Adapter.Dynamics.Test.SearchRequest
{
    public class SearchRequestService_TaxIncomeInformation_Test
    {
        private Mock<IODataClient> _odataClientMock;
        private Mock<IDuplicateDetectionService> _duplicateServiceMock;
        private Mock<ILogger<SearchRequestService>> _loggerMock;

        private readonly Guid _testId = Guid.Parse("6AE89FE6-9909-EA11-B813-00505683FBF4");
        private readonly Guid _testTaxIncomeInformationId = Guid.Parse("6AE89FE6-9909-EA11-1111-00505683FBF4");
        private SearchRequestService _sut;

        [SetUp]
        public void SetUp()
        {
            _odataClientMock = new Mock<IODataClient>();
            _duplicateServiceMock = new Mock<IDuplicateDetectionService>();
            _loggerMock = new Mock<ILogger<SearchRequestService>>();
            _sut = new SearchRequestService(_odataClientMock.Object, _duplicateServiceMock.Object, _loggerMock.Object);
        }

        #region taxinfo testcases
        [Test]
        public async Task with_non_duplicated_person_CreateTaxIncomeInformation_should_return_new_SSG_TaxIncomeInformation()
        {


            _odataClientMock.Setup(x => x.For<SSG_TaxIncomeInformation>(null).Set(It.Is<TaxIncomeInformationEntity>(x => x.TaxYearResult == "1234"))
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_TaxIncomeInformation()
                {
                    TaxYearResult = "4321",
                    TaxincomeinformationId = _testTaxIncomeInformationId
                })
                );

            var taxinfo = new TaxIncomeInformationEntity()
            {
                TaxYearResult = "1234",
                EmploymentIncomeT4Amount = "2222",
                EmergencyVolunteerExemptIncomeAmount = "3333",
                Person = new SSG_Person() { PersonId = _testId }
            };

            var result = await _sut.CreateTaxIncomeInformation(taxinfo, CancellationToken.None);

            Assert.AreEqual("4321", result.TaxYearResult);
        }

        [Test]
        public async Task with_duplicated_person_not_contains_same_taxinfo_should_return_new_SSG_TaxIncomeInformation()
        {
            _odataClientMock.Setup(x => x.For<SSG_TaxIncomeInformation>(null).Set(It.Is<TaxIncomeInformationEntity>(x => x.TaxYearResult == "1234"))
                 .InsertEntryAsync(It.IsAny<CancellationToken>()))
                 .Returns(Task.FromResult(new SSG_TaxIncomeInformation()
                 {
                     TaxYearResult = "7890",
                     TaxincomeinformationId = _testTaxIncomeInformationId
                 })
                 );
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Person>(m => m.PersonId == _testId), It.Is<TaxIncomeInformationEntity>(m => m.TaxYearResult == "1234")))
                .Returns(Task.FromResult(Guid.Empty));

            var taxinfo = new TaxIncomeInformationEntity()
            {
                TaxYearResult = "1234",
                Person = new SSG_Person()
                {
                    PersonId = _testId,
                    SSG_TaxIncomeInformations = new List<SSG_TaxIncomeInformation>()
                    {
                        new SSG_TaxIncomeInformation(){ TaxincomeinformationId=_testTaxIncomeInformationId}
                    }.ToArray(),
                    IsDuplicated = true
                }
            };
            var result = await _sut.CreateTaxIncomeInformation(taxinfo, CancellationToken.None);

            Assert.AreEqual("7890", result.TaxYearResult);
            Assert.AreEqual(_testTaxIncomeInformationId, result.TaxincomeinformationId);
        }

        [Test]
        public async Task with_duplicated_person_contains_same_taxinfo_should_return_original_taxinfo_guid()
        {
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Person>(m => m.PersonId == _testId), It.Is<TaxIncomeInformationEntity>(m => m.TaxYearResult == "duplicated")))
                .Returns(Task.FromResult(_testTaxIncomeInformationId));

            var taxinfo = new TaxIncomeInformationEntity()
            {
                TaxYearResult = "duplicated",
                Person = new SSG_Person()
                {
                    PersonId = _testId,
                    SSG_TaxIncomeInformations = new List<SSG_TaxIncomeInformation>()
                    {
                        new SSG_TaxIncomeInformation(){TaxincomeinformationId=_testTaxIncomeInformationId}
                    }.ToArray(),
                    IsDuplicated = true
                }
            };
            var result = await _sut.CreateTaxIncomeInformation(taxinfo, CancellationToken.None);

            Assert.AreEqual(_testTaxIncomeInformationId, result.TaxincomeinformationId);
        }

        #endregion


    }
}
