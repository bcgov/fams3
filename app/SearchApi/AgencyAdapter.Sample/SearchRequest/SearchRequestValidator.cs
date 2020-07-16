
using FluentValidation;

namespace AgencyAdapter.Sample.SearchRequest
{
    /// <summary>
    /// Validates a person search
    /// </summary>
    public class SearchRequestValidator : AbstractValidator<Models.SearchRequest>
    {
        public SearchRequestValidator()
        {
            RuleFor(x => x.RequestorAgencyCode).NotEmpty().WithMessage("Agency code is required");
            RuleFor(x => x.RequestAction).NotEmpty().WithMessage("Action to perform is required");
            RuleFor(x => x.AgentEmail).NotEmpty().EmailAddress().WithMessage("Valid agent email is required");

            RuleFor(x => x.InformationRequestedList).NotNull();
            RuleFor(x => x.InformationRequestedList).Must(item => item.Count > 0).When(x => x.InformationRequestedList != null);
        }
        
    }

  


}