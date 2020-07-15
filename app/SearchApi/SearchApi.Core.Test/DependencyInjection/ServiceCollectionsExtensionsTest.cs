
using FluentValidation;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using SearchAdapter.Sample.SearchRequest;
using BcGov.Fams3.SearchApi.Core.Adapters.Configuration;
using BcGov.Fams3.SearchApi.Core.DependencyInjection;
using System.IO;
using System.Linq;
using System.Text;
using BcGov.Fams3.SearchApi.Contracts.Person;
using SearchAdapter.Sample.SearchResult;
using System;

namespace SearchApi.Core.Test.DependencyInjection
{

    public class ServiceCollectionsExtensionsTest
    {
        IServiceCollection services;
        IConfiguration configuration;
        IConfigurationBuilder configurationBuilder;

        string settings = string.Empty;




        [SetUp]
        public void SetUp()
        {
            settings = "{\"Logging\":{\"LogLevel\":{\"Default\": \"Information\",\"Microsoft\": \"Warning\",\"Microsoft.Hosting.Lifetime\": \"Information\"}},\"AllowedHosts\": \"*\",\"RabbitMq\": {\"Host\": \"localhost\",\"Port\": 5672,\"Username\": \"guest\",\"Password\": \"guest\"},\"ProviderProfile\": {\"Name\": \"Sample\"}}";
            services = new ServiceCollection();
            configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(settings)));
            configuration = configurationBuilder.Build();

        }

        [Test]
        public void should_register_services()
        {

            services.AddDataPartnerProvider(configuration, (provider) => new SearchRequestConsumer(provider.GetRequiredService<IValidator<Person>>(),
                                 provider.GetRequiredService<IOptions<ProviderProfileOptions>>(),
                                 provider.GetRequiredService<ILogger<SearchRequestConsumer>>()));

            Assert.IsTrue(services.Count > 0);

        }



              [Test]
        public void should_register_service_bus()
        {

            services.AddDataPartnerProvider(configuration, (provider) => new SearchRequestConsumer(provider.GetRequiredService<IValidator<Person>>(),
                                 provider.GetRequiredService<IOptions<ProviderProfileOptions>>(),
                                 provider.GetRequiredService<ILogger<SearchRequestConsumer>>()));

            Assert.IsTrue(services.Any(x => x.ServiceType == typeof(IBusControl)));


        }

        [Test]
        public void should_register_service_bus_for_inbound()
        {

            services.AddDataPartnerProvider(configuration, (provider) => new SearchRequestConsumer(provider.GetRequiredService<IValidator<Person>>(),
                                 provider.GetRequiredService<IOptions<ProviderProfileOptions>>(),
                                 provider.GetRequiredService<ILogger<SearchRequestConsumer>>()));

            Assert.IsTrue(services.Any(x => x.ServiceType == typeof(IBusControl)));


        }

        [Test]
        public void should_register_service_bus_for_passing_normal()
        {

            services.AddDataPartnerProvider(configuration, (provider) => new SearchRequestConsumer(provider.GetRequiredService<IValidator<Person>>(),
                                 provider.GetRequiredService<IOptions<ProviderProfileOptions>>(),
                                 provider.GetRequiredService<ILogger<SearchRequestConsumer>>()), (provider) => new SearchResultConsumer(
                                 provider.GetRequiredService<IOptions<ProviderProfileOptions>>(),
                                 provider.GetRequiredService<ILogger<SearchResultConsumer>>()));

            Assert.IsTrue(services.Any(x => x.ServiceType == typeof(IBusControl)));
          


        }

        [Test]
        public void should_throw_exception_if_null_ordered_consumer()
        {

            Assert.Throws<ArgumentNullException>( () => services.AddDataPartnerProvider(configuration, null, (provider) => new SearchResultConsumer(
                                  provider.GetRequiredService<IOptions<ProviderProfileOptions>>(),
                                  provider.GetRequiredService<ILogger<SearchResultConsumer>>())));
        }

        [Test]
        public void should_register_one_consumer_if_null_recieved_consumer()
        {

         services.AddDataPartnerProvider(configuration, (provider) => new SearchRequestConsumer(provider.GetRequiredService<IValidator<Person>>(),
                                 provider.GetRequiredService<IOptions<ProviderProfileOptions>>(),
                                 provider.GetRequiredService<ILogger<SearchRequestConsumer>>()));

            Assert.IsTrue(services.Any(x => x.ServiceType == typeof(IBusControl)));

        }


    }
}
