using DynamicsAdapter.Web.SearchRequest;
using DynamicsAdapter.Web.SearchRequest.Models;
using Moq;
using NUnit.Framework;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.Test.SearchRequest
{
    public class StatusReasonServiceTest
    {
        private Mock<IODataClient> odataClientMock = new Mock<IODataClient>();

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
            odataClientMock.Setup(x => x.FindEntriesAsync(commandText, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IEnumerable<IDictionary<string, object>>>(list));

            _sut = new StatusReasonService(odataClientMock.Object);

        }

        [Test]
        public void should_return_a_list_of_status_reason()
        {
            var result = _sut.GetList(CancellationToken.None).Result;

            Assert.IsInstanceOf(typeof(StatusReason), result);
        }
    }
}
