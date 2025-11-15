using FluentValidation;

namespace EntreLaunch.Validations;

public class InvestmentOpportunityCreateValidator : AbstractValidator<OpportunityCreateDto>
{
    public InvestmentOpportunityCreateValidator()
    {
        RuleFor(x => x.CompanyName).MustNotBeDefault();
        RuleFor(x => x.CompanyName).MustContainOnlyLettersAndSpaces();
        RuleFor(x => x.CompanyName).MustHaveLengthInRange(2, 250);

        RuleFor(x => x.Logo).MustNotBeDefault();
        RuleFor(x => x.Logo).MustBeValidAttachment();
        RuleFor(x => x.Logo).MustBeValidImage();

        RuleFor(x => x.Description).MustNotBeDefault();
        RuleFor(x => x.Description).MustBeValidDescription(minLength: 10, maxLength: 2000, allowSpecialCharacters: true);

        RuleFor(x => x.Sector).MustNotBeDefault();
        RuleFor(x => x.Sector).MustContainOnlyLettersAndSpaces();
        RuleFor(x => x.Sector).MustHaveLengthInRange(2, 250);

        RuleFor(x => x.Costs).MustNotBeDefault();
        RuleFor(x => x.Costs).MustBeInRange(0.01m, decimal.MaxValue);

        RuleFor(x => x.ContractDurationInDay).MustNotBeDefault();
        RuleFor(x => x.ContractDurationInDay).MustBeInRange(1, maxValue: int.MaxValue);

        RuleFor(x => x.AcceptRequirements).MustNotBeDefault();

        RuleFor(x => x.BrandCountry).MustNotBeDefault();
    }
}

public class InvestmentOpportunityEntreLaunchdateValidator : AbstractValidator<OpportunityEntreLaunchdateDto>
{
    public InvestmentOpportunityEntreLaunchdateValidator()
    {
        //RuleFor(x => x.CompanyName).MustContainOnlyLettersAndSpaces();
        //RuleFor(x => x.CompanyName).MustHaveLengthInRange(2, 250);

        //RuleFor(x => x.Logo).MustBeValidAttachment();
        //RuleFor(x => x.Logo).MustBeValidImage();

        //RuleFor(x => x.Description).MustBeValidDescription(minLength: 10, maxLength: 2000, allowSpecialCharacters: true);

        //RuleFor(x => x.Sector).MustContainOnlyLettersAndSpaces();
        //RuleFor(x => x.Sector).MustHaveLengthInRange(2, 250);

        //RuleFor(x => x.Costs).MustBeInRange(0.01m, decimal.MaxValue);

        //RuleFor(x => x.ContractDurationInDay).MustBeInRange(1, maxValue: int.MaxValue);
    }
}

public class FilterOpportunityValidator : AbstractValidator<OpportunityFilterDto>
{
    public FilterOpportunityValidator()
    {
        RuleFor(x => x.Sector).MustNotBeDefault();
        RuleFor(x => x.Sector).MustContainOnlyLettersAndSpaces();
        RuleFor(x => x.Sector).MustHaveLengthInRange(2, 250);

        RuleFor(x => x.Costs).MustNotBeDefault();
        RuleFor(x => x.Costs).MustBeInRange(0.01m, decimal.MaxValue);

        RuleFor(x => x.BrandCountry).MustNotBeDefault();
    }
}
