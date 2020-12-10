using System;
using BcGov.Fams3.SearchApi.Contracts.Person;
using FluentValidation;
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
using Microsoft.Extensions.Options;
using OpenTracing;
using OpenTracing.Util;
using SearchAdapter.Sample.SearchRequest;
using BcGov.Fams3.SearchApi.Core.Adapters.Configuration;
using BcGov.Fams3.SearchApi.Core.Configuration;
using BcGov.Fams3.SearchApi.Core.DependencyInjection;
using SearchAdapter.Sample.SearchResult;
using SearchAdapter.Sample.IA;

namespace SearchAdapter.Sample
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }



        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
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

            // Configures the Provider Profile Options
            services
                .AddOptions<ProviderProfileOptions>()
                .Bind(Configuration.GetSection("ProviderProfile"))
                .ValidateDataAnnotations();

            this.ConfigureFluentValidation(services);

            this.ConfigureOpenTracing(services);

            ProviderProfileOptions conf = Configuration.GetSection("ProviderProfile").Get<ProviderProfileOptions>();

            if (conf.Name == "SAMPLE")
            {

                services.AddDataPartnerProvider(Configuration, (provider) => new SearchRequestConsumer(provider.GetRequiredService<IValidator<Person>>(),
                                      provider.GetRequiredService<IOptions<ProviderProfileOptions>>(),
                                      provider.GetRequiredService<IOptions<RetryConfiguration>>(),
                                      provider.GetRequiredService<ILogger<SearchRequestConsumer>>()));
            }
            else
            {
                services.AddDataPartnerProvider(Configuration, (provider) => new SearchRequestConsumer(provider.GetRequiredService<IValidator<Person>>(),
                                         provider.GetRequiredService<IOptions<ProviderProfileOptions>>(),
                                          provider.GetRequiredService<IOptions<RetryConfiguration>>(),
                                         provider.GetRequiredService<ILogger<SearchRequestConsumer>>()), (provider) => new SearchResultConsumer(
                                        provider.GetRequiredService<IOptions<ProviderProfileOptions>>(),
                                        provider.GetRequiredService<ILogger<SearchResultConsumer>>()));
            }

          //  services.AddIASearchDataPartner(Configuration, (provider) => new IASearchConsumer(provider.GetRequiredService<ILogger<IASearchConsumer>>()));
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
                // registration of health endpoints see https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });
        }

        /// <summary>
        /// Configures fluent validation see https://fluentvalidation.net/aspnet#asp-net-core
        /// </summary>
        private void ConfigureFluentValidation(IServiceCollection services)
        {
            services.AddTransient<IValidator<Person>, PersonSearchValidator>();
        }


       

    }
}
