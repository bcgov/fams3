using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace DynamicsAdapter.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            try
            {
                Log.Information("Starting up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((hostingContext, loggerConfiguration) =>
                {
                    loggerConfiguration
                        .ReadFrom.Configuration(hostingContext.Configuration)
                        .Enrich.WithPropertySearchRequestKey("SearchRequestKey")
                        .Enrich.WithPropertyDataPartner("DataPartner")
                        .Enrich.FromLogContext();

                    string splunkCollectorUrl = hostingContext.Configuration["SPLUNK_COLLECTOR_URL"];
                    string splunkToken = hostingContext.Configuration["SPLUNK_TOKEN"];

                    if (!string.IsNullOrEmpty(splunkCollectorUrl) && !string.IsNullOrEmpty(splunkToken))
                    {
                       

                        loggerConfiguration.WriteTo.EventCollector(
                            splunkCollectorUrl,   
                            splunkToken,
                            sourceType: "Dynadapter",
                            restrictedToMinimumLevel: LogEventLevel.Debug,
                            messageHandler: new HttpClientHandler
                            {

                                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                            }
                        );
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }

    public static class EnrichersExtensions
    {
        public static LoggerConfiguration WithPropertySearchRequestKey(this LoggerEnrichmentConfiguration enrichmentConfiguration, string propertyName)
        {
            return enrichmentConfiguration.With(new SearchRequestKeyEnricher(propertyName));
        }

        public static LoggerConfiguration WithPropertyDataPartner(this LoggerEnrichmentConfiguration enrichmentConfiguration, string propertyName)
        {
            return enrichmentConfiguration.With(new DataPartnerEnricher(propertyName));
        }

    }

    public class SearchRequestKeyEnricher : ILogEventEnricher
    {
        private readonly string innerPropertyName;

        public SearchRequestKeyEnricher(string innerPropertyName)
        {
            this.innerPropertyName = innerPropertyName;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            LogEventPropertyValue eventPropertyValue;
            if (logEvent.Properties.TryGetValue(innerPropertyName, out eventPropertyValue))
            {
                var value = (eventPropertyValue as ScalarValue)?.Value as string;
                if (!String.IsNullOrEmpty(value))
                {
                    logEvent.AddOrUpdateProperty(new LogEventProperty(innerPropertyName, new ScalarValue("SearchRequestKey:" + value)));
                }
            }
        }
    }

    public class DataPartnerEnricher : ILogEventEnricher
    {
        private readonly string innerPropertyName;

        public DataPartnerEnricher(string innerPropertyName)
        {
            this.innerPropertyName = innerPropertyName;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            LogEventPropertyValue eventPropertyValue;
            if (logEvent.Properties.TryGetValue(innerPropertyName, out eventPropertyValue))
            {
                var value = (eventPropertyValue as ScalarValue)?.Value as string;
                if (!String.IsNullOrEmpty(value))
                {
                    logEvent.AddOrUpdateProperty(new LogEventProperty(innerPropertyName, new ScalarValue("- " + value)));
                }
            }
        }
    }
}
