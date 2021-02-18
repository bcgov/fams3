using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BcGov.ApiKey.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        public string _configApiKeyName = "_ApiKey";
        public static string HEADER_APIKEYNAME = "X-ApiKey"; 
        public static string TRUSTED_HOST_KEYNAME = "TrustedHosts";
        public ApiKeyMiddleware(RequestDelegate next, string configApiKeyName)
        {
            _next = next;
            _configApiKeyName = configApiKeyName;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();
            if (IsSwaggerRequest(context))
            {
                await _next(context);
            }
            else
            {
                if (IsTrustedHost(context, appSettings))
                {
                    if (!context.Request.Headers.TryGetValue(HEADER_APIKEYNAME, out var extractedApiKey))
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Api Key was not provided.");
                        return;
                    }

                    var apiKey = appSettings.GetValue<string>(_configApiKeyName);

                    if (!apiKey.Equals(extractedApiKey))
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Unauthorized client. ");
                        return;
                    }
                    else
                    {
                        await _next(context);
                    }
                }
                else
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("the request is not coming from trusted hosts.");
                    return;
                }
            }
        }

        private bool IsTrustedHost(HttpContext context, IConfiguration configuration)
        {
            string trustedHosts = configuration.GetValue<string>(TRUSTED_HOST_KEYNAME);
            if (trustedHosts != null)
            {
                if (trustedHosts == "*") return true;
                else
                {
                    string[] trustHosts = trustedHosts.ToString().Split(",");
                    return trustHosts.Contains(context.Request.Host.Host);                    
                }
            }
            return false;

        }

        private bool IsSwaggerRequest(HttpContext context)
        {
            if (context.Request.Path != null)
                return context.Request.Path.Value.Contains("/swagger/");
            return false;
        }

    }
}

