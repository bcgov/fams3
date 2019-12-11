using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using DynamicsAdapter.Web.SearchRequest;
using Fams3Adapter.Dynamics.Types;
using Fams3Adapter.Dynamics.OptionSets;
using Fams3Adapter.Dynamics.OptionSets.Models;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace DynamicsAdapter.Web.Health
{
    public class StatusReasonHealthCheck : IHealthCheck
    {
        private readonly IOptionSetService _optionSetService;
        private readonly ILogger<StatusReasonHealthCheck> _logger;

        public StatusReasonHealthCheck(IOptionSetService optionSetService, ILogger<StatusReasonHealthCheck> logger)
        {
            _optionSetService = optionSetService;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {

            if (!await CheckStatusReason(cancellationToken))
            {
                return HealthCheckResult.Unhealthy("Different Status Reason Exists in dynamics");
            }

            if (!await CheckOptionSet(cancellationToken, TypeService.TypeList))
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

        private IEnumerable<Enumeration> GetListOfOptions (string entityName)
        {
            IEnumerable<Enumeration> optionsList;
            switch (entityName)
            {
                case "ssg_canadianprovincecodesimpletype":
                    optionsList= Enumeration.GetAll<CanadianProvinceType>();
                    break;
                case "ssg_identificationtypes":
                    optionsList = Enumeration.GetAll<IdentificationType>();
                    break;
                case "ssg_informationsourcecodes":
                    optionsList = Enumeration.GetAll<InformationSourceType>();
                    break;
                case "ssg_addresscategorycodes":
                    optionsList = Enumeration.GetAll<LocationType>();
                    break;
                case "ssg_telephonenumbercategorycodes":
                    optionsList = Enumeration.GetAll<TelephoneNumberType>();
                    break;
                default:
                    optionsList= Enumeration.GetAll<IdentificationType>();
                    break;
            }
            return optionsList;
        }

        private async Task<bool> CheckOptionSet(CancellationToken cancellationToken, List<string> optionTypes)
        {
            _logger.LogInformation("OptionsSet Match Started!");
            foreach (var optionType in optionTypes)
            {
                _logger.LogDebug(
                      $"Atttempting to retrieve options set list from dyanmics for {optionType}");

                var types = await _optionSetService.GetAllOptions(optionType, cancellationToken);
                _logger.LogInformation(
                     $"Retrieved options set list from dyanmics for {optionType}. {types.Count()} records returned.");
                foreach (var entityType in GetListOfOptions(optionType))
                {
               
                    if (!types.Any(x => x.Value == entityType.Value && string.Equals(x.Name, entityType.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        _logger.LogError(
                      $"Matching failed for {optionType}, {entityType.Name} not matched!");
                        return false;
                    }
                }
            }
            return true;

        }

    }
}
