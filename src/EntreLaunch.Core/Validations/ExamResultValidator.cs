using FluentValidation;

namespace EntreLaunch.Validations;

public class ExamResultCreateValidator : AbstractValidator<ExamResultCreateDto>
{
    public ExamResultCreateValidator()
    {
        RuleFor(x => x.Mark).MustNotBeDefault();
        RuleFor(x => x.Mark).MustBeInRange(1, 100);

        RuleFor(x => x.UserId).MustNotBeDefault();

        RuleFor(x => x.ExamId).MustNotBeDefault();

        RuleFor(x => x.Status).MustNotBeDefault();
        RuleFor(x => x.Status).MustContainOnlyLettersAndSpaces();
        RuleFor(x => x.Status).MustHaveLengthInRange(2, 50);
    }
}

public class ExamResultEntreLaunchdateValidator : AbstractValidator<ExamResultEntreLaunchdateDto>
{
    public ExamResultEntreLaunchdateValidator()
    {
        //RuleFor(x => x.Mark).MustBeInRange(1, 100);

        //RuleFor(x => x.UserId).MustNotBeDefault();

        //RuleFor(x => x.ExamId).MustNotBeDefault();

        //RuleFor(x => x.Status).MustContainOnlyLettersAndSpaces();
        //RuleFor(x => x.Status).MustHaveLengthInRange(2, 50);
    }
}
