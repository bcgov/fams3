using Microsoft.Extensions.Logging;
using Simple.OData.Client;
using System;

namespace Fams3Adapter.Dynamics.Error
{
    public static class DynamicsApiErrorLogger
    {
        public static void LogDynamicsError(Exception ex, ILogger logger)
        {
            logger.LogError(ex, "❌ Dynamics operation failed");

            var inner = ex.InnerException;
            while (inner != null)
            {
                logger.LogError("⛔ InnerException: {Message}", inner.Message);
                inner = inner.InnerException;
            }

            if (ex is WebRequestException webEx && webEx.Response != null)
            {
                try
                {
                    logger.LogError("📝 OData Response Content:\n{Response}", webEx.Response);
                }
                catch { }
            }
        }
    }
}