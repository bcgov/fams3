using AutoMapper;
using BcGov.ApiKey.Middleware;
using BcGov.Fams3.Redis.DependencyInjection;
using DynamicsAdapter.Web.ApiGateway;
using DynamicsAdapter.Web.Auth;
using DynamicsAdapter.Web.Configuration;
using DynamicsAdapter.Web.Health;
using DynamicsAdapter.Web.Infrastructure;
using DynamicsAdapter.Web.PersonSearch;
using DynamicsAdapter.Web.Register;
using DynamicsAdapter.Web.SearchAgency;
using DynamicsAdapter.Web.SearchAgency.Models;
using DynamicsAdapter.Web.SearchAgency.Validation;
using DynamicsAdapter.Web.SearchAgency.Webhook;
using DynamicsAdapter.Web.SearchRequest;
using Fams3Adapter.Dynamics.DataProvider;
using Fams3Adapter.Dynamics.Duplicate;
using Fams3Adapter.Dynamics.OptionSets;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.SearchResponse;
using FluentValidation;
using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using Jaeger;
using Jaeger.Samplers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSwag;
using OpenTracing;
using OpenTracing.Util;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Simple.OData.Client;
using StackExchange.Redis.Extensions.Core.Configuration;
using System;
using System.Collections.Generic;

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
            }).AddFluentValidation();


            services.AddControllers();

            services.AddOptions<AgencyNotificationOptions>().Bind(Configuration.GetSection(Keys.AGENCY_NOTIFICATION_WEB_HOOK_SETTING_KEY));
            services.AddOptions<SearchApiConfiguration>().Bind(Configuration.GetSection("SearchApi"));
            services.AddHttpClient<IAgencyNotificationWebhook<SearchRequestNotification>, AgencyNotificationWebhook>();

            services.AddHealthChecks().AddCheck<DynamicsHealthCheck>("status_reason_health_check", failureStatus: HealthStatus.Degraded);

            this.ConfigureOpenTracing(services);
            this.ConfigureOpenApi(services);
            this.ConfigureSearchApi(services);

            this.ConfigureDynamicsClient(services);

            this.ConfigureScheduler(services);
            this.ConfigureAutoMapper(services);
            this.ConfigureFluentValidation(services);
            services.AddCacheService(Configuration.GetSection(Keys.REDIS_SECTION_SETTING_KEY).Get<RedisConfiguration>());
          

            services.AddTransient<ISearchResultService, SearchResultService>();
            services.AddTransient<ISearchRequestRegister, SearchRequestRegister>();

        }

        private void ConfigureFluentValidation(IServiceCollection services)
        {
            services.AddTransient<IValidator<SearchResponseReady>, SearchResponseReadyValidator>();
        }

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

            services.AddOptions<ApiGatewayOptions>()
                .Bind(Configuration.GetSection(Keys.API_GATEWAY_CONFIGURATION_KEY))
                .ValidateDataAnnotations();

            var oAuthOptions = Configuration.GetSection("OAuth").Get<OAuthOptions>();


            // Add OAuth Middleware
            services.AddTransient<OAuthHandler>();

            // Add Api Gateway Middleware
            services.AddTransient<ApiGatewayHandler>();

            // Register IOAuthApiClient
            services.AddHttpClient<IOAuthApiClient, OAuthApiClient>();



            // Register httpClient for OdataClient with OAuthHandler
            services.AddHttpClient<ODataClientSettings>(cfg => { cfg.BaseAddress = new Uri(oAuthOptions.ResourceUrl); })
                .AddHttpMessageHandler<OAuthHandler>()
                .AddHttpMessageHandler<ApiGatewayHandler>();

            // Register httpClient for StatusReason Service with OAuthHandler
            services.AddHttpClient<IOptionSetService, OptionSetService>(cfg => { cfg.BaseAddress = new Uri(oAuthOptions.ResourceUrl); })
                .AddHttpMessageHandler<OAuthHandler>()
                .AddHttpMessageHandler<ApiGatewayHandler>();

            // Register Odata client
            //services.AddTransient<IODataClient>(provider =>
            services.AddSingleton<IODataClient>(provider =>
            {
                 var settings = provider.GetRequiredService<ODataClientSettings>();
                 settings.IgnoreUnmappedProperties = true;
                 return new ODataClient(settings);
             });

            // Add other Services
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<ISearchApiRequestService, SearchApiRequestService>();
            services.AddTransient<ISearchRequestService, SearchRequestService>();
            services.AddTransient<ISearchResponseService, SearchResponseService>();
            services.AddTransient<IDataPartnerService, DataPartnerService>();
            services.AddSingleton<IDuplicateDetectionService, DuplicateDetectionService>();
            services.AddTransient<IAgencyRequestService, AgencyRequestService>();
            services.AddTransient<IAgencyResponseService, AgencyResponseService>();
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

            services.AddTransient<FailedSearchRequestJob>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(FailedSearchRequestJob),
                cronExpression: schedulerConfiguration.Failed));

            //Registering the Quartz Hosted Service
            services.AddHostedService<QuartzHostedService>();



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
                config.OperationProcessors.Add(new SwaggerApiKeyHeader());
                // configure swagger properties
                config.PostProcess = document =>
                {
                    document.Info.Version = "V0.1";
                    document.Info.Description = "For Dynamics APi Call";
                    document.Info.Title = "FAMS  API";
                    document.Tags = new List<OpenApiTag>()
                    {
                        new OpenApiTag() {
                            Name = "Person Search Events API",
                            Description = "The FAMS People API"
                        }
                    };
                };
            });

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
                app.UseSwaggerUi3();
            }
            app.Use(async (context, next) =>
            {
                context.Response.GetTypedHeaders().CacheControl =
                 new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
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

            app.UseAuthorization();

            app.UseMiddleware<ApiKeyMiddleware>("ApiKey");

            app.UseOpenApi();

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
