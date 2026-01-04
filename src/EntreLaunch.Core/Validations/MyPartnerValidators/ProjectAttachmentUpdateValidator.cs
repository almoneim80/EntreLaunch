using EntreLaunch.DTOs.MyPartnerDtos;

namespace EntreLaunch.Validations.MyPartnerValidators
{
    public class ProjectAttachmentUpdateValidator : AbstractValidator<ProjectAttachmentUpdateDto>
    {
        public ProjectAttachmentUpdateValidator(ILocalizationManager localizationManager)
        {
            // Validate ProjectId: if provided, must be greater than 0
            RuleFor(x => x.ProjectId)
                .GreaterThan(0).When(x => x.ProjectId.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("ProjectIdGreaterThanZero"));
        }
    }
}
