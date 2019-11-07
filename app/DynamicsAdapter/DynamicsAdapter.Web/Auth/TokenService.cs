using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace DynamicsAdapter.Web.Auth
{
    public interface ITokenService
    {
        Task<Token> GetToken(CancellationToken cancellationToken);
    }

    public class TokenService : ITokenService
    {
        private readonly IOAuthApiClient _oAuthApiClient;
        private readonly IDistributedCache _distributedCache;

        public TokenService(IOAuthApiClient oAuthApiClient, IDistributedCache distributedCache)
        {
            _oAuthApiClient = oAuthApiClient;
            _distributedCache = distributedCache;
        }

        public async Task<Token> GetToken(CancellationToken cancellationToken)
        {
            return await GetOrRefreshToken(cancellationToken);
        }

        private async Task<Token> GetOrRefreshToken(CancellationToken cancellationToken)
        {
            var token = await GetFromCache();
            if (token == null)
                return await RefreshToken(cancellationToken);
            return token;
        }

        private async Task<Token> GetFromCache()
        {
            var tokenString = await _distributedCache.GetStringAsync("oauth-token");
            if (String.IsNullOrEmpty(tokenString)) return null;
            return JsonConvert.DeserializeObject<Token>(tokenString);
        }

        private async Task<Token> RefreshToken(CancellationToken cancellationToken)
        {
            var token = await _oAuthApiClient.GetRefreshToken(cancellationToken);
            await _distributedCache.SetStringAsync("oauth-token", JsonConvert.SerializeObject(token), new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = new TimeSpan(0, 0, token.ExpiresIn - 10) }, cancellationToken);
            return token;
        }

    }
}