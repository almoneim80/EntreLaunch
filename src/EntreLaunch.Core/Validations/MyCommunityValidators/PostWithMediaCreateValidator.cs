using EntreLaunch.DTOs.MyCommunityDtos;

namespace EntreLaunch.Validations.MyCommunityValidators
{
    public class PostWithMediaCreateValidator : AbstractValidator<PostWithMediaCreateDto>
    {
        public PostWithMediaCreateValidator(ILocalizationManager localizationManager)
        {
            RuleFor(x => x.Text)
                .MustNotBeDefault()
                .WithMessage(localizationManager.GetLocalizedString("TextCannotBeEmpty"));

            RuleFor(x => x.Text)
                .MustHaveLengthInRange(1, 1000)
                .WithMessage(localizationManager.GetLocalizedString("TextLengthRange"));

            RuleFor(x => x.Media)
                .NotNull()
                .WithMessage(localizationManager.GetLocalizedString("MediaRequired"));

            RuleFor(x => x.Media)
                .Must(m => m.Any())
                .WithMessage(localizationManager.GetLocalizedString("AtLeastOneMediaRequired"));

            RuleForEach(x => x.Media)
                .SetValidator(new MediaCreateValidator(localizationManager));
        }
    }
}
