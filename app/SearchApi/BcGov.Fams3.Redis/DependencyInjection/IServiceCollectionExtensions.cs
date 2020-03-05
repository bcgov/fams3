using BcGov.Fams3.Redis.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;

namespace BcGov.Fams3.Redis.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static void AddCacheService(this IServiceCollection services, IConfiguration configuration)
        {

            var redisSettings = configuration.GetSection("Redis").Get<RedisConfiguration>();
            if (redisSettings == null) throw new Exception("Invalid redis settings. ");

            try
            {
                services.AddSingleton<IRedisConnectionFactory, IRedisConnectionFactory>(factory => new RedisConnectionFactory(redisSettings.ConnectionString, factory.GetService<ILogger<RedisConnectionFactory>>()));
                services.AddSingleton<ICacheService, CacheService>();
            }
            catch(RedisException e)
            {
                throw e;
            }
        }
    }
}
