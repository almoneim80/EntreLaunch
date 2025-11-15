using FluentValidation;

namespace EntreLaunch.Validations;

public class AnswerCreateValidator : AbstractValidator<AnswerCreateDto>
{
    public AnswerCreateValidator()
    {
        RuleFor(x => x.QuestionId).MustNotBeDefault();

        RuleFor(x => x.Text).MustNotBeDefault();
        RuleFor(x => x.Text).MustHaveLengthInRange(10, 2500);

        RuleFor(x => x.IsCorrect).MustNotBeDefault();
    }
}

public class AnswerEntreLaunchdateValidator : AbstractValidator<AnswerEntreLaunchdateDto>
{
    public AnswerEntreLaunchdateValidator()
    {
        RuleFor(x => x.QuestionId).MustNotBeDefault();

        //RuleFor(x => x.Text).MustBeValidDescription(minLength: 10, maxLength: 3000, allowSpecialCharacters: true);
    }
}
