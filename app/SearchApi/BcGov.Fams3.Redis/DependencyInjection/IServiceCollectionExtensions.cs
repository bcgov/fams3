using BcGov.Fams3.Redis.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BcGov.Fams3.Redis.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static void AddCacheService(this IServiceCollection services, IConfiguration configuration)
        {
            RedisConfiguration redisSettings = configuration.GetSection("Redis").Get<RedisConfiguration>();

            if (redisSettings != null)
                services.AddSingleton<ICacheService, CacheService>(service => { return new CacheService(RedisConnectionFactory.OpenConnection(redisSettings.ConnectionString).GetDatabase()); });
        }

    }
}
