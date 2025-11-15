using FluentValidation;

namespace EntreLaunch.Validations;

public class PaymentCreateValidator : AbstractValidator<PaymentCreateDto>
{
    public PaymentCreateValidator()
    {
        RuleFor(x => x.UserId).MustNotBeDefault();

        RuleFor(x => x.TargetId).MustNotBeDefault();

        RuleFor(x => x.Amount).MustBeInRange(0.01m, decimal.MaxValue);
        RuleFor(x => x.Amount).MustNotBeDefault();

        RuleFor(x => x.PaymentPurpose).MustNotBeDefault();
        RuleFor(x => x.PaymentPurpose).MustContainOnlyLettersAndSpaces();
        RuleFor(x => x.PaymentPurpose).MustHaveLengthInRange(2, 250);

        RuleFor(x => x.TargetType).MustNotBeDefault();
        RuleFor(x => x.TargetType).MustContainOnlyLettersAndSpaces();
        RuleFor(x => x.TargetType).MustHaveLengthInRange(2, 250);
    }
}

public class PaymentEntreLaunchdateValidator : AbstractValidator<PaymentEntreLaunchdateDto>
{
    public PaymentEntreLaunchdateValidator()
    {
        RuleFor(x => x.UserId).MustNotBeDefault();

        //RuleFor(x => x.TargetId).MustNotBeDefault();

        //RuleFor(x => x.Amount).MustBeInRange(0.01m, decimal.MaxValue);

        //RuleFor(x => x.PaymentPurpose).MustContainOnlyLettersAndSpaces();
        //RuleFor(x => x.PaymentPurpose).MustHaveLengthInRange(2, 250);

        //RuleFor(x => x.TargetType).MustContainOnlyLettersAndSpaces();
        //RuleFor(x => x.TargetType).MustHaveLengthInRange(2, 250);
    }
}
