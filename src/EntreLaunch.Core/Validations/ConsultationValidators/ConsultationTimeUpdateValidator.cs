using EntreLaunch.DTOs.ConsultationDtos;

namespace EntreLaunch.Validations.ConsultationValidators
{
    public class ConsultationTimeUpdateValidator : AbstractValidator<ConsultationTimeUpdateDto>
    {
        public ConsultationTimeUpdateValidator(ILocalizationManager localizationManager)
        {
            // Validate CounselorId: must be greater than 0 if provided
            RuleFor(x => x.CounselorId)
                .GreaterThan(0).When(x => x.CounselorId != 0)
                .WithMessage(localizationManager.GetLocalizedString("CounselorIdGreaterThanZero"));

            // Validate DateTimeSlot: must be a valid DateTime if provided
            RuleFor(x => x.DateTimeSlot)
                .GreaterThan(DateTimeOffset.MinValue).When(x => x.DateTimeSlot.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("InvalidDateTimeSlot"));
        }
    }
}
