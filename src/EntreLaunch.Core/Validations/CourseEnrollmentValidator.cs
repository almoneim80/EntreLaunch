using FluentValidation;

namespace EntreLaunch.Validations;

public class CourseEnrollmentCreateValidator : AbstractValidator<CourseEnrollmentCreateDto>
{
    public CourseEnrollmentCreateValidator()
    {
        RuleFor(x => x.UserId).MustNotBeDefault();

        RuleFor(x => x.CourseId).MustNotBeDefault();
    }
}

public class CourseEnrollmentEntreLaunchdateValidator : AbstractValidator<CourseEnrollmentEntreLaunchdateDto>
{
    public CourseEnrollmentEntreLaunchdateValidator()
    {
        RuleFor(x => x.UserId).MustNotBeDefault();

        RuleFor(x => x.CourseId).MustNotBeDefault();
    }
}
