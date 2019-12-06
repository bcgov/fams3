using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration.Binder;
using SearchApi.Core.Adapters.Configuration;
using SearchApi.Core.Configuration;
using MassTransit;
using SearchApi.Core.Adapters.Contracts;
using SearchApi.Core.Adapters.Middleware;
using SearchApi.Core.MassTransit;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System;
using SearchApi.Core.OpenTracing;

namespace SearchApi.Core.Registrations
{
    public static class IServiceCollectionExtensions
    {
     
       public static void AddProvider(this IServiceCollection services, IConfiguration configuration, IConsumer<PersonSearchOrdered> consumer)
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
                            e.Consumer(() => consumer);
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
