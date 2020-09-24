using BcGov.Fams3.Redis.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using StackExchange.Redis.Extensions.Core.Configuration;
using System.Collections.Generic;

namespace BcGov.Fams3.Redis.Test.DependencyInjection
{
    public class ServiceCollectionExtensionsTest
    {
        private IServiceCollection _services;


        [SetUp]
        public void SetUp()
        {
            _services = new ServiceCollection();

        }
        [Test]
        public void should_register_services()
        {
            var redisHost = new List<RedisHost>();

            redisHost.Add(new RedisHost { Host="localhost", Port=6709 });

            _services.AddCacheService(new RedisConfiguration() { Hosts = redisHost.ToArray(), Password = "password" });

            Assert.IsTrue(_services.Count == 13);
        }

    }
}
