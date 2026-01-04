using EntreLaunch.DTOs.ConsultationDtos;

namespace EntreLaunch.Validations.ConsultationValidators
{
    public class TicketMessageCreateValidator : AbstractValidator<TicketMessageCreateDto>
    {
        public TicketMessageCreateValidator(ILocalizationManager localizationManager)
        {
            // Validate TicketId: must be greater than 0
            RuleFor(x => x.TicketId)
                .GreaterThan(0)
                .WithMessage(localizationManager.GetLocalizedString("TicketIdGreaterThanZero"));

            // Validate Content: must not be empty
            RuleFor(x => x.Content)
                .MustNotBeDefault();

            RuleFor(x => x.Content)
               .MustHaveLengthInRange(2, 500).WithMessage(localizationManager.GetLocalizedString("ContentLengthRange"));
        }
    }
}
