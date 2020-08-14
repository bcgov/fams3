using Fams3Adapter.Dynamics.Agency;
using Fams3Adapter.Dynamics.Duplicate;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.ResultTransaction;
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
    public class SearchRequestService_SearchRequest_Test
    {
        private Mock<IODataClient> _odataClientMock;
        private Mock<IDuplicateDetectionService> _duplicateServiceMock;

        private readonly Guid testId = Guid.Parse("6AE89FE6-9909-EA11-B813-00505683FBF4");
        private readonly Guid testPersonId = Guid.Parse("6AE89FE6-9909-EA11-1111-00505683FBF4");
        private readonly Guid testAssetOtherId = Guid.Parse("77789FE6-9909-EA11-1901-000056837777");

        private SearchRequestService _sut;

        [SetUp]
        public void SetUp()
        {
            _odataClientMock = new Mock<IODataClient>();
            _duplicateServiceMock = new Mock<IDuplicateDetectionService>();
            _sut = new SearchRequestService(_odataClientMock.Object, _duplicateServiceMock.Object);
        }

        [Test]
        public async Task upload_correct_SearchRequest_should_success()
        {
            _odataClientMock.Setup(x => x.For<SSG_Agency>(null)
                  .Filter(It.IsAny<Expression<Func<SSG_Agency, bool>>>())
                  .FindEntryAsync(It.IsAny<CancellationToken>()))
                  .Returns(Task.FromResult<SSG_Agency>(new SSG_Agency()
                  {
                      AgencyId = Guid.NewGuid(),
                      AgencyCode = "fmep"
                  }));

            _odataClientMock.Setup(x => x.For<SSG_SearchRequestReason>(null)
                 .Filter(It.IsAny<Expression<Func<SSG_SearchRequestReason, bool>>>())
                 .FindEntryAsync(It.IsAny<CancellationToken>()))
                 .Returns(Task.FromResult<SSG_SearchRequestReason>(new SSG_SearchRequestReason()
                 {
                     ReasonId = Guid.NewGuid(),
                     ReasonCode = "reasonCode"
                 }));

            _odataClientMock.Setup(x => x.For<SSG_AgencyLocation>(null)
                 .Filter(It.IsAny<Expression<Func<SSG_AgencyLocation, bool>>>())
                 .FindEntryAsync(It.IsAny<CancellationToken>()))
                 .Returns(Task.FromResult<SSG_AgencyLocation>(new SSG_AgencyLocation()
                 {
                     AgencyLocationId = Guid.NewGuid(),
                     City = "city"
                 }));

            _odataClientMock.Setup(x => x.For<SSG_SearchRequest>(null).Set(It.IsAny<SearchRequestEntity>())
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_SearchRequest()
                {
                    SearchRequestId = testId
                })
                );

            var searchRequest = new SearchRequestEntity()
            {
                AgencyCode = "fmep",
                SearchReasonCode = "reason",
                AgencyOfficeLocationText = "NORTHERN AND INTERIOR CLIENT OFFICE, KAMLOOPS, BC",
                AgentFirstName = "agentName"
            };
            var result = await _sut.CreateSearchRequest(searchRequest, CancellationToken.None);

            Assert.AreEqual(testId, result.SearchRequestId);
        }

        [Test]
        public void upload_invalid_agencycode_SearchRequest_should_throw_exception()
        {
            _odataClientMock.Setup(x => x.For<SSG_Agency>(null)
                  .Filter(It.IsAny<Expression<Func<SSG_Agency, bool>>>())
                  .FindEntryAsync(It.IsAny<CancellationToken>()))
                  .Throws(WebRequestException.CreateFromStatusCode(
                    System.Net.HttpStatusCode.NotFound,
                    new WebRequestExceptionMessageSource(),
                    ""
                    ));

            var searchRequest = new SearchRequestEntity()
            {
                AgencyCode = "wrong",
                SearchReasonCode = "reason",
                AgencyOfficeLocationText = "NORTHERN AND INTERIOR CLIENT OFFICE, KAMLOOPS, BC",
                AgentFirstName = "agentName"
            };
            Assert.ThrowsAsync<WebRequestException>(async () => await _sut.CreateSearchRequest(searchRequest, CancellationToken.None));
        }

        [Test]
        public void upload_invalid_searchReason_SearchRequest_should_throw_exception()
        {
            _odataClientMock.Setup(x => x.For<SSG_Agency>(null)
              .Filter(It.IsAny<Expression<Func<SSG_Agency, bool>>>())
              .FindEntryAsync(It.IsAny<CancellationToken>()))
              .Returns(Task.FromResult<SSG_Agency>(new SSG_Agency()
              {
                  AgencyId = Guid.NewGuid(),
                  AgencyCode = "fmep"
              }));

            _odataClientMock.Setup(x => x.For<SSG_SearchRequestReason>(null)
              .Filter(It.IsAny<Expression<Func<SSG_SearchRequestReason, bool>>>())
              .FindEntryAsync(It.IsAny<CancellationToken>()))
              .Throws(WebRequestException.CreateFromStatusCode(
                System.Net.HttpStatusCode.NotFound,
                new WebRequestExceptionMessageSource(),
                ""
                ));

            var searchRequest = new SearchRequestEntity()
            {
                AgencyCode = "fmep",
                SearchReasonCode = "wrongreasoncode",
                AgencyOfficeLocationText = "NORTHERN AND INTERIOR CLIENT OFFICE, KAMLOOPS, BC",
                AgentFirstName = "agentName"
            };

            Assert.ThrowsAsync<WebRequestException>(async () => await _sut.CreateSearchRequest(searchRequest, CancellationToken.None));
        }

        [Test]
        public async Task upload_invalid_locationtext_SearchRequest_should_upload_locationText()
        {
            _odataClientMock.Setup(x => x.For<SSG_Agency>(null)
               .Filter(It.IsAny<Expression<Func<SSG_Agency, bool>>>())
               .FindEntryAsync(It.IsAny<CancellationToken>()))
               .Returns(Task.FromResult<SSG_Agency>(new SSG_Agency()
               {
                   AgencyId = Guid.NewGuid(),
                   AgencyCode = "fmep"
               }));

            _odataClientMock.Setup(x => x.For<SSG_SearchRequestReason>(null)
                 .Filter(It.IsAny<Expression<Func<SSG_SearchRequestReason, bool>>>())
                 .FindEntryAsync(It.IsAny<CancellationToken>()))
                 .Returns(Task.FromResult<SSG_SearchRequestReason>(new SSG_SearchRequestReason()
                 {
                     ReasonId = Guid.NewGuid(),
                     ReasonCode = "reasonCode"
                 }));

            _odataClientMock.Setup(x => x.For<SSG_AgencyLocation>(null)
              .Filter(It.IsAny<Expression<Func<SSG_AgencyLocation, bool>>>())
              .FindEntryAsync(It.IsAny<CancellationToken>()))
              .Throws(WebRequestException.CreateFromStatusCode(
                System.Net.HttpStatusCode.NotFound,
                new WebRequestExceptionMessageSource(),
                ""
                ));

            _odataClientMock.Setup(x => x.For<SSG_SearchRequest>(null).Set(It.IsAny<SearchRequestEntity>())
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_SearchRequest()
                {
                    SearchRequestId = testId,
                })
                );

            var searchRequest = new SearchRequestEntity()
            {
                AgencyCode = "fmep",
                SearchReasonCode = "reason",
                AgencyOfficeLocationText = "WRONG ADDRESS,BC",
                AgentFirstName = "agentName"
            };

            var result = await _sut.CreateSearchRequest(searchRequest, CancellationToken.None);
            _odataClientMock.Verify(x => x.For<SSG_SearchRequest>(It.IsAny<string>())
             .Set(It.Is<SearchRequestEntity>(m => m.AgencyOfficeLocationText == "WRONG ADDRESS,BC"))
             .InsertEntryAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task upload_valid_locationtext_SearchRequest_should_upload_mapped_location_not_locationText()
        {
            _odataClientMock.Setup(x => x.For<SSG_Agency>(null)
                .Filter(It.IsAny<Expression<Func<SSG_Agency, bool>>>())
                .FindEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Agency>(new SSG_Agency()
                {
                    AgencyId = Guid.NewGuid(),
                    AgencyCode = "fmep"
                }));

            _odataClientMock.Setup(x => x.For<SSG_SearchRequestReason>(null)
                 .Filter(It.IsAny<Expression<Func<SSG_SearchRequestReason, bool>>>())
                 .FindEntryAsync(It.IsAny<CancellationToken>()))
                 .Returns(Task.FromResult<SSG_SearchRequestReason>(new SSG_SearchRequestReason()
                 {
                     ReasonId = Guid.NewGuid(),
                     ReasonCode = "reasonCode"
                 }));

            _odataClientMock.Setup(x => x.For<SSG_AgencyLocation>(null)
                 .Filter(It.IsAny<Expression<Func<SSG_AgencyLocation, bool>>>())
                 .FindEntryAsync(It.IsAny<CancellationToken>()))
                 .Returns(Task.FromResult<SSG_AgencyLocation>(new SSG_AgencyLocation()
                 {
                     AgencyLocationId = Guid.NewGuid(),
                     City = "city"
                 }));

            _odataClientMock.Setup(x => x.For<SSG_SearchRequest>(null).Set(It.IsAny<SearchRequestEntity>())
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_SearchRequest()
                {
                    SearchRequestId = testId,
                })
                );

            var searchRequest = new SearchRequestEntity()
            {
                AgencyCode = "fmep",
                SearchReasonCode = "reason",
                AgencyOfficeLocationText = "NORTHERN AND INTERIOR CLIENT OFFICE, KAMLOOPS, BC",
                AgentFirstName = "agentName"
            };

            var result = await _sut.CreateSearchRequest(searchRequest, CancellationToken.None);
            _odataClientMock.Verify(x => x.For<SSG_SearchRequest>(It.IsAny<string>())
                .Set(It.Is<SearchRequestEntity>(m => m.AgencyOfficeLocationText == null))
                .InsertEntryAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task cancel_correct_SearchRequest_should_success()
        {
            Guid searchRequestId = Guid.NewGuid();
            _odataClientMock.Setup(x => x.For<SSG_SearchRequest>(null)
                  .Filter(It.IsAny<Expression<Func<SSG_SearchRequest, bool>>>())
                  .FindEntryAsync(It.IsAny<CancellationToken>()))
                  .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
                  {
                      SearchRequestId = searchRequestId
                  }));

            _odataClientMock.Setup(x => x.For<SSG_SearchRequest>(null)
                .Key(searchRequestId)
                .Set(new Dictionary<string, object>() { { Keys.DYNAMICS_STATUS_CODE_FIELD, SearchRequestStatusCode.AgencyCancelled.Value } })
                .UpdateEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
                {
                    StatusCode = SearchRequestStatusCode.AgencyCancelled.Value
                }));


            var result = await _sut.CancelSearchRequest("fileId", CancellationToken.None);

            Assert.AreEqual(SearchRequestStatusCode.AgencyCancelled.Value, result.StatusCode);
        }

        [Test]
        public async Task cancel_non_exist_SearchRequest_should_return_null()
        {
            _odataClientMock.Setup(x => x.For<SSG_SearchRequest>(null)
                  .Filter(It.IsAny<Expression<Func<SSG_SearchRequest, bool>>>())
                  .FindEntryAsync(It.IsAny<CancellationToken>()))
                  .Returns(Task.FromResult<SSG_SearchRequest>(null));

            var result = await _sut.CancelSearchRequest("fileId", CancellationToken.None);

            Assert.AreEqual(null, result);
        }

        [Test]
        public void exception_cancel_SearchRequest_should_throw_exception()
        {
            Guid searchRequestId = Guid.NewGuid();
            _odataClientMock.Setup(x => x.For<SSG_SearchRequest>(null)
                  .Filter(It.IsAny<Expression<Func<SSG_SearchRequest, bool>>>())
                  .FindEntryAsync(It.IsAny<CancellationToken>()))
                  .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
                  {
                      SearchRequestId = searchRequestId
                  }));

            _odataClientMock.Setup(x => x.For<SSG_SearchRequest>(null)
                .Key(searchRequestId)
                .Set(new Dictionary<string, object>() { { Keys.DYNAMICS_STATUS_CODE_FIELD, SearchRequestStatusCode.AgencyCancelled.Value } })
                .UpdateEntryAsync(It.IsAny<CancellationToken>()))
                .Throws(WebRequestException.CreateFromStatusCode(
                        System.Net.HttpStatusCode.NotFound,
                        new WebRequestExceptionMessageSource(),
                        ""
                        ));

            Assert.ThrowsAsync<WebRequestException>(async () => await _sut.CancelSearchRequest("fileId", CancellationToken.None));
        }

        [Test]
        public async Task update_correct_SearchRequest_should_success()
        {
            _odataClientMock.Setup(x => x.For<SSG_Agency>(null)
               .Filter(It.IsAny<Expression<Func<SSG_Agency, bool>>>())
               .FindEntryAsync(It.IsAny<CancellationToken>()))
               .Returns(Task.FromResult<SSG_Agency>(new SSG_Agency()
               {
                   AgencyId = Guid.NewGuid(),
                   AgencyCode = "fmep"
               }));

            _odataClientMock.Setup(x => x.For<SSG_SearchRequestReason>(null)
                 .Filter(It.IsAny<Expression<Func<SSG_SearchRequestReason, bool>>>())
                 .FindEntryAsync(It.IsAny<CancellationToken>()))
                 .Returns(Task.FromResult<SSG_SearchRequestReason>(new SSG_SearchRequestReason()
                 {
                     ReasonId = Guid.NewGuid(),
                     ReasonCode = "reasonCode"
                 }));

            _odataClientMock.Setup(x => x.For<SSG_AgencyLocation>(null)
                 .Filter(It.IsAny<Expression<Func<SSG_AgencyLocation, bool>>>())
                 .FindEntryAsync(It.IsAny<CancellationToken>()))
                 .Returns(Task.FromResult<SSG_AgencyLocation>(new SSG_AgencyLocation()
                 {
                     AgencyLocationId = Guid.NewGuid(),
                     City = "city"
                 }));

            _odataClientMock.Setup(x => x.For<SSG_SearchRequest>(null).Key(It.Is<Guid>(m=>m==testId)).Set(It.IsAny<SSG_SearchRequest>())
                .UpdateEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_SearchRequest()
                {
                    SearchRequestId = testId
                })
                );

            var searchRequest = new SSG_SearchRequest()
            {
                AgencyCode = "fmep",
                SearchRequestId=testId
            };
            var result = await _sut.UpdateSearchRequest(searchRequest, CancellationToken.None);

            Assert.AreEqual(testId, result.SearchRequestId);

        }

        [Test]
        public void exception_UpdateSearchRequest_should_throw_exception()
        {
            _odataClientMock.Setup(x => x.For<SSG_Agency>(null)
               .Filter(It.IsAny<Expression<Func<SSG_Agency, bool>>>())
               .FindEntryAsync(It.IsAny<CancellationToken>()))
               .Returns(Task.FromResult<SSG_Agency>(new SSG_Agency()
               {
                   AgencyId = Guid.NewGuid(),
                   AgencyCode = "fmep"
               }));

            _odataClientMock.Setup(x => x.For<SSG_SearchRequestReason>(null)
                 .Filter(It.IsAny<Expression<Func<SSG_SearchRequestReason, bool>>>())
                 .FindEntryAsync(It.IsAny<CancellationToken>()))
                 .Returns(Task.FromResult<SSG_SearchRequestReason>(new SSG_SearchRequestReason()
                 {
                     ReasonId = Guid.NewGuid(),
                     ReasonCode = "reasonCode"
                 }));

            _odataClientMock.Setup(x => x.For<SSG_AgencyLocation>(null)
                 .Filter(It.IsAny<Expression<Func<SSG_AgencyLocation, bool>>>())
                 .FindEntryAsync(It.IsAny<CancellationToken>()))
                 .Returns(Task.FromResult<SSG_AgencyLocation>(new SSG_AgencyLocation()
                 {
                     AgencyLocationId = Guid.NewGuid(),
                     City = "city"
                 }));

            _odataClientMock.Setup(x => x.For<SSG_SearchRequest>(null).Key(It.Is<Guid>(m => m == testId)).Set(It.IsAny<SSG_SearchRequest>())
                .UpdateEntryAsync(It.IsAny<CancellationToken>()))
                .Throws(WebRequestException.CreateFromStatusCode(
                        System.Net.HttpStatusCode.NotFound,
                        new WebRequestExceptionMessageSource(),
                        ""
                        ));

            var searchRequest = new SSG_SearchRequest()
            {
                AgencyCode = "fmep",
                SearchRequestId = testId
            };
            Assert.ThrowsAsync<WebRequestException>(async () => await _sut.UpdateSearchRequest(searchRequest, CancellationToken.None));
        }
    }
}
