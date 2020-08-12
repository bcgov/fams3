using BcGov.Fams3.SearchApi.Contracts.SearchRequest;
using BcGov.Fams3.SearchApi.Core.Configuration;
using BcGov.Fams3.SearchApi.Core.MassTransit;
using BcGov.Fams3.SearchApi.Core.OpenTracing;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SearchRequest.Adaptor.Notifier.Models;
using SearchRequest.Adaptor.Notifier.Models.Validation;
using SearchRequestAdaptor.Configuration;
using SearchRequestAdaptor.Consumer;
using SearchRequestAdaptor.Notifier;
using SearchRequestAdaptor.Publisher;
using System;

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
            services.AddControllers();
            services.AddOptions<SearchRequestAdaptorOptions>().Bind(Configuration.GetSection(Keys.SEARCHREQUEST_SECTION_SETTING_KEY));
            services.AddWebHooks();
            services.AddTransient<IValidator<Notification>, NotificationValidator>();

            this.ConfigureHealthChecks(services);
            this.ConfigureServiceBus(services);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
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
