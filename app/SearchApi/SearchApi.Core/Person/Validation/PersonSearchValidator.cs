using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace SearchApi.Core.Person.Validation
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
