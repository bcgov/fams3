using System;
using DynamicsAdapter.Web.Auth;
using DynamicsAdapter.Web.Configuration;
using DynamicsAdapter.Web.Infrastructure;
using DynamicsAdapter.Web.SearchRequest;
using DynamicsAdapter.Web.Services.Dynamics;
using DynamicsAdapter.Web.Services.Dynamics.Model;
using Jaeger;
using Jaeger.Samplers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OpenTracing.Util;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Simple.OData.Client;

namespace DynamicsAdapter.Web
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
            services.AddControllers();

            services.AddHealthChecks();

            // configure strongly typed settings objects
            var appSettings = ConfigureAppSettings(services);

            this.ConfigureOpenTracing(services);

            this.ConfigureSearchApi(services);

            ConfigureDynamicsClient(services);

            this.ConfigureScheduler(services);
        }

        private AppSettings ConfigureAppSettings(IServiceCollection services)
        {
           return Configuration.GetSection("AppSettings").Get<AppSettings>(); 
     
        }

        /// <summary>
        /// Configures the searchApi http client
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureSearchApi(IServiceCollection services)
        {
            var searchApiConfiguration = Configuration.GetSection("SearchApi").Get<SearchApiConfiguration>();

            services.AddHttpClient<ISearchApiClient, SearchApiClient>(c => c.BaseAddress = new Uri(searchApiConfiguration.BaseUrl));
        }

        public void ConfigureDynamicsClient(IServiceCollection services)
        {
            // Adding distributed cache
            services.AddDistributedMemoryCache();

            // Bind OAuth Configuration
            services.AddOptions<OAuthOptions>()
                .Bind(Configuration.GetSection("OAuth"))
                .ValidateDataAnnotations();

            // Add OAuth Middleware
            services.AddTransient<OAuthHandler>();

            // Register IOAuthApiClient
            services.AddHttpClient<IOAuthApiClient, OAuthApiClient>();

            // Register httpClient for OdataClient with OAuthHandler
            services.AddHttpClient<ODataClientSettings>(cfg =>
            {
                cfg.BaseAddress = new Uri(Configuration.GetSection("OAuth").Get<OAuthOptions>().ResourceUrl);
            }).AddHttpMessageHandler<OAuthHandler>();

            // Register httpClient for StatusReason Service with OAuthHandler
            services.AddHttpClient<IStatusReasonService, StatusReasonService>(cfg =>
            {
                cfg.BaseAddress = new Uri(Configuration.GetSection("OAuth").Get<OAuthOptions>().ResourceUrl);
            }).AddHttpMessageHandler<OAuthHandler>();

            // Register Odata client
            services.AddTransient<IODataClient>(provider =>
                new ODataClient(provider.GetRequiredService<ODataClientSettings>()));

            // Add other Services
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<ISearchRequestService, SearchRequestService>();
            services.AddTransient<IStatusReasonService, StatusReasonService>();

        }
        /// <summary>
        /// Configures the Quartz Hosted Service.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureScheduler(IServiceCollection services)
        {

            var schedulerConfiguration = Configuration.GetSection("Scheduler").Get<SchedulerConfiguration>();

            //Add Quartz Service
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            //Add dynamics Job
            services.AddTransient<SearchRequestJob>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(SearchRequestJob),
                cronExpression: schedulerConfiguration.Cron));

            //Registering the Quartz Hosted Service
            services.AddHostedService<QuartzHostedService>();



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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // registration of health endpoints see https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
            });
        }
    }
}
