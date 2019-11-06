using System.ComponentModel.DataAnnotations;
using SearchApi.Core.Adapters.Contracts;

namespace SearchApi.Core.Adapters.Configuration
{
    /// <summary>
    /// Represents a configurable ProviderProfile
    /// </summary>
    public class ProviderProfileOptions : ProviderProfile
    {
        [Required]
        public string Name { get; set; }
    }
}