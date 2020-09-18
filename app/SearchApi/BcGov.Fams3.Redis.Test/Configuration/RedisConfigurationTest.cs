using NUnit.Framework;
using BcGov.Fams3.Redis.Configuration;


namespace BcGov.Fams3.Redis.Test.Configuration
{
    public class RedisConfigurationTest
    {
        [Test]
        public void With_no_param_should_set_default_redis_config()
        {
            var sut = new FRedisConfiguration();
            Assert.AreEqual("localhost", sut.Host);
            Assert.AreEqual(6379, sut.Port);
        }

        [Test]
        public void With_param_should_configure_redis()
        {
            var sut = new FRedisConfiguration()
            {
                Host = "host",
                Port = 6666,
                Password = "password"
            };

            Assert.AreEqual("host", sut.Host);
            Assert.AreEqual(6666, sut.Port);
            Assert.AreEqual("password", sut.Password);
            Assert.AreEqual("host:6666", sut.ConnectionString);
        }
    }
}
