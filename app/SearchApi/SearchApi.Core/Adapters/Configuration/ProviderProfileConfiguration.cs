using System.ComponentModel.DataAnnotations;
using SearchApi.Core.Adapters.Contracts;

namespace SearchApi.Core.Adapters.Configuration
{
    public class ProviderProfileConfiguration : ProviderProfile
    {
        [Required]
        public string Name { get; set; }
    }
}