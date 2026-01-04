using EntreLaunch.DTOs.ConsultationDtos;

namespace EntreLaunch.Validations.ConsultationValidators
{
    public class ConsultationTimeImportValidator : AbstractValidator<ConsultationTimeImportDto>
    {
        public ConsultationTimeImportValidator(ILocalizationManager localizationManager)
        {
            // Validate CounselorId: must be greater than 0
            RuleFor(x => x.CounselorId)
                .GreaterThan(0)
                .WithMessage(localizationManager.GetLocalizedString("CounselorIdGreaterThanZero"));

            // Validate DateTimeSlot: must be a valid DateTime if provided
            RuleFor(x => x.DateTimeSlot)
                .GreaterThan(DateTimeOffset.MinValue).When(x => x.DateTimeSlot.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("InvalidDateTimeSlot"));

            // Validate IsBooked: must be a valid bool (true or false)
            RuleFor(x => x.IsBooked)
                .NotNull()
                .WithMessage(localizationManager.GetLocalizedString("IsBookedInvalid"));
        }
    }
}
