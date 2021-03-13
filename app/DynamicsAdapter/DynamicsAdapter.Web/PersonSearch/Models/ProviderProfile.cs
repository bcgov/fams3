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
            if (string.Equals(profile.Name, "WorkSafeBC", StringComparison.InvariantCultureIgnoreCase))
            {
                return InformationSourceType.WorkSafeBC.Value;
            }
            else if (string.Equals(profile.Name, "HCIM", StringComparison.InvariantCultureIgnoreCase))
            {
                return InformationSourceType.HCIM.Value;
            }
            else {
                return Enumeration.GetAll<InformationSourceType>().FirstOrDefault(m => m.Name.Equals(profile.Name, StringComparison.OrdinalIgnoreCase))?.Value;
            }
        }
       
    }
}
