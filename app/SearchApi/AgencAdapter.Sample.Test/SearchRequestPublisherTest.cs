using AgencyAdapter.Sample.SearchRequest;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgencAdapter.Sample.Test
{
    public class SearchRequestPublisherTest
    {
        private SearchRequestPublisher _sut;
        Mock<IValidator<AgencyAdapter.Sample.Models.SearchRequest>> validatorMock;
        Mock<ILogger<SearchRequestPublisher>> loggerMock;
        AgencyAdapter.Sample.Models.SearchRequest requestValid;
        AgencyAdapter.Sample.Models.SearchRequest requestInValid;

        [SetUp]
        public void SetUp()
        {
            validatorMock = new Mock<IValidator<AgencyAdapter.Sample.Models.SearchRequest>>();
            loggerMock = new Mock<ILogger<SearchRequestPublisher>>();


            validatorMock.Setup(x => x.Validate(It.Is<AgencyAdapter.Sample.Models.SearchRequest>(request => !string.IsNullOrEmpty(request.RequestorAgencyCode))))
        .Returns(new ValidationResult(Enumerable.Empty<ValidationFailure>()));


            validatorMock.Setup(x => x.Validate(It.Is<AgencyAdapter.Sample.Models.SearchRequest>(request => string.IsNullOrEmpty(request.RequestorAgencyCode))))
           .Returns(new ValidationResult(new List<ValidationFailure>()
               {
                    new ValidationFailure("RequestorAgencyCode", "Code is required.")
               }));

            requestValid = new AgencyAdapter.Sample.Models.SearchRequest
            {
                RequestorAgencyCode = "Code"
            };
            requestInValid = new AgencyAdapter.Sample.Models.SearchRequest
            {
                RequestorAgencyCode = ""
            };
            _sut = new SearchRequestPublisher(validatorMock.Object, loggerMock.Object);
        }

        [Test]
        public async Task  when_request_valid_should_publish()
        {
           await  _sut.ProcessRequest(requestValid);

            loggerMock.VerifyLog(LogLevel.Debug, "Start the process", Times.Once());
            loggerMock.VerifyLog(LogLevel.Debug, "Attempting to validate the Search Request", Times.Once());
            loggerMock.VerifyLog(LogLevel.Debug, "Request is valid.", Times.Once());
            loggerMock.VerifyLog(LogLevel.Debug, "Process and send to queue", Times.Once());
        }

        [Test]
        public async Task when_request_invalid_should_not_publish()
        {
            await _sut.ProcessRequest(requestInValid);

            loggerMock.VerifyLog(LogLevel.Debug, "Start the process", Times.Once());
            loggerMock.VerifyLog(LogLevel.Debug, "Attempting to validate the Search Request", Times.Once());
            loggerMock.VerifyLog(LogLevel.Debug, "Request failed.", Times.Once());
            loggerMock.VerifyLog(LogLevel.Debug, "Process and send to queue", Times.Never());
        }
    }
}
