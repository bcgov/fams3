using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using MassTransit;

namespace BcGov.Fams3.SearchApi.Core.Adapters.Middleware
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