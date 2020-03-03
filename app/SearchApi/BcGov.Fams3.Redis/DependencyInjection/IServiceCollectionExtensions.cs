using BcGov.Fams3.Redis.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;

namespace BcGov.Fams3.Redis.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static void AddCacheService(this IServiceCollection services, IConfiguration configuration)
        {
            try
            {
                RedisConfiguration redisSettings = configuration.GetSection("Redis").Get<RedisConfiguration>();

                if (redisSettings != null)
                    services.AddSingleton<ICacheService, CacheService>(service => { return new CacheService(RedisConnectionFactory.OpenConnection(redisSettings.ConnectionString).GetDatabase()); });
            }catch(RedisException e)
            {
                throw e;
            }
        }

    }
}
