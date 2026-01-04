using EntreLaunch.DTOs.MyOpportunityDtos;

namespace EntreLaunch.Validations.MyOpportunityValidators
{
    public class OpportunityCreateValidator : AbstractValidator<OpportunityCreateDto>
    {
        public OpportunityCreateValidator(ILocalizationManager localizationManager)
        {
            RuleFor(x => x.CompanyName)
                .Must(name => string.IsNullOrWhiteSpace(name) || !string.IsNullOrWhiteSpace(name))
                .WithMessage(localizationManager.GetLocalizedString("OpportunityCompanyNameRequired"));

            RuleFor(x => x.CompanyName)
                .Length(3, 200)
                .When(x => !string.IsNullOrWhiteSpace(x.CompanyName))
                .WithMessage(localizationManager.GetLocalizedString("OpportunityCompanyNameLength"));

            RuleFor(x => x.Logo)
                .MustBeValidImage()
                .WithMessage(localizationManager.GetLocalizedString("OpportunityLogoInvalid"))
                .When(x => !string.IsNullOrWhiteSpace(x.Logo));

            RuleFor(x => x.Description)
                .Length(10, 1000)
                .When(x => !string.IsNullOrWhiteSpace(x.Description))
                .WithMessage(localizationManager.GetLocalizedString("OpportunityDescriptionLength"));

            RuleFor(x => x.Sector)
                .Length(2, 100)
                .When(x => !string.IsNullOrWhiteSpace(x.Sector))
                .WithMessage(localizationManager.GetLocalizedString("OpportunitySectorLength"));

            RuleFor(x => x.Costs)
                .GreaterThan(0)
                .When(x => x.Costs.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("OpportunityCostsPositive"));

            RuleFor(x => x.ContractDurationInDay)
                .GreaterThan(0)
                .When(x => x.ContractDurationInDay.HasValue)
                .WithMessage(localizationManager.GetLocalizedString("OpportunityContractDurationPositive"));

            RuleFor(x => x.AcceptRequirements)
                .Must(list => list != null && list.Any())
                .When(x => x.AcceptRequirements != null)
                .WithMessage(localizationManager.GetLocalizedString("OpportunityAcceptRequirementsNotEmpty"));

            RuleFor(x => x.AcceptRequirements)
                .Must(list => list == null || list.TrueForAll(item => !string.IsNullOrWhiteSpace(item)))
                .WithMessage(localizationManager.GetLocalizedString("OpportunityAcceptRequirementItemNotEmpty"));

            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage(localizationManager.GetLocalizedString("OpportunityTypeInvalid"));
        }
    }
}
