using System.ComponentModel.DataAnnotations;
using SearchApi.Core.Adapters.Contracts;

namespace SearchApi.Core.Adapters.Configuration
{
    public class ProviderProfileOptions : ProviderProfile
    {
        [Required]
        public string Name { get; set; }
    }
}