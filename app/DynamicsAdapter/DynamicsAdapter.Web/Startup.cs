using System;
using DynamicsAdapter.Web.Configuration;
using DynamicsAdapter.Web.Infrastructure;
using DynamicsAdapter.Web.SearchApi;
using DynamicsAdapter.Web.SearchRequest;
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
            
            this.ConfigureOpenTracing(services);

            this.ConfigurePeopleClient(services);

            this.ConfigureScheduler(services);
        }


        public void ConfigurePeopleClient(IServiceCollection services)
        {
            services.AddHttpClient<IPeopleClient, PeopleClient>(c => c.BaseAddress = new Uri("http://localhost:5050"));
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
            services.AddSingleton<SearchRequestJob>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(SearchRequestJob),
                cronExpression: "0/5 * * * * ?"));

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
