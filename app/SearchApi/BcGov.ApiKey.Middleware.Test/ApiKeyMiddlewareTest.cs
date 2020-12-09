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
        Mock<IConfigurationSection> _sectionApiHeaderMock = new Mock<IConfigurationSection>();
        Mock<IConfigurationSection> _sectionTrustedHostMock = new Mock<IConfigurationSection>();
        ApiKeyMiddleware _apiMw;
        RequestDelegate _next;
        bool _isNextDelegateCalled = false;

        [SetUp]
        public void Setup()
        {
            _serviceProviderMock.Setup(x => x.GetService(typeof(IConfiguration))).Returns(_configMock.Object);
            _configMock
                .Setup(x => x.GetSection(It.Is<string>(m=>m=="service_apiKey")))
                .Returns(_sectionApiHeaderMock.Object);
            _configMock
                .Setup(x => x.GetSection(It.Is<string>(m => m == ApiKeyMiddleware.TRUSTED_HOST_KEYNAME)))
                .Returns(_sectionTrustedHostMock.Object);

            _next = (HttpContext hc) => { _isNextDelegateCalled = true; return Task.CompletedTask; };
            _apiMw = new ApiKeyMiddleware(_next,"service_apiKey");
            _isNextDelegateCalled = false;
        }

        private HttpContext GetNoKeyHttpContext(bool pathContainSwagger = false)
        {
            IHeaderDictionary headers = new HeaderDictionary();
            var request = new Mock<HttpRequest>();
            request.SetupGet(r => r.Headers).Returns(headers);
            request.SetupGet(r => r.Host).Returns(new HostString("host1", 9000));
            if( pathContainSwagger )
                request.SetupGet(r => r.Path).Returns(new PathString("/swagger/"));

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
            _sectionApiHeaderMock.Setup(s => s.Value).Returns("apiValidKey");
            _sectionTrustedHostMock.Setup(s => s.Value).Returns("host6,host2");

            httpContext.SetupGet(c => c.Response).Returns(response.Object);
            return httpContext.Object;
        }

        private HttpContext GetWrongKeyHttpContext()
        {
            IHeaderDictionary headers = new HeaderDictionary();
            headers.Add(ApiKeyMiddleware.HEADER_APIKEYNAME, "wrongKey");
            var request = new Mock<HttpRequest>();
            request.SetupGet(r => r.Headers).Returns(headers);
            request.SetupGet(r => r.Host).Returns(new HostString("host1", 9000));
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
            _sectionApiHeaderMock.Setup(s => s.Value).Returns("apiValidKey");
            _sectionTrustedHostMock.Setup(s => s.Value).Returns("host6,host2");

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
        public async Task without_apiKey_not_in_trustedHosts_should_return_401()
        {
            HttpContext httpContext = GetNoKeyHttpContext();
            await _apiMw.InvokeAsync(httpContext);
            Assert.AreEqual(401, httpContext.Response.StatusCode);
            Assert.IsFalse(_isNextDelegateCalled);
        }

        [Test]
        public async Task without_apiKey_but_in_trustedHosts_should_go_on()
        {
            HttpContext httpContext = GetNoKeyHttpContext();
            _sectionTrustedHostMock.Setup(s => s.Value).Returns("host1,host2");
            await _apiMw.InvokeAsync(httpContext);
            Assert.IsTrue(_isNextDelegateCalled);
        }

        [Test]
        public async Task with_wrong_apiKey_not_in_trustedHosts_should_return_401()
        {

            HttpContext httpContext = GetWrongKeyHttpContext();
            await _apiMw.InvokeAsync(httpContext);
            Assert.AreEqual(401, httpContext.Response.StatusCode);
            Assert.IsFalse(_isNextDelegateCalled);
        }

        [Test]
        public async Task with_wrong_apiKey_but_in_trustedHosts_should_go_on()
        {
            HttpContext httpContext = GetWrongKeyHttpContext();
            _sectionTrustedHostMock.Setup(s => s.Value).Returns("host1,host2");
            await _apiMw.InvokeAsync(httpContext);
            Assert.IsTrue(_isNextDelegateCalled);
        }

        [Test]
        public async Task with_wrong_apiKey_but_star_trustedHosts_should_go_on()
        {
            HttpContext httpContext = GetWrongKeyHttpContext();
            _sectionTrustedHostMock.Setup(s => s.Value).Returns("*");
            await _apiMw.InvokeAsync(httpContext);
            Assert.IsTrue(_isNextDelegateCalled);
        }


        [Test]
        public async Task with_correct_apiKey_should_goon()
        {
            HttpContext httpContext = GetRightKeyHttpContext();
            _sectionApiHeaderMock.Setup(s => s.Value).Returns("apiValidKey");
            await _apiMw.InvokeAsync(httpContext);
            Assert.IsTrue(_isNextDelegateCalled);

        }

        [Test]
        public async Task with_swagger_visit_should_go_on()
        {
            HttpContext httpContext = GetNoKeyHttpContext(true);
            await _apiMw.InvokeAsync(httpContext);
            Assert.IsTrue(_isNextDelegateCalled);
        }
    }
}