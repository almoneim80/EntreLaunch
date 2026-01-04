using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Validations.CourseValidators
{
    public class OnlineCourseCreateValidator : AbstractValidator<OnlineCourseCreateDto>
    {
        public OnlineCourseCreateValidator(ILocalizationManager localizationManager)
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

            RuleFor(x => x.StudyWay)
                .MustNotBeDefault()
                .WithMessage(localizationManager.GetLocalizedString("StudyWayCannotBeEmpty"));

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage(localizationManager.GetLocalizedString("InvalidCourseStatus"));

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0)
                .WithMessage(localizationManager.GetLocalizedString("PriceMustBeNonNegative"));

            RuleFor(x => x.Discount)
                .GreaterThanOrEqualTo(0)
                .WithMessage(localizationManager.GetLocalizedString("DiscountMustBeNonNegative"));

            RuleFor(x => x.Discount)
                .LessThanOrEqualTo(x => x.Price)
                .WithMessage(localizationManager.GetLocalizedString("DiscountCannotExceedPrice"));

            RuleFor(x => x.StartDate)
                .MustBeValidDate(mustBeFuture: true)
                .WithMessage(localizationManager.GetLocalizedString("StartDateMustBeFuture"));

            RuleFor(x => x.EndDate)
                .MustBeLaterThan(x => x.StartDate, x => x.StartDate.ToString("g"))
                .WithMessage(localizationManager.GetLocalizedString("EndDateMustBeAfterStartDate"));

            When(x => x.CertificateExists, () =>
            {
                RuleFor(x => x.CertificateValidityInDays)
                    .GreaterThan(0)
                    .WithMessage(localizationManager.GetLocalizedString("CertificateValidityDaysMustBePositive"));
            });
        }
    }
}
