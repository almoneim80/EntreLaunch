using FluentValidation;

namespace EntreLaunch.Validations;

public class CourseRatingCreateValidator : AbstractValidator<CourseRatingCreateDto>
{
    public CourseRatingCreateValidator()
    {
        RuleFor(x => x.CourseId).MustNotBeDefault();

        RuleFor(x => x.Rating).MustNotBeDefault();
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);

        RuleFor(x => x.Review).MustNotBeDefault();
        RuleFor(x => x.Review).MustHaveLengthInRange(2, 250);
    }
}

public class CourseRatingEntreLaunchdateValidator : AbstractValidator<CourseRatingEntreLaunchdateDto>
{
    public CourseRatingEntreLaunchdateValidator()
    {
        RuleFor(x => x.CourseId).MustNotBeDefault();

        //RuleFor(x => x.Review).MustHaveLengthInRange(2, 250);

        //RuleFor(x => x.Rating).InclusiveBetween(1, 5);
    }
}
