using FluentValidation;

namespace EntreLaunch.Validations;

public class LessonCreateValidator : AbstractValidator<LessonCreateDto>
{
    public LessonCreateValidator()
    {
        RuleFor(x => x.Name).MustNotBeDefault();
        RuleFor(x => x.Name).MustContainOnlyLettersAndSpaces();
        RuleFor(x => x.Name).MustHaveLengthInRange(2, 250);

        RuleFor(x => x.VideoUrl).MustNotBeDefault();
        RuleFor(x => x.VideoUrl).MustBeValidAttachment();
        RuleFor(x => x.VideoUrl).MustBeValidVideo();

        RuleFor(x => x.DurationInMinutes).MustNotBeDefault();
        RuleFor(x => x.DurationInMinutes).MustBeInRange(1, maxValue: int.MaxValue);

        RuleFor(x => x.Description).MustNotBeDefault();
        RuleFor(x => x.Description).MustBeValidDescription(minLength: 10, maxLength: 2000, allowSpecialCharacters: true);

        RuleFor(x => x.CourseId).MustNotBeDefault();
    }
}

public class LessonEntreLaunchdateValidator : AbstractValidator<LessonEntreLaunchdateDto>
{
    public LessonEntreLaunchdateValidator()
    {
        //RuleFor(x => x.Name).MustContainOnlyLettersAndSpaces();
        //RuleFor(x => x.Name).MustHaveLengthInRange(2, 250);

        //RuleFor(x => x.VideoUrl).MustBeValidAttachment();
        //RuleFor(x => x.VideoUrl).MustBeValidVideo();

        //RuleFor(x => x.Description).MustBeValidDescription(minLength: 10, maxLength: 2000, allowSpecialCharacters: true);

        //RuleFor(x => x.CourseId).MustNotBeDefault();
    }
}
