using Fams3Adapter.Dynamics.Duplicate;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Moq;
using NUnit.Framework;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fams3Adapter.Dynamics.Test.SearchRequest
{
    public class SearchRequestService_Identifier_Test
    {
        private Mock<IODataClient> _odataClientMock;
        private Mock<IDuplicateDetectionService> _duplicateServiceMock;

        private readonly Guid _testId = Guid.Parse("6AE89FE6-9909-EA11-B813-00505683FBF4");
        private readonly Guid _testIdentifierId = Guid.Parse("6AE89FE6-9909-EA11-1111-00505683FBF4");
        private SearchRequestService _sut;

        [SetUp]
        public void SetUp()
        {
            _odataClientMock = new Mock<IODataClient>();
            _duplicateServiceMock = new Mock<IDuplicateDetectionService>();
            _sut = new SearchRequestService(_odataClientMock.Object, _duplicateServiceMock.Object);
        }

        #region identifier testcases
        [Test]
        public async Task with_non_duplicated_person_createIdentifier_should_return_new_SSG_Identifier()
        {
            _odataClientMock.Setup(x => x.For<SSG_Identifier>(null).Set(It.Is<IdentifierEntity>(x => x.Identification == "normal"))
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_Identifier()
                {
                    IdentifierId = _testIdentifierId
                })
                );
            var id = new IdentifierEntity()
            {
                Identification = "normal",
                Person = new SSG_Person() { IsDuplicated = false }
            };
            var result = await _sut.CreateIdentifier(id, CancellationToken.None);

            Assert.AreEqual(_testIdentifierId, result.IdentifierId);
        }

        [Test]
        public async Task with_duplicated_person_not_contains_same_identifier_should_return_new_SSG_Identifier()
        {
            _odataClientMock.Setup(x => x.For<SSG_Identifier>(null).Set(It.Is<IdentifierEntity>(x => x.Identification == "notContain"))
                 .InsertEntryAsync(It.IsAny<CancellationToken>()))
                 .Returns(Task.FromResult(new SSG_Identifier()
                 {
                     IdentifierId = _testIdentifierId
                 })
                 );
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Person>(m => m.PersonId == _testId), It.Is<IdentifierEntity>(m => m.Identification == "notContain")))
                .Returns(Task.FromResult(Guid.Empty));

            var id = new IdentifierEntity()
            {
                Identification = "notContain",
                Person = new SSG_Person()
                {
                    PersonId = _testId,
                    IsDuplicated = true
                }
            };
            var result = await _sut.CreateIdentifier(id, CancellationToken.None);

            Assert.AreEqual(_testIdentifierId, result.IdentifierId);
        }

        [Test]
        public async Task with_duplicated_person_contains_same_identifier_should_return_original_identifier_guid()
        {
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Person>(m => m.PersonId == _testId), It.Is<IdentifierEntity>(m => m.Identification == "contains")))
                .Returns(Task.FromResult(_testIdentifierId));

            var id = new IdentifierEntity()
            {
                Identification = "contains",
                Person = new SSG_Person()
                {
                    PersonId = _testId,
                    IsDuplicated = true
                }
            };
            var result = await _sut.CreateIdentifier(id, CancellationToken.None);

            Assert.AreEqual(_testIdentifierId, result.IdentifierId);
        }

        [Test]
        public async Task update_correct_Identifier_should_success()
        {
            Guid testId = Guid.NewGuid();
            _odataClientMock.Setup(x => x.For<SSG_Identifier>(null).Key(It.Is<Guid>(m => m == testId)).Set(It.IsAny<SSG_Identifier>())
                .UpdateEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_Identifier()
                {
                    IdentifierId = testId,
                    Identification = "new"
                })
                );

            IDictionary<string, object> updatedFields = new Dictionary<string, object> { { "ssg_identification", "new" } };

            var result = await _sut.UpdateIdentifier(testId, updatedFields, CancellationToken.None);

            Assert.AreEqual("new", result.Identification);

        }
        #endregion


    }
}
