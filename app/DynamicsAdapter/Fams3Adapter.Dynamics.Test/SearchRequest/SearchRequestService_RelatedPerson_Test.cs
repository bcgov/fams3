using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.Duplicate;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.RelatedPerson;
using Fams3Adapter.Dynamics.SearchRequest;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fams3Adapter.Dynamics.Test.SearchRequest
{
    public class SearchRequestService_RelatedPerson_Test
    {
        private Mock<IODataClient> _odataClientMock;
        private Mock<IDuplicateDetectionService> _duplicateServiceMock;
        private Mock<ILogger<SearchRequestService>> _loggerMock;

        private readonly Guid _testId = Guid.Parse("6AE89FE6-9909-EA11-B813-00505683FBF4");
        private Guid _testRelatedPersonId;
        private SearchRequestService _sut;

        [SetUp]
        public void SetUp()
        {
            _odataClientMock = new Mock<IODataClient>();
            _duplicateServiceMock = new Mock<IDuplicateDetectionService>();
            _loggerMock = new Mock<ILogger<SearchRequestService>>();
            _sut = new SearchRequestService(_odataClientMock.Object, _duplicateServiceMock.Object, _loggerMock.Object);
        }

        #region relatedperson testcases
        [Test]
        public async Task with_non_duplicated_person_CreateRelatedPerson_should_return_new_SSG_RelatedPerson()
        {
            _testRelatedPersonId = Guid.NewGuid();
            _odataClientMock.Setup(x => x.For<SSG_Identity>(null).Set(It.Is<RelatedPersonEntity>(x => x.FirstName == "normal"))
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_Identity()
                {
                    FirstName = "normal",
                    RelatedPersonId = _testRelatedPersonId
                })
                );
            var relatedPerson = new RelatedPersonEntity()
            {
                FirstName = "normal",
                Person = new SSG_Person() { PersonId = _testId }
            };

            var result = await _sut.CreateRelatedPerson(relatedPerson, CancellationToken.None);

            Assert.AreEqual("normal", result.FirstName);
            Assert.AreEqual(_testRelatedPersonId, result.RelatedPersonId);
        }

        [Test]
        public async Task with_duplicated_person_not_contains_same_relatedPerson_should_return_new_SSG_RelatedPerson()
        {
            _testRelatedPersonId = Guid.NewGuid();
            _odataClientMock.Setup(x => x.For<SSG_Identity>(null).Set(It.Is<RelatedPersonEntity>(x => x.FirstName == "notContain"))
                 .InsertEntryAsync(It.IsAny<CancellationToken>()))
                 .Returns(Task.FromResult(new SSG_Identity()
                 {
                     FirstName = "notContain",
                     RelatedPersonId = _testRelatedPersonId
                 })
                 );
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Person>(m => m.PersonId == _testId), It.Is<RelatedPersonEntity>(m => m.FirstName == "notContain")))
                .Returns(Task.FromResult(Guid.Empty));

            var relatedPerson = new RelatedPersonEntity()
            {
                FirstName = "notContain",
                Person = new SSG_Person()
                {
                    PersonId = _testId,
                    IsDuplicated = true
                }
            };
            var result = await _sut.CreateRelatedPerson(relatedPerson, CancellationToken.None);

            Assert.AreEqual("notContain", result.FirstName);
            Assert.AreEqual(_testRelatedPersonId, result.RelatedPersonId);
        }

        [Test]
        public async Task with_duplicated_person_contains_same_relatedPerson_should_return_original_relatedPerson_guid()
        {
            _testRelatedPersonId = Guid.NewGuid();
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Person>(m => m.PersonId == _testId), It.Is<RelatedPersonEntity>(m => m.FirstName == "contain")))
                .Returns(Task.FromResult(_testRelatedPersonId));

            var relatedPerson = new RelatedPersonEntity()
            {
                FirstName = "contain",
                Person = new SSG_Person()
                {
                    PersonId = _testId,
                    IsDuplicated = true
                }
            };
            var result = await _sut.CreateRelatedPerson(relatedPerson, CancellationToken.None);

            Assert.AreEqual(_testRelatedPersonId, result.RelatedPersonId);
        }

        [Test]
        public async Task update_correct_relatedPerson_should_success()
        {
            Guid testId = Guid.NewGuid();
            _odataClientMock.Setup(x => x.For<SSG_Identity>(null).Key(It.Is<Guid>(m => m == testId)).Set(It.IsAny<Dictionary<string, object>>())
                .UpdateEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_Identity()
                {
                    RelatedPersonId = testId,
                    FirstName = "new"
                })
                );

            var relatedPerson = new SSG_Identity()
            {
                RelatedPersonId = testId,
                FirstName = "old"
            };
            IDictionary<string, object> updatedFields = new Dictionary<string, object> { { "businessname", "new" } };
            var result = await _sut.UpdateRelatedPerson(testId, updatedFields, CancellationToken.None);

            Assert.AreEqual("new", result.FirstName);

        }
        #endregion


    }
}
