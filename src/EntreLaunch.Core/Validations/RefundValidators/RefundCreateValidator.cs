using EntreLaunch.DTOs.PaymentDtos;

namespace EntreLaunch.Validations.RefundValidators
{
    public class RefundCreateValidator : AbstractValidator<RefundCreateDto>
    {
        public RefundCreateValidator(ILocalizationManager localizationManager)
        {
            RuleFor(x => x.PaymentId)
                .GreaterThan(0)
                .WithMessage(localizationManager.GetLocalizedString("RefundPaymentIdRequired"));

            RuleFor(x => x.Reason)
                .MaximumLength(1000)
                .When(x => !string.IsNullOrWhiteSpace(x.Reason))
                .WithMessage(localizationManager.GetLocalizedString("RefundReasonMaxLength"));
        }
    }
}
