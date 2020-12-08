using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using System.Threading.Tasks;
using Moq;
using System;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Text;
using System.Net.Http;

namespace BcGov.ApiKey.Middleware.Test
{
    public class ApiKeyMiddlewareTest
    {
        Mock<IServiceProvider> _serviceProviderMock = new Mock<IServiceProvider>();
        Mock<IConfiguration> _configMock = new Mock<IConfiguration>();
        ApiKeyMiddleware _apiMw;
        RequestDelegate _next;
        bool _isNextDelegateCalled = false;

        [SetUp]
        public void Setup()
        {
            var sectionMock = new Mock<IConfigurationSection>();
            sectionMock.Setup(s => s.Value).Returns("apiValidKey");
            _serviceProviderMock.Setup(x => x.GetService(typeof(IConfiguration))).Returns(_configMock.Object);
            _configMock
                .Setup(x => x.GetSection(It.IsAny<string>()))
                .Returns(sectionMock.Object);

            _next = (HttpContext hc) => { _isNextDelegateCalled = true; return Task.CompletedTask; };
            _apiMw = new ApiKeyMiddleware(_next);
            _isNextDelegateCalled = false;
        }

        private HttpContext GetWrongKeyHttpContext()
        {
            IHeaderDictionary headers = new HeaderDictionary();
            headers.Add(ApiKeyMiddleware.HEADER_APIKEYNAME, "wrongKey");
            var request = new Mock<HttpRequest>();
            request.SetupGet(r => r.Headers).Returns(headers);
            string actual = null;
            var response = new Mock<HttpResponse>();
            response.Setup(_ => _.Body.WriteAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .Callback((byte[] data, int offset, int length, CancellationToken token) =>
                {
                    if (length > 0)
                        actual = Encoding.UTF8.GetString(data);
                })
                .Returns(Task.FromResult<byte[]>(null));
            response.SetupGet(m => m.StatusCode).Returns(401);

            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(c => c.Request).Returns(request.Object);
            httpContext.SetupGet(c => c.RequestServices).Returns(_serviceProviderMock.Object);
            httpContext.SetupGet(c => c.Response).Returns(response.Object);
            return httpContext.Object;
        }

        private HttpContext GetRightKeyHttpContext()
        {
            IHeaderDictionary headers = new HeaderDictionary();
            headers.Add(ApiKeyMiddleware.HEADER_APIKEYNAME, "apiValidKey");
            var request = new Mock<HttpRequest>();
            request.SetupGet(r => r.Headers).Returns(headers);


            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(c => c.Request).Returns(request.Object);
            httpContext.SetupGet(c => c.RequestServices).Returns(_serviceProviderMock.Object);
            return httpContext.Object;
        }

        [Test]
        public async Task without_apiKey_should_return_401()
        {

            HttpContext httpContext = new DefaultHttpContext();
            await _apiMw.InvokeAsync(httpContext);
            Assert.AreEqual(401, httpContext.Response.StatusCode);
            Assert.IsFalse(_isNextDelegateCalled);
        }

        [Test]
        public async Task with_wrong_apiKey_should_return_401()
        {

            HttpContext httpContext = GetWrongKeyHttpContext();
            await _apiMw.InvokeAsync(httpContext);
            Assert.AreEqual(401, httpContext.Response.StatusCode);
            Assert.IsFalse(_isNextDelegateCalled);
        }

        [Test]
        public async Task with_correct_apiKey_should_goon()
        {
            HttpContext httpContext = GetRightKeyHttpContext();
            await _apiMw.InvokeAsync(httpContext);
            Assert.IsTrue(_isNextDelegateCalled);

        }
    }
}