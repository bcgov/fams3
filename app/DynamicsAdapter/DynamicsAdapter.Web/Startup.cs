using System;
using AutoMapper;
using DynamicsAdapter.Web.Auth;
using DynamicsAdapter.Web.Configuration;
using DynamicsAdapter.Web.Health;
using DynamicsAdapter.Web.Infrastructure;
using DynamicsAdapter.Web.SearchRequest;
using Fams3Adapter.Dynamics.OptionSets;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Fams3Adapter.Dynamics.SearchRequest;
using HealthChecks.UI.Client;
using Jaeger;
using Jaeger.Samplers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
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

            services.AddMvc().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            });


            services.AddControllers();

            services.AddHealthChecks().AddCheck<StatusReasonHealthCheck>("status_reason_health_check",failureStatus:HealthStatus.Degraded);

            // configure strongly typed settings objects
            var appSettings = ConfigureAppSettings(services);

            this.ConfigureOpenTracing(services);

            this.ConfigureSearchApi(services);

            this.ConfigureDynamicsClient(services);

            this.ConfigureScheduler(services);
            this.ConfigureAutoMapper(services);
        }

        private AppSettings ConfigureAppSettings(IServiceCollection services)
        {
           return Configuration.GetSection("AppSettings").Get<AppSettings>(); 
     
        }

        //private void ConfigureAutoMapper(IServiceCollection services)
        //{
        //    var configuration = new MapperConfiguration(cfg =>
        //    {
        //        cfg.AddProfile<SearchRequestAutoMapperProfile>();                
        //    });
        //    // only during development, validate your mappings; remove it before release
        //    configuration.AssertConfigurationIsValid();
        //    services.AddSingleton(configuration.CreateMapper());
        //}
        public void ConfigureAutoMapper(IServiceCollection services)
        {
            services.AddAutoMapper(System.Reflection.Assembly.GetExecutingAssembly());
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

            var dynamicsApiOptions = Configuration.GetSection(Keys.DYNAMICS_CONFIGURATION_KEY).Get<DynamicsApiOptions>();
          
            // Add OAuth Middleware
            services.AddTransient<OAuthHandler>();

            // Register IOAuthApiClient
            services.AddHttpClient<IOAuthApiClient, OAuthApiClient>();

            // Register httpClient for OdataClient with OAuthHandler
            services.AddHttpClient<ODataClientSettings>(cfg =>
            {
                cfg.BaseAddress = new Uri(dynamicsApiOptions.BasePath);
            }).AddHttpMessageHandler<OAuthHandler>();

            // Register httpClient for StatusReason Service with OAuthHandler
            services.AddHttpClient<IOptionSetService, OptionSetService>(cfg =>
            {
                cfg.BaseAddress = new Uri(dynamicsApiOptions.BasePath);
            }).AddHttpMessageHandler<OAuthHandler>();

            // Register Odata client
            services.AddTransient<IODataClient>(provider =>
                new ODataClient(provider.GetRequiredService<ODataClientSettings>()));

            // Add other Services
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<ISearchApiRequestService, SearchApiRequestService>();
            services.AddTransient<ISearchRequestService, SearchRequestService>();

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
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    AllowCachingResponses = false,
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
