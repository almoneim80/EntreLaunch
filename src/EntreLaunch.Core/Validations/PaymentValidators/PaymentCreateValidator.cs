using EntreLaunch.DTOs.PaymentDtos;

namespace EntreLaunch.Validations.PaymentValidators
{
    public class PaymentCreateValidator : AbstractValidator<PaymentCreateDto>
    {
        public PaymentCreateValidator(ILocalizationManager localizationManager)
        {
            RuleFor(x => x.Amount)
                .NotNull();

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage(localizationManager.GetLocalizedString("PaymentAmountRequired"));

            RuleFor(x => x.NetAmount)
                .GreaterThanOrEqualTo(0)
                .When(x => x.NetAmount.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("PaymentNetAmountNonNegative"));

            RuleFor(x => x.DiscountAmount)
                .GreaterThanOrEqualTo(0)
                .When(x => x.DiscountAmount.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("PaymentDiscountNonNegative"));

            RuleFor(x => x.DiscountAmount)
                .LessThanOrEqualTo(x => x.Amount ?? decimal.MaxValue)
                .When(x => x.DiscountAmount.HasValue && x.Amount.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("PaymentDiscountMustBeLessThanAmount"));

            RuleFor(x => x.Currency)
                .Length(2, 5)
                .When(x => !string.IsNullOrWhiteSpace(x.Currency))
                .WithMessage(localizationManager.GetLocalizedString("PaymentCurrencyLength"));

            RuleFor(x => x.Status)
                .NotEmpty()
                .When(x => !string.IsNullOrWhiteSpace(x.Status))
                .WithMessage(localizationManager.GetLocalizedString("PaymentStatusRequired"));

            RuleFor(x => x.TargetId)
                .GreaterThan(0)
                .When(x => x.TargetId.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("PaymentTargetIdPositive"));
        }
    }
}
