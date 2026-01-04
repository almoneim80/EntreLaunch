using EntreLaunch.DTOs.MyOpportunityDtos;

namespace EntreLaunch.Validations.MyOpportunityValidators
{
    public class CreateOpportunityRequestValidator : AbstractValidator<CreateOpportunityRequestDto>
    {
        public CreateOpportunityRequestValidator(ILocalizationManager localizationManager)
        {
            RuleFor(x => x.OpportunityId)
                .GreaterThan(0)
                .WithMessage(localizationManager.GetLocalizedString("OpportunityIdMustBeGreaterThanZero"));

            RuleFor(x => x.City)
                .Length(2, 100)
                .When(x => !string.IsNullOrWhiteSpace(x.City))
                .WithMessage(localizationManager.GetLocalizedString("CreateOpportunityRequestCityLength"));

            RuleFor(x => x.ShareCapital)
                .GreaterThanOrEqualTo(0)
                .When(x => x.ShareCapital.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("ShareCapitalNonNegative"));

            RuleFor(x => x.LoanRatio)
                .InclusiveBetween(0, 100)
                .When(x => x.LoanRatio.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("LoanRatioBetweenZeroAnd100"));

            RuleFor(x => x.ManagementExperince)
                .GreaterThanOrEqualTo(0)
                .WithMessage(localizationManager.GetLocalizedString("ManagementExperienceNonNegative"));

            RuleFor(x => x.FranchiseExperince)
                .GreaterThanOrEqualTo(0)
                .When(x => x.HaveFranchiseProjects)
                .WithMessage(localizationManager.GetLocalizedString("FranchiseExperienceNonNegative"));
        }
    }
}
