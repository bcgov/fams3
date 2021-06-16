using Fams3Adapter.Dynamics.Duplicate;
using Fams3Adapter.Dynamics.Email;
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
    public class SearchRequestService_Email_Test
    {
        private Mock<IODataClient> _odataClientMock;
        private Mock<IDuplicateDetectionService> _duplicateServiceMock;
        private Mock<ILogger<SearchRequestService>> _loggerMock;

        private readonly Guid _testId = Guid.Parse("6AE89FE6-9909-EA11-B813-00505683FBF4");
        private Guid _testEmailId;
        private SearchRequestService _sut;

        [SetUp]
        public void SetUp()
        {
            _odataClientMock = new Mock<IODataClient>();
            _duplicateServiceMock = new Mock<IDuplicateDetectionService>();
            _loggerMock = new Mock<ILogger<SearchRequestService>>();
            _sut = new SearchRequestService(_odataClientMock.Object, _duplicateServiceMock.Object, _loggerMock.Object);
        }

        #region email testcases
        [Test]
        public async Task with_non_duplicated_person_CreateEmail_should_return_new_SSG_Email()
        {
            _testEmailId = Guid.NewGuid();
            _odataClientMock.Setup(x => x.For<SSG_Email>(null).Set(It.Is<EmailEntity>(x => x.Email == "normal"))
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_Email()
                {
                    Email = "normal",
                    EmailId = _testEmailId
                })
                );
            var emailEntity = new EmailEntity()
            {
                Email = "normal",
                Person = new SSG_Person() { PersonId = _testId }
            };

            var result = await _sut.CreateEmail(emailEntity, CancellationToken.None);

            Assert.AreEqual("normal", result.Email);
            Assert.AreEqual(_testEmailId, result.EmailId);
        }

        [Test]
        public async Task with_duplicated_person_not_contains_same_email_should_return_new_SSG_Email()
        {
            _testEmailId = Guid.NewGuid();
            _odataClientMock.Setup(x => x.For<SSG_Email>(null).Set(It.Is<EmailEntity>(x => x.Email == "notContain"))
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_Email()
                {
                    Email = "notContain",
                    EmailId = _testEmailId
                })
                );
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Person>(m => m.PersonId == _testId), It.Is<EmailEntity>(m => m.Email == "notContain")))
                .Returns(Task.FromResult(Guid.Empty));

            var emailEntity = new EmailEntity()
            {
                Email = "notContain",
                Person = new SSG_Person() { PersonId = _testId, IsDuplicated = true }
            };

            var result = await _sut.CreateEmail(emailEntity, CancellationToken.None);

            Assert.AreEqual("notContain", result.Email);
            Assert.AreEqual(_testEmailId, result.EmailId);
        }

        [Test]
        public async Task with_duplicated_person_contains_same_email_should_return_original_email_guid()
        {
            _testEmailId = Guid.NewGuid();
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Person>(m => m.PersonId == _testId), It.Is<EmailEntity>(m => m.Email == "contain")))
                .Returns(Task.FromResult(_testEmailId));

            var emailEntity = new EmailEntity()
            {
                Email = "contain",
                Person = new SSG_Person() { PersonId = _testId, IsDuplicated = true }
            };

            var result = await _sut.CreateEmail(emailEntity, CancellationToken.None);

            Assert.AreEqual(_testEmailId, result.EmailId);
        }


        #endregion


    }
}
