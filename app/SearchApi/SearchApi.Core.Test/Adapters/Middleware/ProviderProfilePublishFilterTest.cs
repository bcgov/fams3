using System.Linq;
using System.Threading.Tasks;
using MassTransit.Testing;
using NUnit.Framework;
using SearchApi.Core.Adapters.Configuration;
using SearchApi.Core.Adapters.Contracts;
using SearchApi.Core.Adapters.Middleware;

namespace SearchApi.Core.Test.Adapters.Middleware
{
    public class ProviderProfilePublishFilterTest
    {

        InMemoryTestHarness _harness;

        [OneTimeSetUp]
        public async Task A_consumer_is_being_tested()
        {
            _harness = new InMemoryTestHarness();
            _harness.OnConfigureInMemoryBus += configurator =>
            {
                configurator.UseProviderProfile(new ProviderProfileOptions()
                {
                    Name = "ProviderA"
                });
            };

            await _harness.Start();

            await _harness.BusControl.Publish<A>(new A());
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _harness.Stop();
        }

        [Test]
        public void Should_send_the_initial_message_to_the_consumer()
        {
            Assert.IsTrue(_harness.Published.Select<A>().Any());
        }

        [Test]
        public void Should_attach_the_provider_information_to_header()
        {
            var message = _harness.Published.Select<A>().FirstOrDefault();

            var providerProfile = (ProviderProfile)message?.Context.Headers.GetAll().FirstOrDefault(x => x.Key == nameof(ProviderProfile)).Value;

            Assert.AreEqual("ProviderA", providerProfile.Name);
        }

        public class A { }

    }
}