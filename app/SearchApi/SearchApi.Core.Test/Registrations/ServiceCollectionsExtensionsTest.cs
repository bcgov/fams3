using Castle.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SearchApi.Core.Registrations;

namespace SearchApi.Core.Test.Registrations
{

    public class ServiceCollectionsExtensionsTest
    {
        IServiceCollection services;
        IConfiguration configuration;

    


        [SetUp]
        public void SetUp()
        {
            services = new ServiceCollection();
           

        }

        [Test]
        public void should_register_provider()
        {


            services.AddProvider(configuration,new  SearchRequestConsumer(
                                provider.GetRequiredService<IValidator<Person>>(),
                                providerOptions,
                                provider.GetRequiredService<ILogger<SearchRequestConsumer>>()));

            Assert.IsTrue(serviceCollection.Any(x => x.ServiceType == typeof(IValidator<QualityControlForm>)));


        }


    }
}
