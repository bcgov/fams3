using Fams3Adapter.Dynamics.Duplicate;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
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
    public class SearchRequestService_Person_Test
    {
        private Mock<IODataClient> _odataClientMock;
        private Mock<IDuplicateDetectionService> _duplicateServiceMock;

        private readonly Guid _testId = Guid.Parse("6AE89FE6-9909-EA11-B813-00505683FBF4");
        private readonly Guid _testPersonId = Guid.Parse("6AE89FE6-9909-EA11-1111-00505683FBF4");
        private SearchRequestService _sut;

        [SetUp]
        public void SetUp()
        {
            _odataClientMock = new Mock<IODataClient>();
            _duplicateServiceMock = new Mock<IDuplicateDetectionService>();
            _sut = new SearchRequestService(_odataClientMock.Object, _duplicateServiceMock.Object);
        }

        #region person testcases
        [Test]
        public async Task with_correct_non_duplicate_person_upload_person_should_success()
        {
            //person normal
            _duplicateServiceMock.Setup(x => x.GetDuplicateDetectHashData(It.Is<PersonEntity>(m => m.FirstName == "First")))
                .Returns(
                    Task.FromResult("normalPersonHashdata")
                );

            _odataClientMock.Setup(x => x.For<SSG_Person>(null)
                .Filter(It.IsAny<Expression<Func<SSG_Person, bool>>>())
                .FindEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Person>(null));

            _odataClientMock.Setup(x => x.For<SSG_Person>(null).Set(It.Is<PersonEntity>(x => x.FirstName == "First"))
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_Person()
                {
                    FirstName = "FirstName",
                    PersonId = _testPersonId
                })
                );

            var person = new PersonEntity()
            {
                FirstName = "First",
                LastName = "lastName",
                MiddleName = "middleName",
                ThirdGivenName = "Third",
                DateOfBirth = null,
                DateOfDeath = null,
                DateOfDeathConfirmed = false,
                Incacerated = 86000071,
                StateCode = 0,
                StatusCode = 1,
                SearchRequest = new SSG_SearchRequest() { SearchRequestId = _testId }
            };

            var result = await _sut.SavePerson(person, CancellationToken.None);

            Assert.AreEqual("FirstName", result.FirstName);
            Assert.AreEqual(_testPersonId, result.PersonId);
        }

        [Test]
        public async Task with_duplicated_upload_person_should_return_original_person_guid()
        {
            _duplicateServiceMock.Setup(x => x.GetDuplicateDetectHashData(It.Is<PersonEntity>(m => m.FirstName == "Duplicated")))
                .Returns(
                    Task.FromResult("duplicatedPersonHashdata")
                );

            _odataClientMock.Setup(x => x.For<SSG_Person>(null)
                .Filter(It.IsAny<Expression<Func<SSG_Person, bool>>>())
                .FindEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_Person()
                {
                    FirstName = "FirstName",
                    PersonId = _testPersonId
                }));

            _odataClientMock.Setup(x => x.For<SSG_Person>(null)
                 .Key(It.Is<Guid>(m => m == _testPersonId))
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
                 .FindEntryAsync(It.IsAny<CancellationToken>()))
                 .Returns(Task.FromResult(new SSG_Person()
                 {
                     FirstName = "FirstName",
                     PersonId = _testPersonId
                 }));

            var person = new PersonEntity()
            {
                FirstName = "Duplicated",
                SearchRequest = new SSG_SearchRequest() { SearchRequestId = _testId }
            };

            var result = await _sut.SavePerson(person, CancellationToken.None);

            Assert.AreEqual("FirstName", result.FirstName);
            Assert.AreEqual(_testPersonId, result.PersonId);
            Assert.AreEqual(true, result.IsDuplicated);
        }

        [Test]
        public void With_Exception_SavePerson_should_throw_it()
        {
            //person - throw non-duplicated-exception
            _duplicateServiceMock.Setup(x => x.GetDuplicateDetectHashData(It.Is<PersonEntity>(m => m.FirstName == "OtherException")))
                .Returns(
                    Task.FromResult("exceptionDuplicatedPersonHashdata")
                );

            _odataClientMock.Setup(x => x.For<SSG_Person>(null)
                .Filter(It.IsAny<Expression<Func<SSG_Person, bool>>>())
                .FindEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Person>(null));

            _odataClientMock.Setup(x => x.For<SSG_Person>(null).Set(It.Is<PersonEntity>(x => x.DuplicateDetectHash == "exceptionDuplicatedPersonHashdata"))
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Throws(WebRequestException.CreateFromStatusCode(
                    System.Net.HttpStatusCode.BadRequest,
                    new WebRequestExceptionMessageSource(),
                    ""
                    ));

            var person = new PersonEntity()
            {
                FirstName = "OtherException",
                SearchRequest = new SSG_SearchRequest() { SearchRequestId = _testId }
            };

            Assert.ThrowsAsync<WebRequestException>(async () => await _sut.SavePerson(person, CancellationToken.None));
        }

        [Test]
        public async Task GetPerson_should_return_1_level_expanded_data()
        {
            Guid personId = Guid.NewGuid();

            _odataClientMock.Setup(x => x.For<SSG_Person>(null)
                .Key(It.Is<Guid>(m => m == personId))
                .Expand(x => x.SSG_Identities)
                .Expand(x => x.SSG_PhoneNumbers)
                .Expand(x => x.SSG_Identifiers)
                .Expand(x => x.SSG_Employments)
                .Expand(x => x.SSG_Addresses)
                .Expand(x => x.SSG_Aliases)
                .Expand(x => x.sSG_SafetyConcernDetails)
                .FindEntryAsync(It.IsAny<CancellationToken>()))
               .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
               {
                   PersonId = personId,
                   SSG_Identifiers = new List<SSG_Identifier>() { new SSG_Identifier() { } }.ToArray()
               }));
            var result = await _sut.GetPerson(personId, CancellationToken.None);
            Assert.AreEqual(1, result.SSG_Identifiers.Length);
        }

        [Test]
        public async Task update_correct_Person_should_success()
        {
            Guid testId = Guid.NewGuid();
            _odataClientMock.Setup(x => x.For<SSG_Person>(null).Key(It.Is<Guid>(m => m == testId)).Set(It.IsAny<Dictionary<string, object>>())
                .UpdateEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_Person()
                {
                    PersonId = testId,
                    FirstName = "new"
                })
                );
            IDictionary<string, object> updatedFields = new Dictionary<string, object> { { "ssg_firstname", "new" } };
            var person = new SSG_Person()
            {
                PersonId = testId,
                FirstName = "old"
            };
            var result = await _sut.UpdatePerson(testId, updatedFields, person, CancellationToken.None);

            Assert.AreEqual("new", result.FirstName);

        }
        #endregion


    }
}
