using EntreLaunch.DTOs.ConsultationDtos;

namespace EntreLaunch.Validations.ConsultationValidators
{
    public class ConsultationUpdateValidator : AbstractValidator<ConsultationUpdateDto>
    {
        public ConsultationUpdateValidator(ILocalizationManager localizationManager)
        {
            // Validate CounselorId: must be non-empty if provided
            RuleFor(x => x.CounselorId)
                .NotEmpty().When(x => x.CounselorId != null)
                .WithMessage(localizationManager.GetLocalizedString("CounselorIdCannotBeEmpty"));

            // Validate ConsultationTimeId: must be greater than 0 if provided
            RuleFor(x => x.ConsultationTimeId)
                .GreaterThan(0).When(x => x.ConsultationTimeId.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("ConsultationTimeIdGreaterThanZero"));

            // Validate Type: must be a valid enum value if provided
            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage(localizationManager.GetLocalizedString("InvalidConsultationType"));

            // Validate Status: must be a valid enum value if provided
            RuleFor(x => x.Status)
                .IsInEnum().When(x => x.Status.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("InvalidConsultationStatus"));

            // Validate Description: must not be empty and should be in valid length range if provided
            RuleFor(x => x.Description)
                .MustHaveLengthInRange(10, 500).When(x => !string.IsNullOrWhiteSpace(x.Description))
                .WithMessage(localizationManager.GetLocalizedString("DescriptionLengthRange"));

            // Validate TicketId: must be greater than 0 if provided
            RuleFor(x => x.TicketId)
                .GreaterThan(0).When(x => x.TicketId.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("TicketIdGreaterThanZero"));
        }
    }
}
