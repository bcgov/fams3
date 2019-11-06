using MassTransit;
using SearchApi.Core.Adapters.Contracts;
using SearchApi.Core.OpenTracing;

namespace SearchApi.Core.Adapters.Middleware
{
    public static class ProviderProfileMiddlewareConfiguratorExtensions
    {
        public static void UseProviderProfile(this IBusFactoryConfigurator value, ProviderProfile providerProfile)
        {
            value.ConfigurePublish(configurator => configurator.AddPipeSpecification(new ProviderProfilePipeSpecification(providerProfile)));
        }
    }
}