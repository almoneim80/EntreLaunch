using EntreLaunch.DTOs.ClubDtos;

namespace EntreLaunch.Validations.ClubEventValidators
{
    public class ClubEventRegistrationCreateValidator : AbstractValidator<ClubEventRegistrationCreateDto>
    {
        public ClubEventRegistrationCreateValidator(ILocalizationManager localizationManager)
        {
            // EventId must be a valid ID (greater than 0)
            RuleFor(x => x.EventId)
                .GreaterThan(0).WithMessage(localizationManager.GetLocalizedString("EventIdGreaterThanZero"));

            // Notes: optional, but if provided must not exceed max length
            RuleFor(x => x.Notes)
                .MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Notes))
                .WithMessage(localizationManager.GetLocalizedString("NotesMaxLength"));
        }
    }
}
