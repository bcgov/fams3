using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fams3Adapter.Dynamics.Types;
using Fams3Adapter.Dynamics.OptionSets;
using Fams3Adapter.Dynamics.OptionSets.Models;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace DynamicsAdapter.Web.Health
{
    public class DynamicsHealthCheck : IHealthCheck
    {
        private readonly IOptionSetService _optionSetService;
        private readonly ILogger<DynamicsHealthCheck> _logger;

        public DynamicsHealthCheck(IOptionSetService optionSetService, ILogger<DynamicsHealthCheck> logger)
        {
            _optionSetService = optionSetService;
            _logger = logger;
        }

        /// <summary>
        /// Check if all defined Enumerations can be mapped to Dynamics Option/Entiy correctly.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

        private IEnumerable<Enumeration> GetListOfOptions(string entityName)
        {
            return entityName.ToLower() switch
            {
                "ssg_canadianprovincecodesimpletype" => Enumeration.GetAll<CanadianProvinceType>(),
                "ssg_identificationtypes" => Enumeration.GetAll<IdentificationType>(),
                "ssg_informationsourcecodes" => Enumeration.GetAll<InformationSourceType>(),
                "ssg_addresscategorycodes" => Enumeration.GetAll<LocationType>(),
                "ssg_telephonenumbercategorycodes" => Enumeration.GetAll<TelephoneNumberType>(),
                "ssg_personnamecategorycodes" => Enumeration.GetAll<PersonNameCategory>(),
                "ssg_nullableboolean" => Enumeration.GetAll<NullableBooleanType>(),
                "ssg_relationshipcategorycodes" => Enumeration.GetAll<PersonRelationType>(),
                "ssg_gendertypes" => Enumeration.GetAll<GenderType>(),
                "ssg_employmentrecordtypes" => Enumeration.GetAll<EmploymentRecordType>(),
                "ssg_incomeassistancestatus" => Enumeration.GetAll<IncomeAssistanceStatusType>(),
                "ssg_payororreceiveroptions" => Enumeration.GetAll<PersonSoughtType>(),
                "ssg_requestpriorities" => Enumeration.GetAll<RequestPriorityType>(),
                "ssg_personcategorycodes" => Enumeration.GetAll<RelatedPersonPersonType>(),
                "ssg_employmentstatus" => Enumeration.GetAll<EmploymentStatusType>(),
                "ssg_selfemploymentcompanytype" => Enumeration.GetAll<SelfEmploymentCompanyType>(),
                "ssg_selfemploymentcompanyrole" => Enumeration.GetAll<SelfEmploymentCompanyRoleType>(),
                "ssg_famsincomeassistanceclasses" => Enumeration.GetAll<IncomeAssistanceClassType>(),
                "ssg_bankaccounttype" => Enumeration.GetAll<BankAccountType>(),
                "ssg_datapartnerspeedtypes" => Enumeration.GetAll<AutoSearchSpeedType>(),
                "ssg_safetyconcerntypes" => Enumeration.GetAll<SafetyConcernType>(),
                _ => Enumeration.GetAll<TelephoneNumberType>()

            };
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
