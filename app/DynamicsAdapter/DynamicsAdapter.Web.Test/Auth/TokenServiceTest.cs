using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DynamicsAdapter.Web.Auth;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using NUnit.Framework;

namespace DynamicsAdapter.Web.Test.Auth
{
    public class TokenServiceTest
    {
        private TokenService _sut;

        private readonly string token_key = "oauth-token";

        private Mock<IOAuthApiClient> _oAuthApiClient = new Mock<IOAuthApiClient>();
        private Mock<IDistributedCache> _distributedCache = new Mock<IDistributedCache>();


        [Test]
        public async Task With_token_in_cache_should_return_token()
        {

            _oAuthApiClient.Setup(x => x.GetRefreshToken(It.IsAny<CancellationToken>())).Returns(Task.FromResult<Token>(
                new Token()
                {
                    AccessToken = "tokenFromApi"
                }));

            _distributedCache
                .Setup(x => x.GetAsync(token_key, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Encoding.UTF8.GetBytes("{\"access_token\": \"tokenFromCache\",\"token_type\": \"bearer\",\"expires_in\": 3600,\"resource\": \"https://resource\",\"refresh_token\": \"refresh_token\",\"refresh_token_expires_in\": 28799,\"scope\": \"openid\",\"id_token\": \"token_id\"}")));

            _sut = new TokenService(_oAuthApiClient.Object, _distributedCache.Object);

            var token = await _sut.GetTokenAsync(CancellationToken.None);

            Assert.AreEqual("tokenFromCache", token.AccessToken);

        }



        [Test]
        public async Task With_token_not_cache_should_return_token_from_api()
        {

            _oAuthApiClient.Setup(x => x.GetRefreshToken(It.IsAny<CancellationToken>())).Returns(Task.FromResult<Token>(
                new Token()
                {
                    AccessToken = "tokenFromApi",
                    ExpiresIn = 12,
                    RefreshToken = "refresh_token",
                    TokenType = "token_type",
                    Scope = "scope",
                    RefreshTokenExpiresIn = 12,
                    IdToken = "idToken",
                    Resource = "resource"
                }));

            _distributedCache
                .Setup(x => x.GetAsync(token_key, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<byte[]>(null));

            _distributedCache
                .Setup(x => x.Set(token_key, It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>())).Verifiable();

            _sut = new TokenService(_oAuthApiClient.Object, _distributedCache.Object);

            var token = await _sut.GetTokenAsync(CancellationToken.None);

            Assert.AreEqual("tokenFromApi", token.AccessToken);

        }

    }

}