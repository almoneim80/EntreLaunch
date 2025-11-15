using FluentValidation;

namespace EntreLaunch.Validations;

public class WheelAwardCreateValidator : AbstractValidator<WheelAwardCreateDto>
{
    public WheelAwardCreateValidator()
    {
        RuleFor(x => x.Name).MustNotBeDefault();
        RuleFor(x => x.Name).MustContainOnlyLettersAndSpaces();
        RuleFor(x => x.Name).MustHaveLengthInRange(2, 250);

        RuleFor(x => x.Description).MustNotBeDefault();
        RuleFor(x => x.Description).MustBeValidDescription(minLength: 10, maxLength: 2000, allowSpecialCharacters: true);

        RuleFor(x => x.Probability).MustNotBeDefault();
        RuleFor(x => x.Probability).MustBeInRange(0.01m, 0.1m);
    }
}

public class WheelAwardEntreLaunchdateValidator : AbstractValidator<WheelAwardEntreLaunchdateDto>
{
    public WheelAwardEntreLaunchdateValidator()
    {
    }
}
