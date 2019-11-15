using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicsAdapter.Web.Extensions;
using DynamicsAdapter.Web.SearchRequest;
using DynamicsAdapter.Web.SearchRequest.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DynamicsAdapter.Web.Health
{
    public class StatusReasonHealthCheck : IHealthCheck
    {
        public IStatusReasonService _statusReasonService;
        public StatusReasonHealthCheck(IStatusReasonService statusReasonService)
        {
            _statusReasonService = statusReasonService;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {

            if (await CheckStatusReason(cancellationToken))
            {
                return HealthCheckResult.Healthy("All Status Reason Exists in dynamics.");
            }

            return HealthCheckResult.Unhealthy("Different Status Reason Exists in dynamics");
        }

        private async Task<bool> CheckStatusReason(CancellationToken cancellationToken)
        {

            var statusReasonListFromDynamics = await _statusReasonService.GetListAsync(cancellationToken);

            if (statusReasonListFromDynamics?.OptionSet?.Options == null) return false;

            foreach (SearchRequestStatusReason reason in Enum.GetValues(typeof(SearchRequestStatusReason)).Cast<SearchRequestStatusReason>())
            {
                if (!statusReasonListFromDynamics.OptionSet.Options.Any(x => x.Value == (int)reason && string.Equals(x.Label.UserLocalizedLabel.Label.Replace(" ", ""), reason.GetName(), StringComparison.OrdinalIgnoreCase)))
                {
                    return false;
                }
            }
            return true;
        }

    }
}
