using EntreLaunch.DTOs.ConsultationDtos;

namespace EntreLaunch.Validations.ConsultationValidators
{
    public class ProcessConsultationStatusValidator : AbstractValidator<ProcessConsultationStatusDto>
    {
        public ProcessConsultationStatusValidator(ILocalizationManager localizationManager)
        {
            // Validate Id: must be greater than 0
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage(localizationManager.GetLocalizedString("IdGreaterThanZero"));

            // Validate Status: must be a valid enum value
            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage(localizationManager.GetLocalizedString("InvalidConsultationStatus"));
        }
    }
}
