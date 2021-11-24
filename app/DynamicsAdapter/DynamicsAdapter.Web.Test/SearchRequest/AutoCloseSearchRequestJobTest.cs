using AutoMapper;
using DynamicsAdapter.Web.Configuration;
using DynamicsAdapter.Web.Register;
using DynamicsAdapter.Web.SearchRequest;
using Fams3Adapter.Dynamics.DataProvider;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Fams3Adapter.Dynamics.SearchRequest;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Quartz;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.Test.SearchRequest
{
    public class AutoCloseSearchRequestJobTest
    {
        private readonly Mock<ISearchRequestService> _searchRequestServiceMock = new Mock<ISearchRequestService>();

        private readonly Mock<ISearchRequestRegister> _searchRequestRegisterMock = new Mock<ISearchRequestRegister>();
        private List<SSG_SearchRequest> _fakeSearchRequests;
        private Guid _validSearchRequestId;
        private AutoCloseSearchRequestJob _sut;
        private Mock<ILogger<AutoCloseSearchRequestJob>> _loggerMock = new Mock<ILogger<AutoCloseSearchRequestJob>>();
        private Mock<IMapper> _mapperMock = new Mock<IMapper>();
        private Mock<IJobExecutionContext> _jobContext = new Mock<IJobExecutionContext>();      

        [SetUp]
        public void Init()
        {
            _validSearchRequestId = Guid.NewGuid();

            SSG_SearchRequest ssgValidSearchRequest = new SSG_SearchRequest()
            {
                SearchRequestId = _validSearchRequestId,
                FileId = "111111" 
            };

            //_searchRequestRegisterMock.Setup(
            //    x => x.FilterDuplicatedIdentifier(It.Is<SSG_SearchApiRequest>(x => x.SearchApiRequestId == _validSearchApiRequestId)))
            //    .Returns(ssgValidSearchApiRequest);

            _searchRequestServiceMock.Setup(
                x => x.GetAutoCloseSearchRequestAsync(It.IsAny<CancellationToken>()))
               .Returns(Task.FromResult<IEnumerable<SSG_SearchRequest>>(new SSG_SearchRequest[] { ssgValidSearchRequest }));

            //_mapperMock.Setup(x => x.Map<PersonSearchRequest>(It.IsAny<SSG_SearchApiRequest>()))
            //    .Returns(new PersonSearchRequest() { SearchRequestKey = "fileId" });

            _sut = new AutoCloseSearchRequestJob(_searchRequestServiceMock.Object,_loggerMock.Object);

        }


        //we will complete this unit tests for AutoCloseSearchRequestJob later, when all code complete.
    }


}
