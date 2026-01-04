using EntreLaunch.DTOs.ConsultationDtos;

namespace EntreLaunch.Validations.ConsultationValidators
{
    public class TicketMessageUpdateValidator : AbstractValidator<TicketMessageUpdateDto>
    {
        public TicketMessageUpdateValidator(ILocalizationManager localizationManager)
        {
            // Validate Content: must not be empty if provided
            RuleFor(x => x.Content)
                .MustNotBeDefault().When(x => !string.IsNullOrWhiteSpace(x.Content))
                .WithMessage(localizationManager.GetLocalizedString("ContentCannotBeEmpty"));
        }
    }
}
