using FluentValidation;

namespace EntreLaunch.Validations;

public class ExamCreateValidator : AbstractValidator<ExamCreateDto>
{
    public ExamCreateValidator()
    {
        RuleFor(x => x.Name).MustNotBeDefault();
        RuleFor(x => x.Name).MustHaveLengthInRange(2, 250);

        RuleFor(x => x.Type).MustNotBeDefault();
        RuleFor(x => x.Type).MustHaveLengthInRange(2, 250);

        RuleFor(x => x.Description).MustNotBeDefault();
        RuleFor(x => x.Description).MustBeValidDescription(minLength: 10, maxLength: 2000, allowSpecialCharacters: true);

        RuleFor(x => x.MinMark).MustBeInRange(1, 100);
        RuleFor(x => x.MinMark).MustNotBeDefault();
        RuleFor(x => x.MaxMark).MustBeLessThanOrEqualTo(x => x.MaxMark);

        RuleFor(x => x.MaxMark).MustNotBeDefault();
        RuleFor(x => x.MaxMark).MustBeInRange(1, 100);

        RuleFor(x => x.DurationInMinutes).MustNotBeDefault();
        RuleFor(x => x.DurationInMinutes).MustBeInRange(1, maxValue: int.MaxValue);

        RuleFor(x => x.ParentEntityType).MustNotBeDefault();
    } 
}

public class ExamEntreLaunchdateValidator : AbstractValidator<ExamEntreLaunchdateDto>
{
    public ExamEntreLaunchdateValidator()
    {
        //RuleFor(x => x.Name).MustHaveLengthInRange(2, 250);

        //RuleFor(x => x.Type).MustHaveLengthInRange(2, 250);

        //RuleFor(x => x.Description).MustBeValidDescription(minLength: 10, maxLength: 2000, allowSpecialCharacters: true);

        //RuleFor(x => x.MaxMark).MustBeLessThanOrEqualTo(x => x.MaxMark);
        //RuleFor(x => x.MinMark).MustBeInRange(1, 100);

        //RuleFor(x => x.DurationInMinutes).MustBeInRange(1, maxValue: int.MaxValue);
    }
}
