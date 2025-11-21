using BcGov.Fams3.Utils.Logger;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Linq;

namespace SearchRequestAdaptor
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            var namespaceMain = typeof(Program).Namespace;

            try
            {
                Log.Information("Starting up: {Namespace}.Main() ---> .NET Runtime Version: {Version}",
                    namespaceMain,
                    RuntimeInformation.FrameworkDescription);

                // Log important environment variables
                var envVars = Environment.GetEnvironmentVariables();
                var sortedEnvVars = envVars.Cast<System.Collections.DictionaryEntry>()
                    .Where(e => !string.IsNullOrEmpty(e.Key?.ToString()))
                    .OrderBy(e => e.Key.ToString(), StringComparer.OrdinalIgnoreCase);

                var envLogBuilder = new System.Text.StringBuilder();
                envLogBuilder.AppendLine("\n  All Environment Variables (excluding sensitive info):");

                foreach (var env in sortedEnvVars)
                {
                    var key = env.Key?.ToString();
                    var value = env.Value?.ToString();
                    var lowerKey = key.ToLowerInvariant();
                    if (lowerKey.Contains("password") || lowerKey.Contains("username") || lowerKey.Contains("token") || lowerKey.Contains("secret"))
                    {
                        envLogBuilder.AppendLine($"\t{key}: [REDACTED]");
                        continue;
                    }
                    envLogBuilder.AppendLine($"\t{key}: {value}");
                }
                Log.Debug("{EnvDump}", envLogBuilder.ToString());

                var builder = Host.CreateDefaultBuilder(args)
                    .UseSerilog((hostingContext, loggerConfiguration) =>
                    {
                        string serviceName = hostingContext.Configuration["JAEGER_SERVICE_NAME"] ?? "request-api";

                        loggerConfiguration
                            .ReadFrom.Configuration(hostingContext.Configuration)
                            .Enrich.WithPropertyRequestRef("RequestRef")
                            .Enrich.WithPropertyAgencyCode("AgencyCode")
                            .Enrich.WithPropertySearchRequestKey("SearchRequestKey")
                            .Enrich.WithProperty("ServiceName", serviceName)
                            .Enrich.FromLogContext();

                        string splunkCollectorUrl = hostingContext.Configuration["SPLUNK_COLLECTOR_URL"];
                        string splunkToken = hostingContext.Configuration["SPLUNK_TOKEN"];

                        if (!string.IsNullOrEmpty(splunkCollectorUrl) && !string.IsNullOrEmpty(splunkToken))
                        {
                            loggerConfiguration.WriteTo.EventCollector(
                                splunkCollectorUrl,
                                splunkToken,
                                sourceType: "RequestAdaptor",
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

                builder.Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "{Namespace} Application start-up failed", namespaceMain);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
