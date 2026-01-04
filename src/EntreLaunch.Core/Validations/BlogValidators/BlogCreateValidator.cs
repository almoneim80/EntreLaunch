using EntreLaunch.DTOs.BlogDtos;

namespace EntreLaunch.Validations.BlogValidators
{
    public class BlogCreateValidator : AbstractValidator<BlogCreateDto>
    {
        public BlogCreateValidator(ILocalizationManager localizationManager)
        {
            // Validate Title: must not be empty, must contain only letters and spaces, and must have a valid length
            RuleFor(x => x.Title)
                .MustNotBeDefault().When(x => !string.IsNullOrWhiteSpace(x.Title)).WithMessage(localizationManager.GetLocalizedString("TitleCannotBeEmpty"));

            RuleFor(x => x.Title)
            .MustHaveLengthInRange(3, 250).When(x => !string.IsNullOrWhiteSpace(x.Title)).WithMessage(localizationManager.GetLocalizedString("TitleLengthRange"));

            // Validate Details: must not be empty, must contain only letters and spaces, and must have a valid length
            RuleFor(x => x.Details)
                .MustNotBeDefault().When(x => !string.IsNullOrWhiteSpace(x.Details)).WithMessage(localizationManager.GetLocalizedString("DetailsCannotBeEmpty"));

            RuleFor(x => x.Details)
            .MustHaveLengthInRange(10, 2500).When(x => !string.IsNullOrWhiteSpace(x.Details)).WithMessage(localizationManager.GetLocalizedString("DetailsLengthRange"));

            // Validate Media: must be a valid URL or valid image/video file path
            RuleFor(x => x.Media)
                .MustNotBeDefault().When(x => !string.IsNullOrWhiteSpace(x.Media)).WithMessage(localizationManager.GetLocalizedString("MediaCannotBeEmpty"));

            RuleFor(x => x.Media)
                .MustBeValidImage().When(x => !string.IsNullOrWhiteSpace(x.Media)).WithMessage(localizationManager.GetLocalizedString("MediaInvalid"));
        }
    }
}
