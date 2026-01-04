using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Validations.TrainingPathvalidators
{
    public class TrainingPathCreateValidator : AbstractValidator<TrainingPathCreateDto>
    {
        public TrainingPathCreateValidator(ILocalizationManager localization)
        {
            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Name)
                .Length(3, 250)
                .WithMessage(localization.GetLocalizedString("TrainingPathNameRequired"));

            RuleFor(x => x.Description)
                .NotEmpty();

            RuleFor(x => x.Description)
                .Length(10, 2000)
                .WithMessage(localization.GetLocalizedString("TrainingPathDescriptionRequired"));

            RuleFor(x => x.Price)
                .GreaterThan(0).When(x => !x.IsFree)
                .WithMessage(localization.GetLocalizedString("TrainingPathPriceMustBeGreaterThanZero"));

            RuleFor(x => x.CertificateValidityInDays)
                .InclusiveBetween(1, 365)
                .When(x => x.CertificateExists)
                .WithMessage(localization.GetLocalizedString("TrainingPathCertificateValidityRequired"));

            RuleFor(x => x.MaxEnrollment)
                .GreaterThan(0).When(x => x.MaxEnrollment > 0)
                .WithMessage(localization.GetLocalizedString("TrainingPathMaxEnrollmentMustBePositive"));

            RuleFor(x => x)
                .Must(x => x.IsFree || (x.Price.HasValue && x.Price > 0))
                .WithMessage(localization.GetLocalizedString("TrainingPathFreeInconsistentWithPrice"));
        }
    }
}
