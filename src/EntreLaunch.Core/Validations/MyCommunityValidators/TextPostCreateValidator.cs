using EntreLaunch.DTOs.MyCommunityDtos;

namespace EntreLaunch.Validations.MyCommunityValidators
{
    public class TextPostCreateValidator : AbstractValidator<TextPostCreateDto>
    {
        public TextPostCreateValidator(ILocalizationManager localizationManager)
        {
            RuleFor(x => x.Text)
                .MustNotBeDefault()
                .WithMessage(localizationManager.GetLocalizedString("TextCannotBeEmpty"));

            RuleFor(x => x.Text)
                .MustHaveLengthInRange(1, 1000)
                .WithMessage(localizationManager.GetLocalizedString("TextLengthRange"));
        }
    }
}
