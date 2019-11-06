using MassTransit;
using SearchApi.Core.Adapters.Contracts;
using SearchApi.Core.OpenTracing;

namespace SearchApi.Core.Adapters.Middleware
{
    /// <summary>
    /// Configures MassTransit to use the provider pipeline
    /// </summary>
    public static class ProviderProfileMiddlewareConfiguratorExtensions
    {
        public static void UseProviderProfile(this IBusFactoryConfigurator value, ProviderProfile providerProfile)
        {
            value.ConfigurePublish(configurator => configurator.AddPipeSpecification(new ProviderProfilePipeSpecification(providerProfile)));
        }
    }
}