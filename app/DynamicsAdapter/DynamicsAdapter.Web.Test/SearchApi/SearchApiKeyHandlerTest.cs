using DynamicsAdapter.Web.Configuration;
using DynamicsAdapter.Web.SearchApi;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.Test.SearchApi
{
    public class SearchApiKeyHandlerTest
    {
        public class WithNullApiKey_HttpHeaderHasNoApiKey
        {
            private SearchApiKeyHandler _sut;
            private Mock<IOptions<SearchApiConfiguration>> _searchApiConfigMock;

            [SetUp]
            public void SetUp()
            {

                _searchApiConfigMock = new Mock<IOptions<SearchApiConfiguration>>();

                _searchApiConfigMock.Setup(x => x.Value).Returns(new SearchApiConfiguration()
                { });

                _sut = new SearchApiKeyHandler(_searchApiConfigMock.Object)
                {
                    InnerHandler = new TestHandler()
                };
            }

            public class TestHandler : DelegatingHandler
            {
                protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                    CancellationToken cancellationToken)
                {
                    return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
                }
            }

            [Test]
            public async Task Execute()
            {
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://foo.com/somepath");
                var invoker = new HttpMessageInvoker(_sut);
                var result = await invoker.SendAsync(httpRequestMessage, new CancellationToken());
                Assert.IsFalse(httpRequestMessage.Headers.Contains("X-ApiKey"));
                Assert.IsTrue(result.IsSuccessStatusCode);
            }
        }

        public class WithApiKeyConfig_HttpHeaderHasApiKey
        {
            private SearchApiKeyHandler _sut;
            private Mock<IOptions<SearchApiConfiguration>> _searchApiConfigMock;

            [SetUp]
            public void SetUp()
            {

                _searchApiConfigMock = new Mock<IOptions<SearchApiConfiguration>>();

                _searchApiConfigMock.Setup(x => x.Value).Returns(new SearchApiConfiguration()
                { 
                    BaseUrl="http://test",
                    ApiKey="key"
                });

                _sut = new SearchApiKeyHandler(_searchApiConfigMock.Object)
                {
                    InnerHandler = new TestHandler()
                };
            }

            public class TestHandler : DelegatingHandler
            {
                protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                    CancellationToken cancellationToken)
                {
                    return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
                }
            }

            [Test]
            public async Task Execute()
            {
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://foo.com/somepath");
                var invoker = new HttpMessageInvoker(_sut);
                var result = await invoker.SendAsync(httpRequestMessage, new CancellationToken());
                Assert.IsFalse(httpRequestMessage.Headers.Contains("X-ApiKey"));
                Assert.IsTrue(result.IsSuccessStatusCode);
            }
        }

        public class WithNullConfig_HttpHeaderHasNoApiKey
        {
            private SearchApiKeyHandler _sut;
            private Mock<IOptions<SearchApiConfiguration>> _searchApiConfigMock;

            [SetUp]
            public void SetUp()
            {
                _sut = new SearchApiKeyHandler(null)
                {
                    InnerHandler = new TestHandler()
                };
            }

            public class TestHandler : DelegatingHandler
            {
                protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                    CancellationToken cancellationToken)
                {
                    return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
                }
            }

            [Test]
            public async Task Execute()
            {
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://foo.com/somepath");
                var invoker = new HttpMessageInvoker(_sut);
                var result = await invoker.SendAsync(httpRequestMessage, new CancellationToken());
                Assert.IsFalse(httpRequestMessage.Headers.Contains("X-ApiKey"));
                Assert.IsTrue(result.IsSuccessStatusCode);
            }
        }
    }
}
