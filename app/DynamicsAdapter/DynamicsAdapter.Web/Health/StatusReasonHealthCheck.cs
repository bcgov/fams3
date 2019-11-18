using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicsAdapter.Web.SearchRequest;
using Fams3Adapter.Dynamics.SearchApiRequest;
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

            foreach (SearchApiRequestStatusReason reason in Enum.GetValues(typeof(SearchApiRequestStatusReason)).Cast<SearchApiRequestStatusReason>())
            {
                if (!statusReasonListFromDynamics.OptionSet.Options.Any(x => x.Value == (int)reason && string.Equals(x.Label.UserLocalizedLabel.Label.Replace(" ", ""), reason.ToString(), StringComparison.OrdinalIgnoreCase)))
                {
                    return false;
                }
            }
            return true;
        }

    }
}
