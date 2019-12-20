namespace BcGov.Fams3.SearchApi.Contracts.PersonSearch
{

    public interface ValidationResult
    {
        string PropertyName { get; }
        string ErrorMessage { get; }
    }

}