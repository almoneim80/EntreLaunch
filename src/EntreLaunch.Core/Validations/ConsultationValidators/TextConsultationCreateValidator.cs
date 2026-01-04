using EntreLaunch.DTOs.ConsultationDtos;

namespace EntreLaunch.Validations.ConsultationValidators
{
    public class TextConsultationCreateValidator : AbstractValidator<TextConsultationCreateDto>
    {
        public TextConsultationCreateValidator(ILocalizationManager localizationManager)
        {
            // CounselorId: must be greater than 0
            RuleFor(x => x.CounselorId)
                .GreaterThan(0).WithMessage(localizationManager.GetLocalizedString("CounselorIdGreaterThanZero"));

            // Description: required, valid length
            RuleFor(x => x.Description)
                .MustNotBeDefault().WithMessage(localizationManager.GetLocalizedString("DescriptionCannotBeEmpty"));

            RuleFor(x => x.Description)
                .MustHaveLengthInRange(10, 500).WithMessage(localizationManager.GetLocalizedString("DescriptionLengthRange"));
        }
    }
}
