using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HealthChecks.UI.Client;
using AutoMapper;
using Jaeger;
using Jaeger.Samplers;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSwag;
using OpenTracing;
using OpenTracing.Util;
using SearchApi.Core.Adapters.Contracts;
using SearchApi.Core.Configuration;
using SearchApi.Core.Contracts;
using SearchApi.Core.MassTransit;
using SearchApi.Core.OpenTracing;
using SearchApi.Web.Configuration;
using SearchApi.Web.Controllers;
using SearchApi.Web.Notifications;
using SearchApi.Web.Search;
using SearchApi.Core.Adapters.Models;

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
        /// <remarks>
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


                    // Configure Person Search Accepted Consumer Consumer
                    cfg.ReceiveEndpoint(host, $"{nameof(PersonSearchAccepted)}_queue", e =>
                    {
                        e.Consumer(() =>
                            new PersonSearchAcceptedConsumer(provider.GetRequiredService<ISearchApiNotifier<PersonSearchAdapterEvent>>(), provider.GetRequiredService<ILogger<PersonSearchAcceptedConsumer>>()));
                    });
                    // Configure Person Search Accepted Consumer Consumer
                    cfg.ReceiveEndpoint(host, $"{nameof(PersonSearchAccepted)}_queue", e =>
                    {
                        e.Consumer(() =>
                            new PersonSearchCompletedConsumer(provider.GetRequiredService<ISearchApiNotifier<PersonSearchAdapterEvent>>(), provider.GetRequiredService<ILogger<PersonSearchCompletedConsumer>>()));
                    });

                }));
            });

            services.AddHostedService<BusHostedService>();

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwaggerUi3();
            }

            app.UseRouting();

            app.UseAuthorization();

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
    }
}
