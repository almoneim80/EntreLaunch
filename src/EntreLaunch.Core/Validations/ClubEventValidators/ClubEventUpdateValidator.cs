using EntreLaunch.DTOs.ClubDtos;

namespace EntreLaunch.Validations.ClubEventValidators
{
    internal class ClubEventUpdateValidator : AbstractValidator<ClubEventUpdateDto>
    {
        public ClubEventUpdateValidator(ILocalizationManager localizationManager)
        {
            // Validate Name: and must have a valid length
            RuleFor(x => x.Name)
                .MustHaveLengthInRange(3, 250).When(x => !string.IsNullOrWhiteSpace(x.Name))
                .WithMessage(localizationManager.GetLocalizedString("NameLengthRange"));

            // Validate City: must contain only letters and spaces, and must have a valid length
            RuleFor(x => x.City)
                .MustContainOnlyLettersAndSpaces().When(x => !string.IsNullOrWhiteSpace(x.City))
                .WithMessage(localizationManager.GetLocalizedString("CityOnlyLettersAndSpaces"));

            RuleFor(x => x.City)
                .MustHaveLengthInRange(3, 250).When(x => !string.IsNullOrWhiteSpace(x.City))
                .WithMessage(localizationManager.GetLocalizedString("CityLengthRange"));

            // Validate Description: must not be empty, must contain only letters and spaces, and must have a valid length
            RuleFor(x => x.Description)
                .MustHaveLengthInRange(10, 500).When(x => !string.IsNullOrWhiteSpace(x.Description))
                .WithMessage(localizationManager.GetLocalizedString("DescriptionLengthRange"));

            // Validate StartDate: must be a valid date in the future
            RuleFor(x => x.StartDate)
                .Must(x => x > DateHelper.UtcNow)
                .WithMessage(localizationManager.GetLocalizedString("StartDateInFuture"));

            // Validate EndDate: must be a valid date
            RuleFor(x => x.EndDate)
                .Must(x => x > DateTimeOffset.MinValue)
                .WithMessage(localizationManager.GetLocalizedString("EndDateInvalid"));
        }
    }
}
