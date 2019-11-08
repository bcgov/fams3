using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DynamicsAdapter.Web.SearchRequest;
using DynamicsAdapter.Web.Services.Dynamics.Model;
using Moq;
using NUnit.Framework;
using Simple.OData.Client;

namespace DynamicsAdapter.Web.Test.SearchRequest
{
    public class SearchRequestServiceTest
    {
        private Mock<IODataClient> odataClientMock = new Mock<IODataClient>();

        private SearchRequestService _sut;


        [SetUp]
        public void SetUp()
        {

            odataClientMock.Setup(x => x.For<SSG_SearchRequest>(null)
                    .Filter(x => x.StatusCode == 867670000)
                    .FindEntriesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IEnumerable<SSG_SearchRequest>>(new List<SSG_SearchRequest>()
                {
                    new SSG_SearchRequest()
                    {
                        SSG_SearchRequestId = Guid.NewGuid(),
                        SSG_PersonGivenName = "personGivenName"
                    }
                }));

            _sut = new SearchRequestService(odataClientMock.Object);

        }


        [Test]
        public void with_success_should_return_a_collection_of_search_request()
        {
            var result = _sut.GetAllReadyForSearchAsync(CancellationToken.None).Result;

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("personGivenName", result.FirstOrDefault().SSG_PersonGivenName);

        }


    }
}