using BcGov.Fams3.SearchApi.Contracts.SearchRequest;
using FluentValidation;
using System.Data;
using System.Linq;

namespace SearchRequest.Adaptor.Notifier.Models.Validation
{
    public class NotificationValidator : AbstractValidator<Notification>
    {
        public NotificationValidator()
        {
            RuleFor(x => x.ActivityDate).NotNull().NotEmpty();
            RuleFor(x => x.AgencyFileId).NotNull().NotEmpty();
            RuleFor(x => x.Agency).NotNull().NotEmpty();
            RuleFor(x => x.FileId).NotNull().NotEmpty();
            RuleFor(x => x.Acvitity).NotNull().NotEmpty().IsEnumName(typeof(NotificationType)).WithMessage("Please pass valid activity");

        }
    }


}
