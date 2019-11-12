using DynamicsAdapter.Web.MatchFound;
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

        [SetUp]
        public void Init()
        {
            _sut = new MatchFoundController(_loggerMock.Object);
        }

        [Test]
        public void with_valid_match_found_data_should_return_ok()
        {
            MatchFoundResponse matchFoundResponse = new MatchFoundResponse
            {
                SearchRequestId = "searchRequestGuid",
                SearchResult = "searchResult"
            };
            IActionResult result =(OkResult) this._sut.MatchFound(matchFoundResponse).Result;
            Assert.IsNotNull(result);
        }

        [Test]
        public void with_invalid_match_found_data_should_return_bad_request()
        {
            MatchFoundResponse matchFoundResponse = new MatchFoundResponse
            {
                SearchRequestId = "",
                SearchResult = "searchResult"
            };
            IActionResult result = (BadRequestResult)this._sut.MatchFound(matchFoundResponse).Result;
            Assert.IsNotNull(result);
        }
    }
}
