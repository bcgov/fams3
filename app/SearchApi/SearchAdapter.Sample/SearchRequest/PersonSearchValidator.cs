using BcGov.Fams3.SearchApi.Contracts.Person;
using FluentValidation;

namespace SearchAdapter.Sample.SearchRequest
{
    /// <summary>
    /// Validates a person search
    /// </summary>
    public class PersonSearchValidator : AbstractValidator<Person>
    {
        public PersonSearchValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
        }
        
    }
}