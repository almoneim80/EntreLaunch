using FluentValidation;

namespace EntreLaunch.Validations;

public class TrainingPathCreateValidator : AbstractValidator<TrainingPathCreateDto>
{
    public TrainingPathCreateValidator()
    {
        RuleFor(x => x.Name).MustNotBeDefault();
        RuleFor(x => x.Name).MustHaveLengthInRange(2, 250);

        RuleFor(x => x.Price).MustNotBeDefault();
        RuleFor(x => x.Price).MustBeInRange(0.01m, decimal.MaxValue);

        RuleFor(x => x.Description).MustNotBeDefault();
        RuleFor(x => x.Description).MustBeValidDescription(minLength: 10, maxLength: 2000, allowSpecialCharacters: true);
    }
}

public class TrainingPathEntreLaunchdateValidator : AbstractValidator<TrainingPathEntreLaunchdateDto>
{
    public TrainingPathEntreLaunchdateValidator()
    {
        //RuleFor(x => x.Name).MustContainOnlyLettersAndSpaces();
        //RuleFor(x => x.Name).MustHaveLengthInRange(2, 250);

        //RuleFor(x => x.Price).MustBeInRange(0.01m, decimal.MaxValue);

        //RuleFor(x => x.Description).MustBeValidDescription(minLength: 10, maxLength: 2000, allowSpecialCharacters: true);
    }
}
