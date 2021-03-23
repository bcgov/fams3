using Fams3Adapter.Dynamics.OptionSets.Models;
using Fams3Adapter.Dynamics.Types;
using System;
using System.Linq;

namespace DynamicsAdapter.Web.PersonSearch.Models
{
    public class ProviderProfile
    {
        public string Name { get; set; }
    }

    public static class ProviderProfileExtension
    {
        public static int? DynamicsID(this ProviderProfile profile)
        {
            return profile.Name.ToUpper() switch
            {
                "WORKSAFEBC" => InformationSourceType.WorkSafeBC.Value,
                "MH_HCIM" => InformationSourceType.HCIM.Value,
                "MH_RAPIDE" => InformationSourceType.RAPIDE.Value,
                "MH_RAPIDR" => InformationSourceType.RAPIDR.Value,
                _ => Enumeration.GetAll<InformationSourceType>().FirstOrDefault(m => m.Name.Equals(profile.Name, StringComparison.OrdinalIgnoreCase))?.Value
            };
            
        }
       
    }
}
