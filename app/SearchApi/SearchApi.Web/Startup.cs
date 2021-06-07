using System;
using System.Collections.Generic;
using HealthChecks.UI.Client;
using AutoMapper;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using Jaeger;
using Jaeger.Samplers;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSwag;
using OpenTracing;
using OpenTracing.Util;
using BcGov.Fams3.SearchApi.Core.Configuration;
using BcGov.Fams3.SearchApi.Core.MassTransit;
using BcGov.Fams3.SearchApi.Core.OpenTracing;
using SearchApi.Web.Configuration;
using SearchApi.Web.Notifications;
using SearchApi.Web.Search;

using SearchApi.Web.Messaging;
using BcGov.Fams3.Redis.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using SearchApi.Web.DeepSearch;
using StackExchange.Redis.Extensions.Core.Configuration;
using GreenPipes;

namespace SearchApi.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvc().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            });

            services.AddControllers();

            // Bind OAuth Configuration
            services.AddOptions<SearchApiOptions>()
                .Bind(Configuration.GetSection(Keys.SEARCHAPI_SECTION_SETTING_KEY));

            services.AddOptions<DeepSearchOptions>()
                .Bind(Configuration.GetSection(Keys.DEEPSEARCH_SECTION_SETTING_KEY));
      
          services.AddCacheService(Configuration.GetSection(Keys.REDIS_SECTION_SETTING_KEY).Get<RedisConfiguration>());

            services.AddWebHooks();

            this.ConfigureHealthChecks(services);

            this.ConfigureOpenTracing(services);

            this.ConfigureOpenApi(services);

            this.ConfigureServiceBus(services);

            this.ConfigureAutoMapper(services);
        }
        public void ConfigureAutoMapper(IServiceCollection services)
        {
            services.AddAutoMapper(System.Reflection.Assembly.GetExecutingAssembly());
        }
            private void ConfigureHealthChecks(IServiceCollection services)
        {

            var rabbitMqSettings = Configuration.GetSection("RabbitMq").Get<RabbitMqConfiguration>();
            var rabbitConnectionString = $"amqp://{rabbitMqSettings.Username}:{rabbitMqSettings.Password}@{rabbitMqSettings.Host}:{rabbitMqSettings.Port}";

            services
                .AddHealthChecks()
                .AddRabbitMQ(
                    rabbitMQConnectionString: rabbitConnectionString);
        }

        /// <summary>
        /// Configures OpenTracing with Jaeger Instrumentation from Environment Variables
        /// https://github.com/jaegertracing/jaeger-client-csharp
        /// </summary>
        // <remarks>
        /// The `JAEGER_SERVICE_NAME` variable is required to be set
        /// </remarks>
        /// <param name="services"></param>
        private void ConfigureOpenTracing(IServiceCollection services)
        {

            services.AddOpenTracing();

            services.AddSingleton<ITracer>(serviceProvider =>
            {

                ITracer tracer;

                try
                {
                    tracer = Jaeger.Configuration.FromEnv(serviceProvider.GetService<ILoggerFactory>()).GetTracer();
                    
                }
                catch (ArgumentException ex)
                {
                    if (ex.Message == "Service name must not be null or empty")
                    {
                        tracer = new Tracer.Builder(serviceProvider.GetRequiredService<IHostEnvironment>().ApplicationName)
                            .WithSampler(new ConstSampler(false))
                            .Build();
                    }
                    else
                    {
                        throw;
                    }
                }

                GlobalTracer.Register(tracer);
                return tracer;

            });

        }

        /// <summary>
        /// Configure Open Api using NSwag
        /// https://github.com/RicoSuter/NSwag
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureOpenApi(IServiceCollection services)
        {

            services.AddSwaggerDocument(config =>
            {
                // configure swagger properties
                config.PostProcess = document =>
                {
                    document.Info.Version = "V0.1";
                    document.Info.Description = "For Search";
                    document.Info.Title = "FAMS Search API";
                    document.Tags = new List<OpenApiTag>()
                    {
                        new OpenApiTag() {
                            Name = "People API",
                            Description = "The FAMS People API"
                        } 
                    };
                };
            });

        }

      

        /// <summary>
        /// Configure MassTransit Service Bus
        /// http://masstransit-project.com/
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureServiceBus(IServiceCollection services)
        {

            services.Configure<RabbitMqConfiguration>(Configuration.GetSection("RabbitMq"));

            var rabbitMqSettings = Configuration.GetSection("RabbitMq").Get<RabbitMqConfiguration>();
            var rabbitBaseUri = $"amqp://{rabbitMqSettings.Host}:{rabbitMqSettings.Port}";

            var queueRateLimit = Configuration.GetSection("QueueRateLimit").Get<QueueRateLimit>();
            if (queueRateLimit == null)
                queueRateLimit = new QueueRateLimit();

            services.AddMassTransit(x =>
            {
                // Add RabbitMq Service Bus
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    
                    var host = cfg.Host(new Uri(rabbitBaseUri), hostConfigurator =>
                    {
                        hostConfigurator.Username(rabbitMqSettings.Username);
                        hostConfigurator.Password(rabbitMqSettings.Password);
                    });

                    // Add Diagnostic context for tracing
                    cfg.PropagateOpenTracingContext();


                    // Configure Person Search Accepted Consumer 
                    cfg.ReceiveEndpoint( $"{nameof(PersonSearchAccepted)}_queue", e =>
                    {
                        e.UseConcurrencyLimit(queueRateLimit.PersonSearchAccepted_ConcurrencyLimit);
                        e.UseRateLimit(queueRateLimit.PersonSearchAccepted_RateLimit, TimeSpan.FromSeconds(queueRateLimit.PersonSearchAccepted_RateInterval));
                        e.Consumer(() =>
                            new PersonSearchAcceptedConsumer(provider.GetRequiredService<ISearchApiNotifier<PersonSearchAdapterEvent>>(), provider.GetRequiredService<ILogger<PersonSearchAcceptedConsumer>>()));
                    });
                    // Configure Person Search Completed Consumer 
                    cfg.ReceiveEndpoint( $"{nameof(PersonSearchCompleted)}_queue", e =>
                    {
                        e.UseConcurrencyLimit(queueRateLimit.PersonSearchCompleted_ConcurrencyLimit);
                        e.UseRateLimit(queueRateLimit.PersonSearchCompleted_RateLimit, TimeSpan.FromSeconds(queueRateLimit.PersonSearchCompleted_RateInterval));
                        e.Consumer(() =>
                            new PersonSearchCompletedConsumer(provider.GetRequiredService<ISearchApiNotifier<PersonSearchAdapterEvent>>(), provider.GetRequiredService<ILogger<PersonSearchCompletedConsumer>>()));
                    });
                    // Configure Person Search Completed Consumer 
                    cfg.ReceiveEndpoint($"{nameof(PersonSearchCompletedJCA)}_queue", e =>
                    {
                        e.UseConcurrencyLimit(queueRateLimit.PersonSearchCompletedJCA_ConcurrencyLimit);
                        e.UseRateLimit(queueRateLimit.PersonSearchCompletedJCA_RateLimit, TimeSpan.FromSeconds(queueRateLimit.PersonSearchCompletedJCA_RateInterval));
                        e.Consumer(() =>
                            new PersonSearchCompletedJCAConsumer(provider.GetRequiredService<ISearchApiNotifier<PersonSearchAdapterEvent>>(), provider.GetRequiredService<ILogger<PersonSearchCompletedJCAConsumer>>()));
                    });

                    // Configure Person Search Rejected Consumer 
                    cfg.ReceiveEndpoint( $"{nameof(PersonSearchRejected)}_queue", e =>
                    {
                        e.UseConcurrencyLimit(queueRateLimit.PersonSearchRejected_ConcurrencyLimit);
                        e.UseRateLimit(queueRateLimit.PersonSearchRejected_RateLimit, TimeSpan.FromSeconds(queueRateLimit.PersonSearchRejected_RateInterval));
                        e.Consumer(() =>
                            new PersonSearchRejectedConsumer(provider.GetRequiredService<ISearchApiNotifier<PersonSearchAdapterEvent>>(), provider.GetRequiredService<ILogger<PersonSearchRejectedConsumer>>()));
                    });

                    // Configure Person Search Failed Consumer 
                    cfg.ReceiveEndpoint( $"{nameof(PersonSearchFailed)}_queue", e =>
                    {
                        e.UseConcurrencyLimit(queueRateLimit.PersonSearchFailed_ConcurrencyLimit);
                        e.UseRateLimit(queueRateLimit.PersonSearchFailed_RateLimit, TimeSpan.FromSeconds(queueRateLimit.PersonSearchFailed_RateInterval));
                        e.Consumer(() =>
                            new PersonSearchFailedConsumer(provider.GetRequiredService<ISearchApiNotifier<PersonSearchAdapterEvent>>(), provider.GetRequiredService<ILogger<PersonSearchFailedConsumer>>()));
                    });

                    // Configure Person Search Submitted Consumer 
                    cfg.ReceiveEndpoint($"{nameof(PersonSearchSubmitted)}_queue", e =>
                    {
                        e.UseConcurrencyLimit(queueRateLimit.PersonSearchSubmitted_ConcurrencyLimit);
                        e.UseRateLimit(queueRateLimit.PersonSearchSubmitted_RateLimit, TimeSpan.FromSeconds(queueRateLimit.PersonSearchSubmitted_RateInterval));
                        e.Consumer(() =>
                            new PersonSearchSubmittedConsumer(provider.GetRequiredService<ISearchApiNotifier<PersonSearchAdapterEvent>>(), provider.GetRequiredService<ILogger<PersonSearchSubmittedConsumer>>()));
                    });

                    // Configure Person Search Information Consumer 
                    cfg.ReceiveEndpoint($"{nameof(PersonSearchInformation)}_queue", e =>
                    {
                        e.UseConcurrencyLimit(queueRateLimit.PersonSearchInformation_ConcurrencyLimit);
                        e.UseRateLimit(queueRateLimit.PersonSearchInformation_RateLimit, TimeSpan.FromSeconds(queueRateLimit.PersonSearchInformation_RateInterval));
                        e.Consumer(() =>
                            new PersonSearchInformationConsumer(provider.GetRequiredService<ISearchApiNotifier<PersonSearchAdapterEvent>>(), provider.GetRequiredService<ILogger<PersonSearchInformationConsumer>>()));
                    });


                }));
            });

            services.AddHostedService<BusHostedService>();
            services.AddTransient<IDispatcher, Dispatcher>();
            services.AddTransient<IDeepSearchDispatcher, DeepSearchDispatcher>();
            //services.AddTransient<IDeepSearchService, DeepSearchService>();
            services.AddSingleton<IDeepSearchService, DeepSearchService>();




        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwaggerUi3();
            }

         app.Use(async (context, next) =>
            {
                context.Response.GetTypedHeaders().CacheControl =
                 new CacheControlHeaderValue()
                 {
                     NoStore = true,
                     NoCache = true,
                     MustRevalidate = true,
                     MaxAge = TimeSpan.FromSeconds(0),
                     Private = true,
                     

                 };
                context.Response.Headers.Add("Pragma", "no-cache");
                   context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                await next();
            });
            app.UseRouting();

            app.UseAuthorization();

            app.UseOpenApi();

            app.Use(async (context, next) =>
            {
                await next();
            });

            app.UseEndpoints(endpoints =>
            {
                // registration of health endpoints see https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapControllers();
            });
        }
    }
}
