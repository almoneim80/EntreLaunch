using EntreLaunch.DTOs.WheelDtos;

namespace EntreLaunch.Validations.WheelValidators
{
    public class WheelPlayerCreateValidator : AbstractValidator<WheelPlayerCreateDto>
    {
        public WheelPlayerCreateValidator(ILocalizationManager localizationManager)
        {
            // PlayerId: required
            RuleFor(x => x.PlayerId)
                .MustNotBeDefault()
                .WithMessage(localizationManager.GetLocalizedString("PlayerIdRequired"));

            // AwardId: must be greater than zero
            RuleFor(x => x.AwardId)
                .GreaterThan(0)
                .WithMessage(localizationManager.GetLocalizedString("AwardIdMustBePositive"));

            // PlayedAt: optional, if provided, must be in the past
            RuleFor(x => x.PlayedAt)
                .MustBeValidDate(mustBePast: true)
                .WithMessage(localizationManager.GetLocalizedString("PlayedAtMustBeInPast"));
        }
    }
}
