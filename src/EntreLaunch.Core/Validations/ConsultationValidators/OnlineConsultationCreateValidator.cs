using EntreLaunch.DTOs.ConsultationDtos;

namespace EntreLaunch.Validations.ConsultationValidators
{
    public class OnlineConsultationCreateValidator : AbstractValidator<OnlineConsultationCreateDto>
    {
        public OnlineConsultationCreateValidator(ILocalizationManager localizationManager)
        {
            // CounselorId: must be greater than 0
            RuleFor(x => x.CounselorId)
                .GreaterThan(0).WithMessage(localizationManager.GetLocalizedString("CounselorIdGreaterThanZero"));

            // ConsultationTimeId: must be greater than 0
            RuleFor(x => x.ConsultationTimeId)
                .GreaterThan(0).WithMessage(localizationManager.GetLocalizedString("ConsultationTimeIdGreaterThanZero"));

            // Description: required, valid length
            RuleFor(x => x.Description)
                .MustNotBeDefault()
                .WithMessage(localizationManager.GetLocalizedString("DescriptionCannotBeEmpty"));

            RuleFor(x => x.Description)
                .MustHaveLengthInRange(10, 500)
                .WithMessage(localizationManager.GetLocalizedString("DescriptionLengthRange"));
        }
    }
}
