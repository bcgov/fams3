using DynamicsAdapter.Web.SearchRequest;
using DynamicsAdapter.Web.SearchRequest.Models;
using Moq;
using NUnit.Framework;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.Test.SearchRequest
{
    public class StatusReasonServiceTest
    {
        private Mock<HttpClient> htppClientMock = new Mock<HttpClient>();

        private StatusReasonService _sut;

        private string commandText = "";

        [SetUp]
        public void SetUp()
        {
            var results = new Dictionary<string, object>()
            {
                {"string", new object() }
            };
            var list = new List<IDictionary<string, object>>();
            list.Add(results);
            htppClientMock.Setup(x => x.GetAsync(commandText, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new HttpResponseMessage()));

            _sut = new StatusReasonService(htppClientMock.Object);

        }

        [Test]
        public void should_return_a_list_of_status_reason()
        {
            var result = _sut.GetList(CancellationToken.None).Result;

            Assert.IsInstanceOf(typeof(StatusReason), result);
        }
    }
}
