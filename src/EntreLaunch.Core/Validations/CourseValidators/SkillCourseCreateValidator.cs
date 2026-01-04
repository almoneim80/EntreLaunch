using EntreLaunch.DTOs.TrainingDtos;
using EntreLaunch.Validations.LessonValidators;

namespace EntreLaunch.Validations.CourseValidators
{
    public class SkillCourseCreateValidator : AbstractValidator<SkillCourseCreateDto>
    {
        public SkillCourseCreateValidator(ILocalizationManager localizationManager)
        {
            RuleFor(x => x.Name)
                .MustNotBeDefault()
                .WithMessage(localizationManager.GetLocalizedString("NameCannotBeEmpty"));

            RuleFor(x => x.Name)
                .MustHaveLengthInRange(3, 500)
                .WithMessage(localizationManager.GetLocalizedString("NameLengthRange"));

            RuleFor(x => x.Description)
                .MustNotBeDefault()
                .WithMessage(localizationManager.GetLocalizedString("DescriptionCannotBeEmpty"));

            RuleFor(x => x.Description)
                .MustHaveLengthInRange(10, 2500)
                .WithMessage(localizationManager.GetLocalizedString("DescriptionLengthRange"));

            RuleFor(x => x.FieldId)
                .GreaterThan(0)
                .WithMessage(localizationManager.GetLocalizedString("FieldIdGreaterThanZero"));

            RuleFor(x => x.Logo)
                .MustNotBeDefault()
                .WithMessage(localizationManager.GetLocalizedString("LogoCannotBeEmpty"));

            RuleFor(x => x.Logo)
                .MustBeValidImage()
                .WithMessage(localizationManager.GetLocalizedString("LogoMustBeValidImage"));

            When(x => x.CertificateExists, () =>
            {
                RuleFor(x => x.CertificateValidityInDays)
                    .GreaterThan(0)
                    .WithMessage(localizationManager.GetLocalizedString("CertificateValidityDaysMustBePositive"));
            });

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0)
                .WithMessage(localizationManager.GetLocalizedString("PriceMustBeNonNegative"));

            RuleFor(x => x.Discount)
                .GreaterThanOrEqualTo(0)
                .WithMessage(localizationManager.GetLocalizedString("DiscountMustBeNonNegative"));

            RuleFor(x => x.Discount)
                .LessThanOrEqualTo(x => x.Price)
                .WithMessage(localizationManager.GetLocalizedString("DiscountCannotExceedPrice"));

            RuleFor(x => x.Lessons)
                .NotNull()
                .WithMessage(localizationManager.GetLocalizedString("LessonsCannotBeNull"));

            RuleFor(x => x.Lessons)
                .Must(x => x.Count > 0)
                .WithMessage(localizationManager.GetLocalizedString("LessonsMustContainAtLeastOneItem"))
                .ForEach(x => x.SetValidator(new LessonsCreateValidator(localizationManager)));
        }
    }
}
