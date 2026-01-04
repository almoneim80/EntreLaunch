using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Validations.CourseValidators
{
    public class OnlineCourseUpdateValidator : AbstractValidator<OnlineCourseUpdateDto>
    {
        public OnlineCourseUpdateValidator(ILocalizationManager localizationManager)
        {
            RuleFor(x => x.Name)
                .MustNotBeDefault().When(x => !string.IsNullOrWhiteSpace(x.Name))
                .WithMessage(localizationManager.GetLocalizedString("NameCannotBeEmpty"));

            RuleFor(x => x.Description)
                .MustNotBeDefault().When(x => !string.IsNullOrWhiteSpace(x.Description))
                .WithMessage(localizationManager.GetLocalizedString("DescriptionCannotBeEmpty"));

            RuleFor(x => x.StudyWay)
                .MustNotBeDefault().When(x => !string.IsNullOrWhiteSpace(x.StudyWay))
                .WithMessage(localizationManager.GetLocalizedString("StudyWayCannotBeEmpty"));

            RuleFor(x => x.Status)
                .IsInEnum().When(x => x.Status.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("InvalidCourseStatus"));

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).When(x => x.Price.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("PriceMustBeNonNegative"));

            RuleFor(x => x.Discount)
                .GreaterThanOrEqualTo(0).When(x => x.Discount.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("DiscountMustBeNonNegative"));

            RuleFor(x => x)
                .Must(dto => !(dto.Discount.HasValue && dto.Price.HasValue && dto.Discount > dto.Price))
                .WithMessage(localizationManager.GetLocalizedString("DiscountCannotExceedPrice"));

            RuleFor(x => x.StartDate)
                .MustBeValidDate(mustBeFuture: true)
                .WithMessage(localizationManager.GetLocalizedString("StartDateMustBeFuture"));

            RuleFor(x => x.EndDate)
                .MustBeLaterThan(x => x.StartDate, x => x.StartDate.ToString("g") ?? "-")
                .When(x => x.EndDate > DateTimeOffset.MinValue && x.StartDate > DateTimeOffset.MinValue)
                .WithMessage(localizationManager.GetLocalizedString("EndDateMustBeAfterStartDate"));

            When(x => x.CertificateExists == true && x.CertificateValidityInDays.HasValue, () =>
            {
                RuleFor(x => x.CertificateValidityInDays)
                    .GreaterThan(0)
                    .WithMessage(localizationManager.GetLocalizedString("CertificateValidityDaysMustBePositive"));
            });
        }
    }
}
