using System;
using BcGov.Fams3.SearchApi.Core.Adapters.Configuration;
using BcGov.Fams3.SearchApi.Core.Adapters.Contracts;
using BcGov.Fams3.SearchApi.Core.Adapters.Middleware;
using BcGov.Fams3.SearchApi.Core.Configuration;
using BcGov.Fams3.SearchApi.Core.MassTransit;
using BcGov.Fams3.SearchApi.Core.OpenTracing;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BcGov.Fams3.SearchApi.Core.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Configure MassTransit Service Bus
        /// http://masstransit-project.com/
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="function"></param>

        public static void AddProvider(this IServiceCollection services, IConfiguration configuration, Func<IServiceProvider, IConsumer<PersonSearchOrdered>> function)
        {

            var rabbitMqSettings = configuration.GetSection("RabbitMq").Get<RabbitMqConfiguration>();

            var rabbitBaseUri = $"amqp://{rabbitMqSettings.Host}:{rabbitMqSettings.Port}";

            services.AddTransient<IConsumeMessageObserver<PersonSearchOrdered>, PersonSearchObserver>();
            
            services.AddMassTransit(x =>
            {

                // Add RabbitMq Service Bus
                x.AddBus(provider =>
                {

                    var providerOptions = provider.GetRequiredService<IOptions<ProviderProfileOptions>>();

                    var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
                    {
                        var host = cfg.Host(new Uri(rabbitBaseUri), hostConfigurator =>
                        {
                            hostConfigurator.Username(rabbitMqSettings.Username);
                            hostConfigurator.Password(rabbitMqSettings.Password);
                        });

                        cfg.ReceiveEndpoint(host, $"{nameof(PersonSearchOrdered)}_{providerOptions.Value.Name}", e =>
                        {
                            e.Consumer(() => function.Invoke(provider));
                        });

                        // Add Provider profile context
                        cfg.UseProviderProfile(provider.GetRequiredService<IOptionsMonitor<ProviderProfileOptions>>()
                            .CurrentValue);

                        // Add Diagnostic context for tracing
                        cfg.PropagateOpenTracingContext();

                    });

                    bus.ConnectConsumeMessageObserver(new PersonSearchObserver(
                        provider.GetRequiredService<IOptions<ProviderProfileOptions>>(),
                        provider.GetRequiredService<ILogger<PersonSearchObserver>>()));

                    return bus;

                });

            });

            services.AddHostedService<BusHostedService>();
        }

       
    }
}
