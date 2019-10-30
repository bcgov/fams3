using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicsAdapter.Web.Configuration;
using DynamicsAdapter.Web.Services.Dynamics;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicsAdapter.Web.Test.Services.Dynamics
{
    public class ConfigureDynamicsModuleTest
    {

        [Test]
        public void Should_register_the_services_lab_sample()
        {
            var serviceCollection = new ServiceCollection();

            var appSettings = new AppSettings
            {
                DynamicsAPI = new DynamicsAPIConfig
                {
                    TokenTimeout = 60,
                    Timeout =  60,
                    EndPoints = new List<EndPoint>()
                }
            };
            ConfigureDynamicsModule.ConfigureServices(serviceCollection, appSettings);

            Assert.IsTrue(serviceCollection.Any(x => x.ServiceType == typeof(IDynamicService)));

        }
    }
}
