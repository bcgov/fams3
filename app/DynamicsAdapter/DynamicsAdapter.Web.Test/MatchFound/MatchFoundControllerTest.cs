using DynamicsAdapter.Web.MatchFound;
using Fams3Adapter.Dynamics.SearchRequest;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicsAdapter.Web.Test.MatchFound
{
    public class MatchFoundControllerTest
    {
        private MatchFoundController _sut;
        private readonly Mock<ILogger<MatchFoundController>> _loggerMock = new Mock<ILogger<MatchFoundController>>();
        private readonly Mock<SearchRequestService> _searchRequestServiceMock = new Mock<SearchRequestService>();

        [SetUp]
        public void Init()
        {
            _sut = new MatchFoundController(_loggerMock.Object, _searchRequestServiceMock.Object);
        }

        [Test]
        public void with_valid_match_found_data_should_return_ok()
        {
            Guid id = new Guid();
            Object obj = new object();
            IActionResult result = (OkResult)this._sut.MatchFound(id.ToString(), obj).Result;
            Assert.IsNotNull(result);
        }

        [Test]
        public void with_invalid_match_found_data_should_return_bad_request()
        {
            Object obj = new object();
            IActionResult result = (BadRequestResult)this._sut.MatchFound("", obj).Result;
            Assert.IsNotNull(result);
        }
    }
}
