using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace DynamicsAdapter.Web.Auth
{
    public interface ITokenService
    {
        Task<Token> GetTokenAsync(CancellationToken cancellationToken);
    }

    /// <summary>
    /// The TokenService expose services to get OAuth Tokens
    /// </summary>
    public class TokenService : ITokenService
    {
        private const int Buffer = 10;

        private readonly string token_key = "oauth-token";

        private readonly IOAuthApiClient _oAuthApiClient;
        private readonly IDistributedCache _distributedCache;

        public TokenService(IOAuthApiClient oAuthApiClient, IDistributedCache distributedCache)
        {
            _oAuthApiClient = oAuthApiClient;
            _distributedCache = distributedCache;
        }

        /// <summary>
        /// Gets a token, from the cache or the authority provider.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Token> GetTokenAsync(CancellationToken cancellationToken)
        {
            return await GetOrRefreshTokenAsync(cancellationToken);
        }

        private async Task<Token> GetOrRefreshTokenAsync(CancellationToken cancellationToken)
        {
            var token = await GetFromCacheAsync();
            if (token == null)
                return await RefreshTokenAsync(cancellationToken);
            return token;
        }

        private async Task<Token> GetFromCacheAsync()
        {
            var tokenString = await _distributedCache.GetStringAsync(this.token_key);
            if (String.IsNullOrEmpty(tokenString)) return null;
            return JsonConvert.DeserializeObject<Token>(tokenString);
        }

        private async Task<Token> RefreshTokenAsync(CancellationToken cancellationToken)
        {
            var token = await _oAuthApiClient.GetRefreshToken(cancellationToken);
            await _distributedCache.SetStringAsync(this.token_key, JsonConvert.SerializeObject(token), new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = new TimeSpan(0, 0, token.ExpiresIn - Buffer) }, cancellationToken);
            return token;
        }

    }
}