using FluentValidation;

namespace EntreLaunch.Validations;

/// <summary>
/// Validator for <see cref="MyPartnerCreateDto"/>.
/// </summary>
public class ProjectCreateValidator : AbstractValidator<MyPartnerCreateDto>
{
    public ProjectCreateValidator()
    {
        RuleFor(x => x.Activity).MustNotBeDefault();
        RuleFor(x => x.Activity).MustContainOnlyLettersAndSpaces();
        RuleFor(x => x.Activity).MustHaveLengthInRange(2, 250);

        RuleFor(x => x.City).MustNotBeDefault();
        RuleFor(x => x.City).MustContainOnlyLettersAndSpaces();
        RuleFor(x => x.City).MustHaveLengthInRange(2, 250);

        RuleFor(x => x.Sector).MustNotBeDefault();
        RuleFor(x => x.Sector).MustContainOnlyLettersAndSpaces();
        RuleFor(x => x.Sector).MustHaveLengthInRange(2, 250);

        RuleFor(x => x.Cost).MustNotBeDefault();
        RuleFor(x => x.Cost).MustBeInRange(0.01m, decimal.MaxValue);

        RuleFor(x => x.Idea).MustNotBeDefault();
        RuleFor(x => x.Idea).MustBeValidDescription(minLength: 10, maxLength: 2000, allowSpecialCharacters: true);

        RuleFor(x => x.AcceptRequirements).MustNotBeDefault();

        RuleFor(x => x.CapitalFrom).MustNotBeDefault();
        RuleFor(x => x.CapitalFrom).MustBeInRange(0.01m, decimal.MaxValue);
        RuleFor(x => x.CapitalFrom).MustBeLessThanOrEqualTo(x => x.CapitalTo);

        RuleFor(x => x.CapitalTo).MustBeInRange(0.01m, decimal.MaxValue);
        RuleFor(x => x.CapitalTo).MustNotBeDefault();

        RuleFor(x => x.Contact).MustNotBeDefault();
        RuleFor(x => x.Contact).MustHaveLengthInRange(2, 250);
    }
}

/// <summary>
/// Validator for <see cref="MyPartnerEntreLaunchdateDto"/>.
/// </summary>
public class ProjectEntreLaunchdateValidator : AbstractValidator<MyPartnerEntreLaunchdateDto>
{
    public ProjectEntreLaunchdateValidator()
    {
        //RuleFor(x => x.Activity).MustContainOnlyLettersAndSpaces();
        //RuleFor(x => x.Activity).MustHaveLengthInRange(2, 250);

        //RuleFor(x => x.City).MustContainOnlyLettersAndSpaces();
        //RuleFor(x => x.City).MustHaveLengthInRange(2, 250);

        //RuleFor(x => x.Sector).MustContainOnlyLettersAndSpaces();
        //RuleFor(x => x.Sector).MustHaveLengthInRange(2, 250);

        //RuleFor(x => x.Cost).MustBeInRange(0.01m, decimal.MaxValue);

        //RuleFor(x => x.Idea).MustBeValidDescription(minLength: 10, maxLength: 2000, allowSpecialCharacters: true);

        //RuleFor(x => x.CapitalFrom).MustBeInRange(0.01m, decimal.MaxValue);
        //RuleFor(x => x.CapitalFrom).MustBeLessThanOrEqualTo(x => x.CapitalTo);

        //RuleFor(x => x.CapitalTo).MustBeInRange(0.01m, decimal.MaxValue);

        //RuleFor(x => x.Contact).MustContainOnlyLettersAndSpaces();
        //RuleFor(x => x.Contact).MustHaveLengthInRange(2, 250);
    }
}

/// <summary>
/// Validator for <see cref="MyPartnerAttachmentCreateDto"/>.
/// </summary>
public class ProjectAttachmentCreateValidator : AbstractValidator<MyPartnerAttachmentCreateDto>
{
    public ProjectAttachmentCreateValidator()
    {
        RuleFor(x => x.Url).MustNotBeDefault();
        RuleFor(x => x.Url).MustBeValidAttachment();
        RuleFor(x => x.Url).MustBeValidImage();
    }
}

/// <summary>
/// Validator for <see cref="ProjectAttachmentEntreLaunchdateDto"/>.
/// </summary>
public class ProjectAttachmentEntreLaunchdateValidator : AbstractValidator<ProjectAttachmentEntreLaunchdateDto>
{
    public ProjectAttachmentEntreLaunchdateValidator()
    {
        //RuleFor(x => x.Url).MustBeValidAttachment();
        //RuleFor(x => x.Url).MustBeValidImage();
    }
}

public class FilterProjectsValidator : AbstractValidator<FilterProjectsDto>
{
    public FilterProjectsValidator()
    {
        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City can not be null");

        RuleFor(x => x.Activity)
            .NotEmpty().WithMessage("Activity can not be null");

        RuleFor(x => x.CapitalFrom)
            .GreaterThan(0).WithMessage("CapitalFrom must be greater than 0");

        RuleFor(x => x.CapitalTo)
            .GreaterThan(0).WithMessage("CapitalTo must be greater than 0");

        RuleFor(x => x.CapitalTo)
            .GreaterThanOrEqualTo(x => x.CapitalFrom)
            .WithMessage("CapitalTo must be greater than or equal to CapitalFrom");
    }
}
