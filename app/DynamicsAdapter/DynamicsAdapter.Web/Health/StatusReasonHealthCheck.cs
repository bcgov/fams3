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
        public StatusReasonService _statusReasonService;
        public StatusReasonHealthCheck(StatusReasonService statusReasonService)
        {
            _statusReasonService = statusReasonService;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {

            if (await CheckStatusReason())
            {
                return await Task.FromResult(
                    HealthCheckResult.Healthy("A healthy result."));
            }

            return await Task.FromResult(
                HealthCheckResult.Unhealthy("An unhealthy result."));
        }

        async Task<bool> CheckStatusReason()
        {
            var healthy = true; 
            var statusReasonServiceList = Enum.GetNames(typeof(SearchRequestStatusReason));
            var statusReasonListFromDynamics = await _statusReasonService.GetListAsync(new CancellationToken());
            var options = OptionsModified(statusReasonListFromDynamics);
            foreach (var i in statusReasonServiceList)
            {
                var reason = i.GetStatusReasonItem();

                if (!options.Contains(new Option()
                {
                    Value = (int) reason,
                    Label = new Label()
                    {
                        UserLocalizedLabel = new UserLocalizedLabel()
                        {
                            Label = reason.GetName()
                        }
                    }
                }))
                {
                    healthy = false;
                }
            }
            return await Task.FromResult(healthy);
        }

        private static List<Option> OptionsModified(StatusReason statusReasonListFromDynamics)
        {
            var options = new List<Option>();
            foreach (var option in statusReasonListFromDynamics.OptionSet.Options)
            {
                options.Add(new Option()
                {
                    Value = option.Value,
                    Label = new Label()
                    {
                        UserLocalizedLabel = new UserLocalizedLabel()
                        {
                            Label = option.Label.UserLocalizedLabel.Label.Replace(" ", "")
                        }
                    }
                });
            }

            return options;
        }
    }
}
