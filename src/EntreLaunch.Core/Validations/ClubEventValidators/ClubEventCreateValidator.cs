using EntreLaunch.DTOs.ClubDtos;

namespace EntreLaunch.Validations.ClubEventValidators
{
    public class ClubEventCreateValidator : AbstractValidator<ClubEventCreateDto>
    {
        public ClubEventCreateValidator(ILocalizationManager localizationManager)
        {
            // Validate Name: must not be empty, must contain only letters and spaces, and must have a valid length
            RuleFor(x => x.Name)
                .NotNull()
                .WithMessage(localizationManager.GetLocalizedString("NameCannotBeEmpty"));

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(localizationManager.GetLocalizedString("NameLengthRange"));

            RuleFor(x => x.Name)
                .Must(name => !string.IsNullOrWhiteSpace(name))
                .WithMessage(localizationManager.GetLocalizedString("NameCannotBeWhitespace"));

            RuleFor(x => x.Name)
                .MustHaveLengthInRange(3, 250)
                .When(x => !string.IsNullOrWhiteSpace(x.Name))
                .WithMessage(localizationManager.GetLocalizedString("NameLengthRange"));

            // Validate City: must not be empty, must contain only letters and spaces, and must have a valid length
            RuleFor(x => x.City)
                .MustNotBeDefault().When(x => !string.IsNullOrWhiteSpace(x.City))
                .WithMessage(localizationManager.GetLocalizedString("CityCannotBeEmpty"));

            RuleFor(x => x.City)
                .MustContainOnlyLettersAndSpaces().When(x => !string.IsNullOrWhiteSpace(x.City))
                .WithMessage(localizationManager.GetLocalizedString("CityOnlyLettersAndSpaces"));

            RuleFor(x => x.City)
                .MustHaveLengthInRange(3, 250).When(x => !string.IsNullOrWhiteSpace(x.City))
                .WithMessage(localizationManager.GetLocalizedString("CityLengthRange"));

            // Validate Description: must not be empty, must contain only letters and spaces, and must have a valid length
            RuleFor(x => x.Description)
                .MustNotBeDefault().When(x => !string.IsNullOrWhiteSpace(x.Description))
                .WithMessage(localizationManager.GetLocalizedString("DescriptionCannotBeEmpty"));

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

            // Validate StartDate must be earlier than EndDate
            RuleFor(x => x.StartDate)
                .MustBeEarlierThan(x => x.EndDate, x => x.EndDate.ToString("d"))
                .WithMessage(localizationManager.GetLocalizedString("StartDateMustBeEarlierThanEndDate"));
        }
    }
}
