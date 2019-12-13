using System.ComponentModel.DataAnnotations;
using BcGov.Fams3.SearchApi.Core.Adapters.Contracts;

namespace BcGov.Fams3.SearchApi.Core.Adapters.Configuration
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