using DynamicsAdapter.Web.SearchAgency;
using DynamicsAdapter.Web.SearchAgency.Models;
using DynamicsAdapter.Web.SearchAgency.Webhook;
using Fams3Adapter.Dynamics.SearchRequest;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.Test.SearchAgency
{
    public class AgencyResponseControllerTest
    {
        private AgencyResponseController _sut;
        private Mock<ILogger<AgencyResponseController>> _loggerMock;
        private Mock<IAgencyResponseService> _agencyResponseServiceMock;
        private Mock<IAgencyNotificationWebhook<SearchRequestNotification>> _agencyWebhookMock;
        private SearchResponseReady _ready;

        [SetUp]
        public void Init()
        {
            _loggerMock = new Mock<ILogger<AgencyResponseController>>();
            _agencyResponseServiceMock = new Mock<IAgencyResponseService>();
            _agencyWebhookMock = new Mock<IAgencyNotificationWebhook<SearchRequestNotification>>();

            _ready = new SearchResponseReady()
            {
                Activity = "RequestClosed",
                ActivityDate = DateTime.Now,
                Agency = "agency",
                FileId = "fileId",
                AgencyFileId = "referId",
                FSOName = "fso",
                ResponseGuid = Guid.NewGuid().ToString()
            };

            _sut = new AgencyResponseController(_loggerMock.Object, _agencyResponseServiceMock.Object, _agencyWebhookMock.Object);
        }

        [Test]
        public async Task With_valid_responseReady_ResponseReady_should_return_ok()
        {

            _agencyResponseServiceMock.Setup(x => x.GetSearchRequestResponse(It.IsAny<SearchResponseReady>()))
                .Returns(Task.FromResult<Person>(new Person()
                { }));

            _agencyWebhookMock.Setup(x => x.SendNotificationAsync(It.IsAny<SearchRequestNotification>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _sut.ResponseReady(_ready);

            _agencyResponseServiceMock.Verify(x => x.GetSearchRequestResponse(It.IsAny<SearchResponseReady>()), Times.Once);
            _agencyWebhookMock.Verify(x => x.SendNotificationAsync(It.IsAny<SearchRequestNotification>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsInstanceOf(typeof(OkResult), result);
        }

        [Test]
        public async Task With_exception_throws_ResponseReady_should_return_InternalServerError()
        {
            _agencyResponseServiceMock.Setup(x => x.GetSearchRequestResponse(It.IsAny<SearchResponseReady>()))
                .Throws(new Exception("exception"));

            _agencyWebhookMock.Setup(x => x.SendNotificationAsync(It.IsAny<SearchRequestNotification>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _sut.ResponseReady(_ready);
            _agencyResponseServiceMock.Verify(x => x.GetSearchRequestResponse(It.IsAny<SearchResponseReady>()), Times.Once);
            _agencyWebhookMock.Verify(x => x.SendNotificationAsync(It.IsAny<SearchRequestNotification>(), It.IsAny<CancellationToken>()), Times.Never);

            Assert.AreEqual(500, ((ObjectResult)result).StatusCode);
        }

        [Test]
        public async Task With_null_person_ResponseReady_should_return_BadRequest()
        {
            _agencyResponseServiceMock.Setup(x => x.GetSearchRequestResponse(It.IsAny<SearchResponseReady>()))
                .Returns(Task.FromResult<Person>(null));

            var result = await _sut.ResponseReady(_ready);
            _agencyResponseServiceMock.Verify(x => x.GetSearchRequestResponse(It.IsAny<SearchResponseReady>()), Times.Once);
            _agencyWebhookMock.Verify(x => x.SendNotificationAsync(It.IsAny<SearchRequestNotification>(), It.IsAny<CancellationToken>()), Times.Never);

            Assert.AreEqual(400, ((ObjectResult)result).StatusCode);
        }

    }
}
