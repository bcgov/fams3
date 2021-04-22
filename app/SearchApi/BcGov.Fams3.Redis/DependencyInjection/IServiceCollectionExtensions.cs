using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Core.Implementations;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace BcGov.Fams3.Redis.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {

        /// Add StackExchange.Redis with its serialization provider.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="redisConfiguration">The redis configration.</param>
        /// <typeparam name="T">The typof of serializer. <see cref="ISerializer" />.</typeparam>
        private static void AddStackExchangeRedisExtensions<T>(this IServiceCollection services)
            where T : class, ISerializer, new()
        {
         
            services.AddSingleton<IRedisCacheClient, RedisCacheClient>();
            services.AddSingleton<IRedisCacheConnectionPoolManager, RedisCacheConnectionPoolManager>();
            services.AddSingleton<ISerializer, T>();

            services.AddSingleton((provider) =>
            {
                return provider.GetRequiredService<IRedisCacheClient>().GetDbFromConfiguration();
            });

        
        }

        public static void AddCacheService(this IServiceCollection services, RedisConfiguration redisConfig)
        {


            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = $"{redisConfig.Hosts[0].Host}:{redisConfig.Hosts[0].Port},Password={redisConfig.Password}";
                //options.ConfigurationOptions.ConnectTimeout = redisConfig.ConnectTimeout;
                //options.ConfigurationOptions.AbortOnConnectFail = redisConfig.AbortOnConnectFail;
                //options.ConfigurationOptions.SyncTimeout = redisConfig.SyncTimeout;
                //options.ConfigurationOptions.ConnectRetry = 4;

            });
            services.AddSingleton<ICacheService, CacheService>();

            services.AddSingleton(redisConfig);

            services.AddStackExchangeRedisExtensions<NewtonsoftSerializer>();
        }
    }
}
