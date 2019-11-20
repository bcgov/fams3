using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using DynamicsAdapter.Web.SearchRequest;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.OptionSets;
using Fams3Adapter.Dynamics.OptionSets.Models;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DynamicsAdapter.Web.Health
{
    public class StatusReasonHealthCheck : IHealthCheck
    {
        private readonly IOptionSetService _optionSetService;

        public StatusReasonHealthCheck(IOptionSetService optionSetService)
        {
            _optionSetService = optionSetService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {

            if (!await CheckStatusReason(cancellationToken))
            {
                return HealthCheckResult.Unhealthy("Different Status Reason Exists in dynamics");
            }

            if (!await CheckOptionSet(cancellationToken))
            {
                return HealthCheckResult.Unhealthy("Different Option Sets Exists in dynamics");
            }


            return HealthCheckResult.Healthy("All Status Reason Exists in dynamics.");
        }

        private async Task<bool> CheckStatusReason(CancellationToken cancellationToken)
        {

            var statusReasonListFromDynamics = await _optionSetService.GetAllStatusCode(nameof(SSG_SearchApiRequest).ToLower(), cancellationToken);

            foreach (SearchApiRequestStatusReason reason in Enumeration.GetAll<SearchApiRequestStatusReason>())
            {
                if (!statusReasonListFromDynamics.Any(x => x.Value == reason.Value && string.Equals(x.Name, reason.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    return false;
                }
            }
            return true;
        }


        private async Task<bool> CheckOptionSet(CancellationToken cancellationToken)
        {

            var idTypes = await _optionSetService.GetAllOptions("ssg_identificationtypes", cancellationToken);

            foreach (var identificationType in Enumeration.GetAll<IdentificationType>())
            {
                if (!idTypes.Any(x => x.Value == identificationType.Value && string.Equals(x.Name, identificationType.Name, StringComparison.OrdinalIgnoreCase)))
                    return false;
            }

            return true;
        }

    }
}
