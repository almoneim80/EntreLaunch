using EntreLaunch.DTOs.TrainingDtos;

namespace EntreLaunch.Validations.TrainingPathvalidators
{
    public class TrainingPathUpdateValidator : AbstractValidator<TrainingPathUpdateDto>
    {
        public TrainingPathUpdateValidator(ILocalizationManager localization)
        {
            RuleFor(x => x.Name)
                .Length(3, 250)
                .When(x => !string.IsNullOrWhiteSpace(x.Name))
                .WithMessage(localization.GetLocalizedString("TrainingPathNameLengthRange"));

            RuleFor(x => x.Description)
                .Length(10, 1000)
                .When(x => !string.IsNullOrWhiteSpace(x.Description))
                .WithMessage(localization.GetLocalizedString("TrainingPathDescriptionLengthRange"));

            RuleFor(x => x.Price)
                .GreaterThan(0)
                .When(x => x.Price.HasValue && (!x.IsFree.HasValue || x.IsFree == false))
                .WithMessage(localization.GetLocalizedString("TrainingPathPriceMustBePositive"));

            RuleFor(x => x)
                .Must(x => !x.IsFree.HasValue || x.IsFree == false || (x.Price.HasValue && x.Price.Value == 0))
                .WithMessage(localization.GetLocalizedString("TrainingPathIsFreePriceMismatch"));

            RuleFor(x => x.CertificateValidityInDays)
                .InclusiveBetween(1, 365)
                .When(x => x.CertificateExists == true && x.CertificateValidityInDays.HasValue)
                .WithMessage(localization.GetLocalizedString("TrainingPathCertificateValidityRange"));

            RuleFor(x => x.MaxEnrollment)
                .GreaterThan(0)
                .When(x => x.MaxEnrollment.HasValue)
                .WithMessage(localization.GetLocalizedString("TrainingPathMaxEnrollmentMustBePositive"));
        }
    }
}
