using BcGov.Fams3.SearchApi.Contracts.PersonSearch;

namespace BcGov.Fams3.SearchApi.Core.Adapters.Models
{
    public class DefaultValidationResult : ValidationResult
    {
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }
    }
}