using EntreLaunch.DTOs.ConsultationDtos;

namespace EntreLaunch.Validations.ConsultationValidators
{
    public class ConsultationTimeCreateValidator : AbstractValidator<ConsultationTimeCreateDto>
    {
        public ConsultationTimeCreateValidator(ILocalizationManager localizationManager)
        {
            // Validate CounselorId: must be greater than 0
            RuleFor(x => x.CounselorId)
                .GreaterThan(0)
                .WithMessage(localizationManager.GetLocalizedString("CounselorIdGreaterThanZero"));

            // Validate DateTimeSlot: must be a valid DateTime
            RuleFor(x => x.DateTimeSlot)
                .GreaterThan(DateTimeOffset.MinValue)
                .WithMessage(localizationManager.GetLocalizedString("InvalidDateTimeSlot"));

            // Validate IsRecurringDaily: no need for complex validation (true or false)
            RuleFor(x => x.IsRecurringDaily)
                .NotNull()
                .WithMessage(localizationManager.GetLocalizedString("IsRecurringDailyMustBeTrueOrFalse"));
        }
    }
}
