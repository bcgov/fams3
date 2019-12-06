
using FluentValidation;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using SearchAdapter.Sample.SearchRequest;
using SearchApi.Core.Adapters.Configuration;
using SearchApi.Core.MassTransit;
using SearchApi.Core.Registrations;
using System.Linq;

namespace SearchApi.Core.Test.Registrations
{

    public class ServiceCollectionsExtensionsTest
    {
        IServiceCollection services;
        IConfiguration configuration;
        IConfigurationBuilder configurationBuilder;




        [SetUp]
        public void SetUp()
        {
            services = new ServiceCollection();
            configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile(@"Settings\appsettings.json");
            configuration = configurationBuilder.Build();

        }

        [Test]
        public void should_register_services()
        {

            services.AddProvider(configuration, (provider) => new SearchRequestConsumer(provider.GetRequiredService<IValidator<Person.Contracts.Person>>(),
                                 provider.GetRequiredService<IOptions<ProviderProfileOptions>>(),
                                 provider.GetRequiredService<ILogger<SearchRequestConsumer>>()));

            Assert.IsTrue(services.Count > 0);

        }



              [Test]
        public void should_register_service_bus()
        {

            services.AddProvider(configuration, (provider) => new SearchRequestConsumer(provider.GetRequiredService<IValidator<Person.Contracts.Person>>(),
                                 provider.GetRequiredService<IOptions<ProviderProfileOptions>>(),
                                 provider.GetRequiredService<ILogger<SearchRequestConsumer>>()));

            Assert.IsTrue(services.Any(x => x.ServiceType == typeof(IBusControl)));


        }


    }
}
