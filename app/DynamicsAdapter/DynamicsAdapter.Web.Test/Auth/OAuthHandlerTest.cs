using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DynamicsAdapter.Web.Auth;
using Moq;
using NUnit.Framework;

namespace DynamicsAdapter.Web.Test.Auth
{
    public class OAuthHandlerTest
    {
        private OAuthHandler _sut;

        private Mock<ITokenService> _tokenServiceMock = new Mock<ITokenService>();

        [SetUp]
        public void SetUp()
        {
            _tokenServiceMock.Setup(x => x.GetTokenAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult<Token>(
                new Token()
                {
                    AccessToken = "accessToken"
                }));

            _sut = new OAuthHandler(_tokenServiceMock.Object)
            {
                InnerHandler = new TestHandler()
                {
                    InnerHandler = new HttpClientHandler()
                }
            };
        }

        public class TestHandler : DelegatingHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Assert.AreEqual("accessToken", request.Headers.Authorization.Parameter);
                Assert.AreEqual("Bearer", request.Headers.Authorization.Scheme);
                return await base.SendAsync(request, cancellationToken);
            }
        }

        [Test]
        public async Task with_token_it_should_add_it_to_header()
        {
            // in your test class method
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://foo.com");

            var invoker = new HttpMessageInvoker(_sut);
            var result = await invoker.SendAsync(httpRequestMessage, new CancellationToken());

        }

    }
}