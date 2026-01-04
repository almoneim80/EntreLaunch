using EntreLaunch.DTOs.ConsultationDtos;

namespace EntreLaunch.Validations.ConsultationValidators
{
    public class TicketAttachmentCreateValidator : AbstractValidator<TicketAttachmentCreateDto>
    {
        public TicketAttachmentCreateValidator(ILocalizationManager localizationManager)
        {
            // Validate TicketId: must be greater than 0
            RuleFor(x => x.TicketId)
                .GreaterThan(0)
                .WithMessage(localizationManager.GetLocalizedString("TicketIdGreaterThanZero"));

            // Validate Url: must be a valid URL
            RuleFor(x => x.Url)
                .Must(url => Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
                .WithMessage(localizationManager.GetLocalizedString("InvalidUrl"));
        }
    }
}
