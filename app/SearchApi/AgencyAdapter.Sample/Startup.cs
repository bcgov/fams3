using System;
using BcGov.Fams3.SearchApi.Core.Configuration;
using HealthChecks.UI.Client;
using Jaeger;
using Jaeger.Samplers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OpenTracing.Util;
using BcGov.Fams3.SearchApi.Core.Adapters.Configuration;
using FluentValidation;
using AgencyAdapter.Sample.SearchRequest;
using MassTransit;
using BcGov.Fams3.SearchApi.Core.Adapters.Middleware;
using BcGov.Fams3.SearchApi.Contracts.SearchRequest;
using BcGov.Fams3.SearchApi.Core.OpenTracing;
using BcGov.Fams3.SearchApi.Core.MassTransit;

namespace AgencyAdapter.Sample
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
            services
                .AddMvc()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                });

            services.AddControllers();

            this.ConfigureHealthChecks(services);
            ConfigureFluentValidation(services);

            this.ConfigureOpenTracing(services);
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


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                // registration of health endpoints see https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });
        }


        private void ConfigureFluentValidation(IServiceCollection services)
        {
            services.AddTransient<IValidator<Models.SearchRequest>, SearchRequestValidator>();
        }


        private void ConfigureRabbitMQ(IServiceCollection services )
        {
           

                var rabbitMqSettings = Configuration.GetSection("RabbitMq").Get<RabbitMqConfiguration>();



            var rabbitBaseUri = $"amqp://{rabbitMqSettings.Host}:{rabbitMqSettings.Port}";

            services.AddTransient<IConsumeMessageObserver<SearchRequestOrdered>, SearchRequestObserver>();

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

                        cfg.ReceiveEndpoint($"{nameof(SearchRequestSaved)}_queue", e =>
                        {
                            e.Consumer(() =>
                             new SearchRequestResponseConsumer( provider.GetRequiredService<ILogger<SearchRequestResponseConsumer>>()));
                        });


                        // Add Diagnostic context for tracing
                        cfg.PropagateOpenTracingContext();

                    });

                    bus.ConnectConsumeMessageObserver(new SearchRequestObserver(
                        provider.GetRequiredService<ILogger<SearchRequestObserver>>()));

                    return bus;

                });

            });

            services.AddHostedService<BusHostedService>();

        }
    }
}
