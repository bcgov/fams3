using System;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using BcGov.Fams3.SearchApi.Contracts.SearchRequest;
using BcGov.Fams3.SearchApi.Core.Adapters.Configuration;
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

        public static void AddDataPartnerProvider(this IServiceCollection services, IConfiguration configuration, Func<IServiceProvider, IConsumer<PersonSearchOrdered>> ordered, Func<IServiceProvider, IConsumer<PersonSearchReceived>> recieved = null)
        {
            if (ordered == null) throw new ArgumentNullException("Consumer for search ordered cannot be null");
            var rabbitMqSettings = configuration.GetSection(Keys.RABBITMQ_SECTION_SETTING_KEY).Get<RabbitMqConfiguration>();
            var providerConfiguration = configuration.GetSection(Keys.PROVIDER_SECTION_SETTING_KEY).Get<ProviderProfileOptions>();


            // Configures the Provider Profile Options
            services
                .AddOptions<ProviderProfileOptions>()
                .Bind(configuration.GetSection(Keys.PROVIDER_SECTION_SETTING_KEY))
                .ValidateDataAnnotations();


            var rabbitBaseUri = $"amqp://{rabbitMqSettings.Host}:{rabbitMqSettings.Port}";

            services.AddTransient<IConsumeMessageObserver<PersonSearchOrdered>, PersonSearchObserver>();
            
            services.AddMassTransit(x =>
            {

                // Add RabbitMq Service Bus
                x.AddBus(provider =>
                {

                    var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
                    {
                        var host = cfg.Host(new Uri(rabbitBaseUri), hostConfigurator =>
                        {
                            hostConfigurator.Username(rabbitMqSettings.Username);
                            hostConfigurator.Password(rabbitMqSettings.Password);
                        });
                        if (recieved == null)
                        {
                            cfg.ReceiveEndpoint($"{nameof(PersonSearchOrdered)}_{providerConfiguration.Name}", e =>
                            {
                                e.Consumer(() => ordered.Invoke(provider));
                            });
                        }
                        else
                        {
                            cfg.ReceiveEndpoint($"{nameof(PersonSearchReceived)}_{providerConfiguration.Name}", e =>
                            {
                                e.Consumer(() => recieved.Invoke(provider));
                            });

                            cfg.ReceiveEndpoint($"{nameof(PersonSearchOrdered)}_{providerConfiguration.Name}", e =>
                            {
                                e.Consumer(() => ordered.Invoke(provider));
                            });
                        }

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


        /// <summary>
        /// Configure MassTransit Service Bus
        /// http://masstransit-project.com/
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="function"></param>

        public static void AddRequestAgencyProvider(this IServiceCollection services, IConfiguration configuration, Func<IServiceProvider, IConsumer<SearchRequestOrdered>> ordered)
        {
            if (ordered == null) throw new ArgumentNullException("Consumer for search request ordered cannot be null");
            var rabbitMqSettings = configuration.GetSection(Keys.RABBITMQ_SECTION_SETTING_KEY).Get<RabbitMqConfiguration>();
            var providerConfiguration = configuration.GetSection(Keys.PROVIDER_SECTION_SETTING_KEY).Get<ProviderProfileOptions>();


            // Configures the Provider Profile Options
            services
                .AddOptions<ProviderProfileOptions>()
                .Bind(configuration.GetSection(Keys.PROVIDER_SECTION_SETTING_KEY))
                .ValidateDataAnnotations();


            var rabbitBaseUri = $"amqp://{rabbitMqSettings.Host}:{rabbitMqSettings.Port}";

            services.AddTransient<IConsumeMessageObserver<PersonSearchOrdered>, PersonSearchObserver>();

            services.AddMassTransit(x =>
            {

                // Add RabbitMq Service Bus
                x.AddBus(provider =>
                {

                    var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
                    {
                        var host = cfg.Host(new Uri(rabbitBaseUri), hostConfigurator =>
                        {
                            hostConfigurator.Username(rabbitMqSettings.Username);
                            hostConfigurator.Password(rabbitMqSettings.Password);
                        });
                       
                        cfg.ReceiveEndpoint($"{nameof(SearchRequestSubmitted)}_queue", e =>
                        {
                                e.Consumer(() => ordered.Invoke(provider));
                        });

                        cfg.ReceiveEndpoint($"{nameof(SearchRequestFailed)}_queue", e =>
                        {
                            e.Consumer(() => ordered.Invoke(provider));
                        });

                        cfg.ReceiveEndpoint($"{nameof(SearchRequestRejected)}_queue", e =>
                        {
                            e.Consumer(() => ordered.Invoke(provider));
                        });

                        cfg.ReceiveEndpoint($"{nameof(SearchRequestNotification)}_queue", e =>
                        {
                            e.Consumer(() => ordered.Invoke(provider));
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
