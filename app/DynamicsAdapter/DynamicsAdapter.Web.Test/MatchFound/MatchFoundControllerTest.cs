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
        private readonly Mock<ISearchRequestService> _searchRequestServiceMock = new Mock<ISearchRequestService>();

        [SetUp]
        public void Init()
        {
            _sut = new MatchFoundController(_loggerMock.Object, _searchRequestServiceMock.Object);
        }

        [Test]
        public void with_valid_match_found_data_should_return_ok()
        {
            Guid id = new Guid();
            IActionResult result = (OkResult)this._sut.MatchFound(id, new Web.MatchFound.MatchFound()
            {
                PersonIds = new List<PersonId>()
            }).Result;
            Assert.IsNotNull(result);
        }

    }
}
