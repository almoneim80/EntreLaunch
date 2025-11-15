using FluentValidation;

namespace EntreLaunch.Validations;

public class TeamEmployeeCreateValidator : AbstractValidator<EmployeeCreateDto>
{
    public TeamEmployeeCreateValidator()
    {
        RuleFor(x => x.UserId).MustNotBeDefault();

        RuleFor(x => x.WorkField).MustNotBeDefault();
        RuleFor(x => x.WorkField).MustHaveLengthInRange(2, 500);

        RuleFor(x => x.JobTitle).MustNotBeDefault();
        RuleFor(x => x.JobTitle).MustHaveLengthInRange(2, 500);

        RuleFor(x => x.EmployeeDefinition).MustNotBeDefault();
        RuleFor(x => x.EmployeeDefinition).MustHaveLengthInRange(2, 3000);

        RuleFor(x => x.Skills).MustNotBeDefault();
    }
}

public class PortfolioCreateValidator : AbstractValidator<EmployeePortfolioCreateDto>
{
    public PortfolioCreateValidator()
    {
        RuleFor(x => x.ProjectTitle).MustNotBeDefault();
        RuleFor(x => x.ProjectTitle).MustHaveLengthInRange(2, 500);

        RuleFor(x => x.CostFrom).MustNotBeDefault();

        RuleFor(x => x.CostTo).MustNotBeDefault();

        RuleFor(x => x.Description).MustNotBeDefault();
        RuleFor(x => x.Description).MustBeValidDescription(minLength: 10, maxLength: 3000, allowSpecialCharacters: true);

        RuleFor(x => x.About).MustNotBeDefault();
        RuleFor(x => x.About).MustBeValidDescription(minLength: 10, maxLength: 3000, allowSpecialCharacters: true);

        RuleFor(x => x.Logo).MustNotBeDefault();
        RuleFor(x => x.Logo).MustBeValidImage();
    }
}

public class PortfolioAttachmentCreateValidator : AbstractValidator<PortfolioAttachmentCreateDto>
{
    public PortfolioAttachmentCreateValidator()
    {
        RuleFor(x => x.Url).MustNotBeDefault();
        RuleFor(x => x.Url).MustBeValidAttachment();
        RuleFor(x => x.Url).MustBeValidImage();
    }
}
