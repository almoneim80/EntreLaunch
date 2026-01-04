using EntreLaunch.DTOs.WheelDtos;
namespace EntreLaunch.Validations.WheelValidators
{
    public class WheelAwardCreateValidator : AbstractValidator<WheelAwardCreateDto>
    {
        public WheelAwardCreateValidator(ILocalizationManager localizationManager)
        {
            // Name: optional but if provided must be valid
            RuleFor(x => x.Name)
                .MustNotBeDefault().When(x => !string.IsNullOrWhiteSpace(x.Name))
                .WithMessage(localizationManager.GetLocalizedString("AwardNameRequired"));

            RuleFor(x => x.Name)
                .MustHaveLengthInRange(2, 500).When(x => !string.IsNullOrWhiteSpace(x.Name))
                .WithMessage(localizationManager.GetLocalizedString("AwardNameLengthRange"));

            // Description: optional but if provided must be valid
            RuleFor(x => x.Description)
                .MustNotBeDefault().When(x => !string.IsNullOrWhiteSpace(x.Description))
                .WithMessage(localizationManager.GetLocalizedString("AwardDescriptionRequired"));

            RuleFor(x => x.Description)
                .MustHaveLengthInRange(2, 500).When(x => !string.IsNullOrWhiteSpace(x.Description))
                .WithMessage(localizationManager.GetLocalizedString("AwardDescriptionLengthRange"));

            // Probability: optional but must be between 0 and 1
            RuleFor(x => x.Probability)
                .InclusiveBetween(0, 1).When(x => x.Probability.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("ProbabilityRange"));

            // Type: enum validation
            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage(localizationManager.GetLocalizedString("InvalidAwardType"));
        }
    }
}
