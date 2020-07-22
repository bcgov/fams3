using BcGov.Fams3.SearchApi.Contracts.PersonSearch;

namespace SearchRequestAdaptor.Publisher.Models
{
    public class ValidationResultData : ValidationResult
    {
        public string PropertyName { get; set; }

        public string ErrorMessage { get; set; }
    }
}
