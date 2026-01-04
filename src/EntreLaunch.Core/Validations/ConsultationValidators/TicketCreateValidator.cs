using EntreLaunch.DTOs.ConsultationDtos;

namespace EntreLaunch.Validations.ConsultationValidators
{
    public class TicketCreateValidator : AbstractValidator<TicketCreateDto>
    {
        public TicketCreateValidator(ILocalizationManager localizationManager)
        {
            // Validate CreatorId: must be greater than 0
            RuleFor(x => x.CreatorId)
                .GreaterThan(0)
                .WithMessage(localizationManager.GetLocalizedString("CreatorIdGreaterThanZero"));

            // Validate ConsultationId: must be greater than 0
            RuleFor(x => x.ConsultationId)
                .GreaterThan(0)
                .WithMessage(localizationManager.GetLocalizedString("ConsultationIdGreaterThanZero"));
        }
    }
}
