using AutoMapper;
using DynamicsAdapter.Web.SearchAgency;
using DynamicsAdapter.Web.SearchAgency.Models;
using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.Employment;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.Name;
using Fams3Adapter.Dynamics.Notes;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.PhoneNumber;
using Fams3Adapter.Dynamics.RelatedPerson;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.SearchResponse;
using Fams3Adapter.Dynamics.Types;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.Test.SearchAgency
{
    public class AgencyResponseServiceTest
    {
        private AgencyResponseService _sut;
        private Mock<ILogger<AgencyResponseService>> _loggerMock;
        private Mock<ISearchResponseService> _searchResponseServiceMock;
        private Mock<IMapper> _mapper;
        private SearchResponseReady _ready;

        [SetUp]
        public void Init()
        {
            _loggerMock = new Mock<ILogger<AgencyResponseService>>();
            _searchResponseServiceMock = new Mock<ISearchResponseService>();
            _mapper = new Mock<IMapper>();

            _mapper.Setup(m => m.Map<Person>(It.IsAny<SSG_SearchRequestResponse>()))
                               .Returns(new Person());


            _searchResponseServiceMock.Setup(x => x.GetSearchResponse(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_SearchRequestResponse>(new SSG_SearchRequestResponse()
                {
                }));
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

            _sut = new AgencyResponseService(_searchResponseServiceMock.Object, _loggerMock.Object, _mapper.Object);

        }

        [Test]
        public async Task normal_SearchResponseReady_GetSearchRequestResponse_should_succeed()
        {
            Person person = await _sut.GetSearchRequestResponse(_ready);
            _searchResponseServiceMock.Verify(x => x.GetSearchResponse(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsNotNull(person);
        }

        [Test]
        public async Task empty_ResponseGuid_GetSearchRequestResponse_should_return_null()
        {
            _ready.ResponseGuid = Guid.Empty.ToString();
            Person person = await _sut.GetSearchRequestResponse(_ready);
            _searchResponseServiceMock.Verify(x => x.GetSearchResponse(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            Assert.IsNull(person);
        }

        [Test]
        public async Task wrong_ResponseGuid_GetSearchRequestResponse_should_return_null()
        {
            Guid wrongGuid = Guid.NewGuid();
            _ready.ResponseGuid = wrongGuid.ToString();
            _searchResponseServiceMock.Setup(x => x.GetSearchResponse(It.Is<Guid>(m=>m==wrongGuid), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<SSG_SearchRequestResponse>(null));
            Person person = await _sut.GetSearchRequestResponse(_ready);
            _searchResponseServiceMock.Verify(x => x.GetSearchResponse(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsNull(person);
        }

    }
}
