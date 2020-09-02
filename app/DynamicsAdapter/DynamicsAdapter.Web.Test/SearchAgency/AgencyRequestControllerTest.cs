using DynamicsAdapter.Web.SearchAgency;
using DynamicsAdapter.Web.SearchAgency.Models;
using Fams3Adapter.Dynamics.SearchRequest;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.Test.SearchAgency
{
    public class AgencyRequestControllerTest
    {
        private AgencyRequestController _sut;
        private Mock<ILogger<AgencyRequestController>> _loggerMock;
        private Mock<IAgencyRequestService> _agencyRequestServiceMock;

        [SetUp]
        public void Init()
        {
            _loggerMock = new Mock<ILogger<AgencyRequestController>>();
            _agencyRequestServiceMock = new Mock<IAgencyRequestService>();

            _sut = new AgencyRequestController(_loggerMock.Object, _agencyRequestServiceMock.Object);
        }

        [Test]
        public async Task With_valid_searchRequestOrdered_CreateSearchRequest_should_return_ok_with_correct_content()
        {
            SearchRequestOrdered validSearchRequestOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.NEW,
                RequestId = "121212121212",
                TimeStamp = DateTime.Now,
                Person = new Person()
                {
                    Agency = new Agency()
                    {
                        RequestId = "121212121212",
                        Code = "FMEP"
                    }
                }
            };

            _agencyRequestServiceMock.Setup(x => x.ProcessSearchRequestOrdered(It.IsAny<SearchRequestOrdered>()))
                .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
                {
                    FileId = "fileId",
                    SearchRequestId = Guid.NewGuid()
                }));

            var result = await _sut.CreateSearchRequest("normalsearchRequest", validSearchRequestOrdered);

            _agencyRequestServiceMock.Verify(x => x.ProcessSearchRequestOrdered(It.IsAny<SearchRequestOrdered>()), Times.Once);
            var resultValue = result as OkObjectResult;
            Assert.NotNull(resultValue);
            var saved = resultValue.Value as SearchRequestSaved;
            Assert.NotNull(saved);
            Assert.AreEqual("fileId", saved.SearchRequestKey);
            Assert.AreEqual("FMEP", saved.ProviderProfile.Name);
            Assert.AreEqual("The new Search Request reference: 121212121212 has been submitted successfully.", saved.Message);
        }

        [Test]
        public async Task With_null_key_searchRequestOrdered_CreateSearchRequest_should_return_BadRequest()
        {
            SearchRequestOrdered validSearchRequestOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.NEW,
                RequestId = Guid.NewGuid().ToString(),
                TimeStamp = DateTime.Now,
            };
            var result = await _sut.CreateSearchRequest(null, validSearchRequestOrdered);

            _agencyRequestServiceMock.Verify(x => x.ProcessSearchRequestOrdered(It.IsAny<SearchRequestOrdered>()), Times.Never);
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task With_update_action_searchRequestOrdered_CreateSearchRequest_should_return_BadRequest()
        {
            SearchRequestOrdered updateSearchRequestOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.UPDATE,
                RequestId = Guid.NewGuid().ToString(),
                TimeStamp = DateTime.Now,
            };
            var result = await _sut.CreateSearchRequest("requestId", updateSearchRequestOrdered);

            _agencyRequestServiceMock.Verify(x => x.ProcessSearchRequestOrdered(It.IsAny<SearchRequestOrdered>()), Times.Never);
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task With_exception_throws_searchRequestOrdered_CreateSearchRequest_should_return_InternalServerError()
        {
            string requestId = "exception";
            SearchRequestOrdered updateSearchRequestOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.NEW,
                RequestId = requestId,
                TimeStamp = DateTime.Now,
            };

            _agencyRequestServiceMock.Setup(
                x => x.ProcessSearchRequestOrdered(It.Is<SearchRequestOrdered>(x => x.RequestId == requestId)))
            .Throws(new Exception("exception throws"));

            var result = await _sut.CreateSearchRequest("exceptionrequest", updateSearchRequestOrdered);
            _agencyRequestServiceMock.Verify(x => x.ProcessSearchRequestOrdered(It.IsAny<SearchRequestOrdered>()), Times.Once);
            Assert.AreEqual(500, ((ObjectResult)result).StatusCode);
        }

        [Test]
        public async Task With_valid_searchRequestOrdered_CancelSearchRequest_should_return_ok_with_correct_content()
        {
            SearchRequestOrdered validSearchRequestOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.CANCEL,
                RequestId = "121212121212",
                SearchRequestKey = "fileId",
                TimeStamp = DateTime.Now,
                Person = new Person()
                {
                    Agency = new Agency()
                    {
                        RequestId = "121212121212",
                        Code = "FMEP"
                    }
                }
            };

            _agencyRequestServiceMock.Setup(x => x.ProcessCancelSearchRequest(It.IsAny<SearchRequestOrdered>()))
                .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
                {
                    FileId = "fileId",
                    SearchRequestId = Guid.NewGuid(),
                    StatusCode = SearchRequestStatusCode.AgencyCancelled.Value
                }));

            var result = await _sut.CancelSearchRequest(validSearchRequestOrdered);

            _agencyRequestServiceMock.Verify(x => x.ProcessCancelSearchRequest(It.IsAny<SearchRequestOrdered>()), Times.Once);
            var resultValue = result as OkObjectResult;
            Assert.NotNull(resultValue);
            var saved = resultValue.Value as SearchRequestSaved;
            Assert.NotNull(saved);
            Assert.AreEqual("fileId", saved.SearchRequestKey);
            Assert.AreEqual("FMEP", saved.ProviderProfile.Name);
            Assert.AreEqual("The Search Request fileId has been cancelled successfully upon the request 121212121212.", saved.Message);
        }

        [Test]
        public async Task With_null_key_searchRequestOrdered_CancelSearchRequest_should_return_BadRequest()
        {
            SearchRequestOrdered validSearchRequestOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.NEW,
                RequestId = Guid.NewGuid().ToString(),
                TimeStamp = DateTime.Now,
            };
            var result = await _sut.CancelSearchRequest(validSearchRequestOrdered);

            _agencyRequestServiceMock.Verify(x => x.ProcessSearchRequestOrdered(It.IsAny<SearchRequestOrdered>()), Times.Never);
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task With_update_action_searchRequestOrdered_CancelSearchRequest_should_return_BadRequest()
        {
            SearchRequestOrdered updateSearchRequestOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.UPDATE,
                RequestId = Guid.NewGuid().ToString(),
                TimeStamp = DateTime.Now,
            };
            var result = await _sut.CancelSearchRequest(updateSearchRequestOrdered);

            _agencyRequestServiceMock.Verify(x => x.ProcessSearchRequestOrdered(It.IsAny<SearchRequestOrdered>()), Times.Never);
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task With_empty_fileID_searchRequestOrdered_CancelSearchRequest_should_return_BadRequest()
        {
            SearchRequestOrdered updateSearchRequestOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.CANCEL,
                RequestId = Guid.NewGuid().ToString(),
                TimeStamp = DateTime.Now,
            };
            var result = await _sut.CancelSearchRequest(updateSearchRequestOrdered);

            _agencyRequestServiceMock.Verify(x => x.ProcessSearchRequestOrdered(It.IsAny<SearchRequestOrdered>()), Times.Never);
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task With_nonexist_fileID_searchRequestOrdered_CancelSearchRequest_should_return_BadRequest()
        {
            SearchRequestOrdered updateSearchRequestOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.CANCEL,
                RequestId = Guid.NewGuid().ToString(),
                TimeStamp = DateTime.Now,
                SearchRequestKey = "notexist"
            };
            _agencyRequestServiceMock.Setup(x => x.ProcessCancelSearchRequest(It.IsAny<SearchRequestOrdered>()))
                .Returns(Task.FromResult<SSG_SearchRequest>(null));
            var result = await _sut.CancelSearchRequest(updateSearchRequestOrdered);

            _agencyRequestServiceMock.Verify(x => x.ProcessSearchRequestOrdered(It.IsAny<SearchRequestOrdered>()), Times.Never);
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task With_exception_CancelSearchRequest_should_return_OK_with_failed_message()
        {
            SearchRequestOrdered updateSearchRequestOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.CANCEL,
                RequestId = "23232321",
                TimeStamp = DateTime.Now,
                SearchRequestKey = "exceptionFileId"
            };

            _agencyRequestServiceMock.Setup(
                x => x.ProcessCancelSearchRequest(It.Is<SearchRequestOrdered>(x => x.SearchRequestKey == "exceptionFileId")))
                .Throws(new Exception("exception throws"));

            var result = await _sut.CancelSearchRequest(updateSearchRequestOrdered);

            _agencyRequestServiceMock.Verify(x => x.ProcessCancelSearchRequest(It.IsAny<SearchRequestOrdered>()), Times.Once);
            Assert.AreEqual(500, ((ObjectResult)result).StatusCode);
        }

        [Test]
        public async Task With_valid_searchRequestOrdered_UpdateSearchRequest_should_return_ok_with_correct_content()
        {
            SearchRequestOrdered validSearchRequestOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.UPDATE,
                RequestId = "121212121212",
                SearchRequestKey = "fileId",
                TimeStamp = DateTime.Now,
                Person = new Person()
                {
                    Agency = new Agency()
                    {
                        RequestId = "121212121212",
                        Code = "FMEP"
                    }
                }
            };

            _agencyRequestServiceMock.Setup(x => x.ProcessUpdateSearchRequest(It.IsAny<SearchRequestOrdered>()))
                .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
                {
                    FileId = "fileId",
                    SearchRequestId = Guid.NewGuid()
                }));

            var result = await _sut.UpdateSearchRequest("121212121212", validSearchRequestOrdered);

            _agencyRequestServiceMock.Verify(x => x.ProcessUpdateSearchRequest(It.IsAny<SearchRequestOrdered>()), Times.Once);
            var resultValue = result as OkObjectResult;
            Assert.NotNull(resultValue);
            var saved = resultValue.Value as SearchRequestSaved;
            Assert.NotNull(saved);
            Assert.AreEqual("fileId", saved.SearchRequestKey);
            Assert.AreEqual("FMEP", saved.ProviderProfile.Name);
            Assert.AreEqual("The Search Request fileId has been updated successfully upon the request 121212121212.", saved.Message);
        }

        [Test]
        public async Task With_null_fileId_searchRequestOrdered_UpdateSearchRequest_should_return_BadRequest()
        {
            SearchRequestOrdered validSearchRequestOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.UPDATE,
                RequestId = Guid.NewGuid().ToString(),
                TimeStamp = DateTime.Now,
            };
            var result = await _sut.UpdateSearchRequest(null, validSearchRequestOrdered);

            _agencyRequestServiceMock.Verify(x => x.ProcessUpdateSearchRequest(It.IsAny<SearchRequestOrdered>()), Times.Never);
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task With_cancel_action_searchRequestOrdered_UpdateSearchRequest_should_return_BadRequest()
        {
            SearchRequestOrdered updateSearchRequestOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.CANCEL,
                RequestId = Guid.NewGuid().ToString(),
                TimeStamp = DateTime.Now,
            };
            var result = await _sut.UpdateSearchRequest("requestId", updateSearchRequestOrdered);

            _agencyRequestServiceMock.Verify(x => x.ProcessUpdateSearchRequest(It.IsAny<SearchRequestOrdered>()), Times.Never);
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task With_empty_fileID_searchRequestOrdered_UpdateSearchRequest_should_return_BadRequest()
        {
            SearchRequestOrdered updateSearchRequestOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.UPDATE,
                RequestId = Guid.NewGuid().ToString(),
                TimeStamp = DateTime.Now,
            };
            var result = await _sut.UpdateSearchRequest("requestId", updateSearchRequestOrdered);

            _agencyRequestServiceMock.Verify(x => x.ProcessUpdateSearchRequest(It.IsAny<SearchRequestOrdered>()), Times.Never);
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task With_nonexist_fileID_searchRequestOrdered_UpdateSearchRequest_should_return_BadRequest()
        {
            SearchRequestOrdered updateSearchRequestOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.UPDATE,
                RequestId = Guid.NewGuid().ToString(),
                TimeStamp = DateTime.Now,
                SearchRequestKey = "notexist"
            };
            _agencyRequestServiceMock.Setup(x => x.ProcessUpdateSearchRequest(It.IsAny<SearchRequestOrdered>()))
                .Returns(Task.FromResult<SSG_SearchRequest>(null));
            var result = await _sut.UpdateSearchRequest("requestId", updateSearchRequestOrdered);

            _agencyRequestServiceMock.Verify(x => x.ProcessUpdateSearchRequest(It.IsAny<SearchRequestOrdered>()), Times.Once);
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task With_exception_UpdateSearchRequest_should_return_OK_with_failed_message()
        {
            SearchRequestOrdered updateSearchRequestOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.UPDATE,
                RequestId = "23232321",
                TimeStamp = DateTime.Now,
                SearchRequestKey = "exceptionFileId"
            };

            _agencyRequestServiceMock.Setup(
                x => x.ProcessUpdateSearchRequest(It.Is<SearchRequestOrdered>(x => x.SearchRequestKey == "exceptionFileId")))
                .Throws(new Exception("exception throws"));

            var result = await _sut.UpdateSearchRequest("requestId", updateSearchRequestOrdered);

            _agencyRequestServiceMock.Verify(x => x.ProcessUpdateSearchRequest(It.IsAny<SearchRequestOrdered>()), Times.Once);
            Assert.AreEqual(500, ((ObjectResult)result).StatusCode);
        }
    }
}
