using EntreLaunch.DTOs.MyCommunityDtos;

namespace EntreLaunch.Validations.MyCommunityValidators
{
    public class MediaCreateValidator : AbstractValidator<MediaCreateDto>
    {
        public MediaCreateValidator(ILocalizationManager localizationManager)
        {
            RuleFor(x => x.MediaType)
                .MustNotBeDefault()
                .WithMessage(localizationManager.GetLocalizedString("MediaTypeRequired"));

            RuleFor(x => x.Url)
                .MustNotBeDefault()
                .WithMessage(localizationManager.GetLocalizedString("MediaUrlRequired"));

            RuleFor(x => x.Url)
                .MustBeValidAttachment()
                .WithMessage(localizationManager.GetLocalizedString("InvalidMediaUrl"));
        }
    }
}
