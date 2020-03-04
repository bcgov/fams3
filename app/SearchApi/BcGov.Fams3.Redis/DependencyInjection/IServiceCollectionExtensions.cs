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

            RedisConfiguration redisSettings = configuration.GetSection("Redis").Get<RedisConfiguration>();
            if (redisSettings == null)
            {
                throw new Exception("Invalid redis settings. ");
            }

            try
            {
                if (redisSettings != null)
                {
                    services.AddSingleton<IRedisConnectionFactory, RedisConnectionFactory>(factory => {return new RedisConnectionFactory(redisSettings.ConnectionString); });
                    services.AddSingleton<ICacheService, CacheService>(service => { return new CacheService(service.GetService<IRedisConnectionFactory>()); });
                }
            }
            catch(RedisException e)
            {
                throw e;
            }
        }

    }
}
