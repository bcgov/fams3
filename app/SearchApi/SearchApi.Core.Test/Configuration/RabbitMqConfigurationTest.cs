using NUnit.Framework;
using SearchApi.Core.Configuration;

namespace SearchApi.Core.Test.Configuration
{
    public class RabbitMqConfigurationTest
    {
        [Test]
        public void With_no_param_should_set_default_rabbit_config()
        {
            var sut = new RabbitMqConfiguration();

            Assert.AreEqual("localhost", sut.Host);
            Assert.AreEqual(5672, sut.Port);
            Assert.AreEqual("guest", sut.Username);
            Assert.AreEqual("guest", sut.Password);

        }

        [Test]
        public void With_param_should_configure_rabbitMq()
        {
            var sut = new RabbitMqConfiguration()
            {
                Host = "host",
                Port = 6666,
                Username = "username",
                Password = "pwd"
            };

            Assert.AreEqual("host", sut.Host);
            Assert.AreEqual(6666, sut.Port);
            Assert.AreEqual("username", sut.Username);
            Assert.AreEqual("pwd", sut.Password);

        }
    }
}