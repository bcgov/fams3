using BcGov.Fams3.SearchApi.Contracts.SearchRequest;
using BcGov.Fams3.SearchApi.Core.Configuration;
using BcGov.Fams3.SearchApi.Core.MassTransit;
using BcGov.Fams3.SearchApi.Core.OpenTracing;
using FluentValidation;
using Jaeger;
using Jaeger.Samplers;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSwag;
using HealthChecks.UI.Client;
using OpenTracing;
using OpenTracing.Util;
using SearchRequest.Adaptor.Notifier.Models;
using SearchRequest.Adaptor.Notifier.Models.Validation;
using SearchRequestAdaptor.Configuration;
using SearchRequestAdaptor.Consumer;
using SearchRequestAdaptor.Notifier;
using SearchRequestAdaptor.Publisher;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using GreenPipes;
using BcGov.ApiKey.Middleware;

namespace SearchRequestAdaptor
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            });

            services.AddControllers();
            services.AddOptions<SearchRequestAdaptorOptions>().Bind(Configuration.GetSection(Keys.SEARCHREQUEST_SECTION_SETTING_KEY));
            services.AddWebHooks();
            services.AddTransient<IValidator<Notification>, NotificationValidator>();

            this.ConfigureHealthChecks(services);
            this.ConfigureServiceBus(services);
            ConfigureOpenApi(services);
            ConfigureOpenTracing(services);
            

        }
        public void ConfigureOpenApi(IServiceCollection services)
        {
            services.AddSwaggerDocument(config =>
            {
                config.OperationProcessors.Add(new SwaggerApiKeyHeader());
                // configure swagger properties
                config.PostProcess = document =>
                {

                    document.Info.Version = "V0.1";
                    document.Info.Description = "To Recieve  Notification from Dynamics";
                    document.Info.Title = $"Fams notification";
                    document.Tags = new List<OpenApiTag>()
                    {
                        new OpenApiTag() {
                            Name = "Notification Api",
                            Description = $"The FAMS api to recieve notification from dynamics"
                        }
                    };
                };
            });

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
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("Pragma", "no-cache");
                await next();
            });
            app.UseRouting();
            app.UseMiddleware<ApiKeyMiddleware>("RequestApi_ApiKey");
            app.UseOpenApi();
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

        private void ConfigureHealthChecks(IServiceCollection services)
        {
            var rabbitMqSettings = Configuration.GetSection("RabbitMq").Get<RabbitMqConfiguration>();
            var rabbitConnectionString = $"amqp://{rabbitMqSettings.Username}:{rabbitMqSettings.Password}@{rabbitMqSettings.Host}:{rabbitMqSettings.Port}";

            services
                .AddHealthChecks()
                .AddRabbitMQ(
                    rabbitConnectionString: rabbitConnectionString);
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


                    // Configure Search Request Ordered Consumer 
                    cfg.ReceiveEndpoint($"{nameof(SearchRequestOrdered)}_queue", e =>
                    {

                        e.UseRateLimit(1, TimeSpan.FromSeconds(5));

                        e.Consumer(() =>
                            new SearchRequestOrderedConsumer(
                                provider.GetRequiredService<ISearchRequestNotifier<SearchRequestOrdered>>(),
                                provider.GetRequiredService<ILogger<SearchRequestOrderedConsumer>>()));

                    
                    
                    });
                }));
            });

            services.AddHostedService<BusHostedService>();
            services.AddTransient<ISearchRequestEventPublisher, SearchRequestEventPublisher>();
        }
    }
}
