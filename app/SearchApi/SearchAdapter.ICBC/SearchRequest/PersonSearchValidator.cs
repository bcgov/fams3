using FluentValidation;
using SearchApi.Core.Contracts;

namespace SearchAdapter.ICBC.SearchRequest
{
    public class PersonSearchValidator : AbstractValidator<ExecuteSearch>
    {

        public PersonSearchValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
        }
        
    }
}