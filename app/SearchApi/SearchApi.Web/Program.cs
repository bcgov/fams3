using BcGov.Fams3.Utils.Logger;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.Net.Http;

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
                        .Enrich.WithPropertyDataPartner("DataPartner")
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
                            restrictedToMinimumLevel: LogEventLevel.Information,
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
}
