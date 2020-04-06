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
        public static void AddCacheService(this IServiceCollection services, RedisConfiguration redisConfig)
        {

      
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = $"{redisConfig.Host}:{redisConfig.Port}";
            });
            services.AddSingleton<ICacheService, CacheService>();
        }
    }
}
