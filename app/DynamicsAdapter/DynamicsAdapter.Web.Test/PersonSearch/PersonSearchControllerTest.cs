using System;
using System.Collections.Generic;
using DynamicsAdapter.Web.PersonSearch;
using Fams3Adapter.Dynamics.SearchRequest;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace DynamicsAdapter.Web.Test.PersonSearch
{
    public class PersonSearchControllerTest
    {
        private PersonSearchController _sut;
        private readonly Mock<ILogger<PersonSearchController>> _loggerMock = new Mock<ILogger<PersonSearchController>>();
        private readonly Mock<ISearchRequestService> _searchRequestServiceMock = new Mock<ISearchRequestService>();

        [SetUp]
        public void Init()
        {
            _sut = new PersonSearchController(_loggerMock.Object, _searchRequestServiceMock.Object);
        }

        [Test]
        public void with_valid_match_found_data_should_return_ok()
        {
            Guid id = new Guid();
            IActionResult result = (OkResult)this._sut.MatchFound(id, new Web.PersonSearch.MatchFound()
            {
                PersonIds = new List<PersonId>()
            }).Result;
            Assert.IsNotNull(result);
        }

    }
}
