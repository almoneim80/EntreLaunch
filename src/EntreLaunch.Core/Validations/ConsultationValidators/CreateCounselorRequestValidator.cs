using EntreLaunch.DTOs.ConsultationDtos;

namespace EntreLaunch.Validations.ConsultationValidators
{
    public class CreateCounselorRequestValidator : AbstractValidator<CreateCounselorRequestDto>
    {
        public CreateCounselorRequestValidator(ILocalizationManager localizationManager)
        {
            // Validate Qualification: must not be empty
            RuleFor(x => x.Qualification)
                .NotEmpty()
                .WithMessage(localizationManager.GetLocalizedString("QualificationCannotBeEmpty"));

            // Validate City: must only contain letters and spaces
            RuleFor(x => x.City)
                .MustNotBeDefault().When(x => !string.IsNullOrWhiteSpace(x.City))
                .WithMessage(localizationManager.GetLocalizedString("CityCannotBeEmpty"));

            RuleFor(x => x.City)
                .MustContainOnlyLettersAndSpaces()
                .WithMessage(localizationManager.GetLocalizedString("CityMustContainOnlyLettersAndSpaces"));

            RuleFor(x => x.City)
                .MustHaveLengthInRange(3, 250).When(x => !string.IsNullOrWhiteSpace(x.City))
                .WithMessage(localizationManager.GetLocalizedString("CityLengthRange"));

            // Validate SpecializationExperience: must be >= 0
            RuleFor(x => x.SpecializationExperience)
                .GreaterThanOrEqualTo(0)
                .WithMessage(localizationManager.GetLocalizedString("SpecializationExperienceGreaterThanZero"));

            // Validate ConsultingExperience: must be >= 0
            RuleFor(x => x.ConsultingExperience)
                .GreaterThanOrEqualTo(0)
                .WithMessage(localizationManager.GetLocalizedString("ConsultingExperienceGreaterThanZero"));

            // Validate DailyHours: must be between 1 and 24
            RuleFor(x => x.DailyHours)
                .InclusiveBetween(1, 24)
                .WithMessage(localizationManager.GetLocalizedString("DailyHoursBetweenOneAndTwentyFour"));

            // Validate SocialMediaAccounts: must contain at least one entry with valid key and value
            RuleFor(x => x.SocialMediaAccounts)
                .NotEmpty()
                .WithMessage(localizationManager.GetLocalizedString("SocialMediaAccountsCannotBeEmpty"));

            RuleFor(x => x.SocialMediaAccounts)
                .Must(accounts => accounts.All(a => !string.IsNullOrWhiteSpace(a.Key) && !string.IsNullOrWhiteSpace(a.Value)))
                .WithMessage(localizationManager.GetLocalizedString("SocialMediaAccountKeysAndValuesCannotBeEmpty"));
        }
    }
}
