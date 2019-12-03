using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DynamicsAdapter.Web.ApiGateway;
using DynamicsAdapter.Web.Auth;
using DynamicsAdapter.Web.Configuration;
using DynamicsAdapter.Web.Test.Auth;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace DynamicsAdapter.Web.Test.ApiGateway
{
    public class ApiGatewayHandlerTest
    {
        

        public class WithApiGatewayUrlNoPathConfiguration
        {

            private ApiGatewayHandler _sut;
            private Mock<IOptions<ApiGatewayOptions>> _apiGatewayOptionsMock;

            [SetUp]
            public void SetUp()
            {

                _apiGatewayOptionsMock = new Mock<IOptions<ApiGatewayOptions>>();

                _apiGatewayOptionsMock.Setup(x => x.Value).Returns(new ApiGatewayOptions()
                {
                    BasePath = "http://apigateway"
                });

                _sut = new ApiGatewayHandler(_apiGatewayOptionsMock.Object)
                {
                    InnerHandler = new TestHandler()
                };
            }

            public class TestHandler : DelegatingHandler
            {
                protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                    CancellationToken cancellationToken)
                {
                    Assert.AreEqual("http://apigateway/", request.RequestUri.AbsoluteUri);
                    return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
                }
            }

            [Test]
            public async Task Execute()
            {
                // in your test class method
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://foo.com");
                var invoker = new HttpMessageInvoker(_sut);
                var result = await invoker.SendAsync(httpRequestMessage, new CancellationToken());
            }
        }



        public class WithApiGatewayUrlWithPathNoPathConfiguration
        {

            private ApiGatewayHandler _sut;
            private Mock<IOptions<ApiGatewayOptions>> _apiGatewayOptionsMock;

            [SetUp]
            public void SetUp()
            {

                _apiGatewayOptionsMock = new Mock<IOptions<ApiGatewayOptions>>();

                _apiGatewayOptionsMock.Setup(x => x.Value).Returns(new ApiGatewayOptions()
                {
                    BasePath = "http://apigateway"
                });

                _sut = new ApiGatewayHandler(_apiGatewayOptionsMock.Object)
                {
                    InnerHandler = new TestHandler()
                };
            }

            public class TestHandler : DelegatingHandler
            {
                protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                    CancellationToken cancellationToken)
                {
                    Assert.AreEqual("http://apigateway/test", request.RequestUri.AbsoluteUri);
                    return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
                }
            }

            [Test]
            public async Task Execute()
            {
                // in your test class method
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://foo.com/test");
                var invoker = new HttpMessageInvoker(_sut);
                var result = await invoker.SendAsync(httpRequestMessage, new CancellationToken());
            }
        }
    }


    public class WithApiGatewayNullConfiguration
    {

        private ApiGatewayHandler _sut;
        private Mock<IOptions<ApiGatewayOptions>> _apiGatewayOptionsMock;

        [SetUp]
        public void SetUp()
        {

            _apiGatewayOptionsMock = new Mock<IOptions<ApiGatewayOptions>>();

            _apiGatewayOptionsMock.Setup(x => x.Value).Returns(new ApiGatewayOptions()
            {
                BasePath = ""
            });

            _sut = new ApiGatewayHandler(_apiGatewayOptionsMock.Object)
            {
                InnerHandler = new TestHandler()
            };
        }

        public class TestHandler : DelegatingHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                Assert.AreEqual("http://foo.com/test", request.RequestUri.AbsoluteUri);
                return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            }
        }

        [Test]
        public async Task Execute()
        {
            // in your test class method
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://foo.com/test");
            var invoker = new HttpMessageInvoker(_sut);
            var result = await invoker.SendAsync(httpRequestMessage, new CancellationToken());
        }
    }


    public class WithApiGatewayBasePathWithPathConfiguration
    {

        private ApiGatewayHandler _sut;
        private Mock<IOptions<ApiGatewayOptions>> _apiGatewayOptionsMock;

        [SetUp]
        public void SetUp()
        {

            _apiGatewayOptionsMock = new Mock<IOptions<ApiGatewayOptions>>();

            _apiGatewayOptionsMock.Setup(x => x.Value).Returns(new ApiGatewayOptions()
            {
                BasePath = "http://test/path/"
            });

            _sut = new ApiGatewayHandler(_apiGatewayOptionsMock.Object)
            {
                InnerHandler = new TestHandler()
            };
        }

        public class TestHandler : DelegatingHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                Assert.AreEqual("http://test/path/somepath", request.RequestUri.AbsoluteUri);
                return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            }
        }

        [Test]
        public async Task Execute()
        {

            if (Uri.TryCreate(new Uri("http://test/path/"), "/somepath", out var a))
            {
                Console.WriteLine(a);
            }

            var test = new Uri("http://test/path/", UriKind.Absolute);

            // in your test class method
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://foo.com/somepath");
            var invoker = new HttpMessageInvoker(_sut);
            var result = await invoker.SendAsync(httpRequestMessage, new CancellationToken());
        }
    }

    public class WithApiGatewayUrlAndQueryStringConfiguration
    {

        private ApiGatewayHandler _sut;
        private Mock<IOptions<ApiGatewayOptions>> _apiGatewayOptionsMock;

        [SetUp]
        public void SetUp()
        {

            _apiGatewayOptionsMock = new Mock<IOptions<ApiGatewayOptions>>();

            _apiGatewayOptionsMock.Setup(x => x.Value).Returns(new ApiGatewayOptions()
            {
                BasePath = "http://apigateway"
            });

            _sut = new ApiGatewayHandler(_apiGatewayOptionsMock.Object)
            {
                InnerHandler = new TestHandler()
            };
        }

        public class TestHandler : DelegatingHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                Assert.AreEqual("http://apigateway/path?test=test", request.RequestUri.AbsoluteUri);
                return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            }
        }

        [Test]
        public async Task Execute()
        {
            // in your test class method
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://foo.com/path?test=test");
            var invoker = new HttpMessageInvoker(_sut);
            var result = await invoker.SendAsync(httpRequestMessage, new CancellationToken());
        }
    }


    public class WithApiGatewayUrlWithPathAndQueryStringConfiguration
    {

        private ApiGatewayHandler _sut;
        private Mock<IOptions<ApiGatewayOptions>> _apiGatewayOptionsMock;

        [SetUp]
        public void SetUp()
        {

            _apiGatewayOptionsMock = new Mock<IOptions<ApiGatewayOptions>>();

            _apiGatewayOptionsMock.Setup(x => x.Value).Returns(new ApiGatewayOptions()
            {
                BasePath = "http://apigateway/test"
            });

            _sut = new ApiGatewayHandler(_apiGatewayOptionsMock.Object)
            {
                InnerHandler = new TestHandler()
            };
        }

        public class TestHandler : DelegatingHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                Assert.AreEqual("http://apigateway/test/path?test=test", request.RequestUri.AbsoluteUri);
                return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            }
        }

        [Test]
        public async Task Execute()
        {
            // in your test class method
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://foo.com/path?test=test");
            var invoker = new HttpMessageInvoker(_sut);
            var result = await invoker.SendAsync(httpRequestMessage, new CancellationToken());
        }
    }
}