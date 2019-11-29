using FluentValidation;
using SearchApi.Core.Person.Contracts;

namespace SearchAdapter.ICBC.SearchRequest
{
    /// <summary>
    /// Validates a person search
    /// </summary>
    public class PersonSearchValidator : AbstractValidator<ExecuteSearch>
    {
        public PersonSearchValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
        }
        
    }
}