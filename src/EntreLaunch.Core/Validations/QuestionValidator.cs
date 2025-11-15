using FluentValidation;

namespace EntreLaunch.Validations;

public class QuestionCreateValidator : AbstractValidator<QuestionCreateDto>
{
    public QuestionCreateValidator()
    {
        RuleFor(x => x.ExamId).MustNotBeDefault();

        RuleFor(x => x.Text).MustNotBeDefault();
        RuleFor(x => x.Text).MustHaveLengthInRange(2, 2000);

        RuleFor(x => x.Mark).MustNotBeDefault();
        RuleFor(x => x.Mark).MustBeInRange(0.1m, 100m);
    }
}

public class QuestionEntreLaunchdateValidator : AbstractValidator<QuestionEntreLaunchdateDto>
{
    public QuestionEntreLaunchdateValidator()
    {
        RuleFor(x => x.ExamId).MustNotBeDefault();

        //RuleFor(x => x.Text).MustHaveLengthInRange(2, 2000);

        //RuleFor(x => x.Mark).MustBeInRange(0.01m, 0.100m);
    }
}
