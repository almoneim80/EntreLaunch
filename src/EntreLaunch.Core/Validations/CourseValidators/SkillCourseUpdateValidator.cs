using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Validations.CourseValidators
{
    public class SkillCourseUpdateValidator : AbstractValidator<SkillCourseUpdateDto>
    {
        public SkillCourseUpdateValidator(ILocalizationManager localizationManager)
        {
            RuleFor(x => x.Name)
                .MustNotBeDefault().When(x => !string.IsNullOrWhiteSpace(x.Name))
                .WithMessage(localizationManager.GetLocalizedString("NameCannotBeEmpty"));

            RuleFor(x => x.Description)
                .MustNotBeDefault().When(x => !string.IsNullOrWhiteSpace(x.Description))
                .WithMessage(localizationManager.GetLocalizedString("DescriptionCannotBeEmpty"));

            RuleFor(x => x.Logo)
                .MustBeValidImage().When(x => !string.IsNullOrWhiteSpace(x.Logo))
                .WithMessage(localizationManager.GetLocalizedString("LogoMustBeValidImage"));

            RuleFor(x => x.FieldId)
                .GreaterThan(0).When(x => x.FieldId.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("FieldIdGreaterThanZero"));

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).When(x => x.Price.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("PriceMustBeNonNegative"));

            RuleFor(x => x.Discount)
                .GreaterThanOrEqualTo(0).When(x => x.Discount.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("DiscountMustBeNonNegative"));

            RuleFor(x => x)
                .Must(dto => !(dto.Discount.HasValue && dto.Price.HasValue && dto.Discount > dto.Price))
                .WithMessage(localizationManager.GetLocalizedString("DiscountCannotExceedPrice"));

            When(x => x.CertificateExists == true && x.CertificateValidityInDays.HasValue, () =>
            {
                RuleFor(x => x.CertificateValidityInDays)
                    .GreaterThan(0)
                    .WithMessage(localizationManager.GetLocalizedString("CertificateValidityDaysMustBePositive"));
            });
        }
    }
}
