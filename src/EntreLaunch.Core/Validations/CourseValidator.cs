using FluentValidation;

namespace EntreLaunch.Validations;

public class CourseCreateValidator : AbstractValidator<CourseCreateDto>
{
    public CourseCreateValidator()
    {
        RuleFor(x => x.Name).MustNotBeDefault();
        RuleFor(x => x.Name).MustHaveLengthInRange(5, 250);

        RuleFor(x => x.Description).MustNotBeDefault();
        RuleFor(x => x.Description).MustBeValidDescription(minLength: 10, maxLength: 2000, allowSpecialCharacters: true);

        RuleFor(x => x.Price).MustBeInRange(0.01m, decimal.MaxValue);

        RuleFor(x => x.Discount).MustBeInRange(0, 100);

        RuleFor(x => x.StudyWay).MustNotBeDefault(); 
        RuleFor(x => x.StudyWay).MustContainOnlyLettersAndSpaces();
        RuleFor(x => x.Name).MustHaveLengthInRange(5, 250);

        RuleFor(x => x.DurationInDays!).MustBeInRange(1, maxValue: int.MaxValue);

        RuleFor(x => x.StartDate).MustBeValidDate(mustBeFuture: true);
        //RuleFor(x => x.StartDate)
        //    .MustBeEarlierThan(x => x.EndDate, x => x.EndDate.DateTime.ToShortDateString()
        //    ?? "Unknown End Date");

        RuleFor(x => x.EndDate).MustBeValidDate(mustBeFuture: true);

        RuleFor(x => x.CertificateUrl).MustBeValidAttachment();
        RuleFor(x => x.CertificateUrl).MustBeValidImage();

        RuleFor(x => x.Logo).MustBeValidAttachment();
        RuleFor(x => x.Logo).MustBeValidImage();

        RuleFor(x => x.Audience).MustNotBeDefault();

        RuleFor(x => x.Requirements).MustNotBeDefault();

        RuleFor(x => x.Topics).MustNotBeDefault();

        RuleFor(x => x.Goals).MustNotBeDefault();

        RuleFor(x => x.Outcomes).MustNotBeDefault();
    }
}

public class CourseEntreLaunchdateValidator : AbstractValidator<CourseEntreLaunchdateDto>
{
    public CourseEntreLaunchdateValidator()
    {
        //RuleFor(x => x.Name).MustHaveLengthInRange(5, 250);

        //RuleFor(x => x.Price).MustBeInRange(0.01m, decimal.MaxValue);

        //RuleFor(x => x.Discount).MustBeInRange(0, 100);

        //RuleFor(x => x.Description).MustBeValidDescription(minLength: 10, maxLength: 2000, allowSpecialCharacters: true);

        //RuleFor(x => x.Field).MustContainOnlyLettersAndSpaces();
        //RuleFor(x => x.Field).MustHaveLengthInRange(5, 250);

        //RuleFor(x => x.StudyWay).MustContainOnlyLettersAndSpaces();
        //RuleFor(x => x.Name).MustHaveLengthInRange(5, 250);

        //RuleFor(x => x.DurationInDays).MustBeInRange(1, maxValue: int.MaxValue);

        //RuleFor(x => x.StartDate).MustBeValidDate(mustBeFuture: true);
        //RuleFor(x => x.StartDate).MustBeEarlierThan(x => x.EndDate, x => x.EndDate?.DateTime.ToShortDateString() ?? "Unknown End Date");

        //RuleFor(x => x.EndDate).MustBeValidDate(mustBeFuture: true);

        //RuleFor(x => x.CertificateUrl).MustBeValidAttachment();
        //RuleFor(x => x.CertificateUrl).MustBeValidImage();

        //RuleFor(x => x.Logo).MustBeValidAttachment();
        //RuleFor(x => x.Logo).MustBeValidImage();
    }
}
