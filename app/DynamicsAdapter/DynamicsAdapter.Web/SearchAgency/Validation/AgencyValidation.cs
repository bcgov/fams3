using DynamicsAdapter.Web.SearchAgency.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.SearchAgency.Validation
{
    public class SearchResponseReadyValidator : AbstractValidator<SearchResponseReady>
    {
        public SearchResponseReadyValidator()
        {
            RuleFor(x => x.FileId).NotNull().NotEmpty().WithMessage("File Id is required");
            RuleFor(x => x.Agency).NotNull().NotEmpty().WithMessage("Agency Name is required.");
            RuleFor(x => x.AgencyFileId).NotNull().NotEmpty().WithMessage("Agency File ID is required.");
            RuleFor(x => x.Activity).Must(m => m.Equals("RequestClosed", StringComparison.InvariantCultureIgnoreCase)).WithMessage("Activity must be RequestClosed.");
            RuleFor(x => x.ResponseGuid).NotNull().NotEmpty().Must(m => Guid.Parse(m) != Guid.Empty)
                .WithMessage("ResponseGuid is required.");


        }
    }

}
