using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace SearchApi.Web
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
                        .Enrich.FromLogContext();

                    string splunkCollectorUrl = hostingContext.Configuration["SPLUNK_COLLECTOR_URL"];
                    string splunkToken = hostingContext.Configuration["SPLUNK_TOKEN"];

                    if (!string.IsNullOrEmpty(splunkCollectorUrl) && !string.IsNullOrEmpty(splunkToken))
                    {
                        // enable Splunk logger using Serilog
                        //var fields = new Serilog.Sinks.Splunk.CustomFields();
                        //if (!string.IsNullOrEmpty(hostingContext.Configuration["SPLUNK_CHANNEL"]))
                        //{
                        //    fields.CustomFieldList.Add(new Serilog.Sinks.Splunk.CustomField("channel", hostingContext.Configuration["SPLUNK_CHANNEL"]));
                        //}

                        loggerConfiguration.WriteTo.EventCollector(
                            splunkCollectorUrl,
                            splunkToken,
                            sourceType: "SearchApi",
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
}
