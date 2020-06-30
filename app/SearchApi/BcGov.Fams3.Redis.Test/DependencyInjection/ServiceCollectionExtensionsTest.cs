using BcGov.Fams3.Redis.Configuration;
using BcGov.Fams3.Redis.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

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
            _services.AddCacheService(new RedisConfiguration() { Host = "localhost", Port = 5677, Password = "password" });

            Assert.IsTrue(_services.Count == 8);
        }

    }
}
