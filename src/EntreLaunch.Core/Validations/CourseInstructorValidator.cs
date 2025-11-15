using FluentValidation;

namespace EntreLaunch.Validations;

public class CourseInstructorCreateValidator : AbstractValidator<CourseInstructorCreateDto>
{
    public CourseInstructorCreateValidator()
    {
        RuleFor(x => x.UserId).MustNotBeDefault();

        RuleFor(x => x.CourseId).MustNotBeDefault();
    }
}

public class CourseInstructorEntreLaunchdateValidator : AbstractValidator<CourseInstructorEntreLaunchdateDto>
{
    public CourseInstructorEntreLaunchdateValidator()
    {
        RuleFor(x => x.UserId).MustNotBeDefault();

        RuleFor(x => x.CourseId).MustNotBeDefault();
    }
}
