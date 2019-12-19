using Fams3Adapter.Dynamics.OptionSets.Models;
using Fams3Adapter.Dynamics.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            return Enumeration.GetAll<InformationSourceType>().FirstOrDefault(m => m.Name.Equals(profile.Name, StringComparison.OrdinalIgnoreCase))?.Value;
        }
       
    }
}
