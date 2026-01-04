using EntreLaunch.DTOs.SimulationDtos;

namespace EntreLaunch.Validations.SimulationValidators
{
    public class FeasibilityStudyCreateDtoValidator : AbstractValidator<FeasibilityStudyCreateDto>
    {
        public FeasibilityStudyCreateDtoValidator(ILocalizationManager localization)
        {
            RuleFor(x => x.ProjectName)
                .MustNotBeDefault();

            RuleFor(x => x.ProjectName)
               .MustHaveLengthInRange(3, 200)
                .WithMessage(localization.GetLocalizedString("ProjectNameRequired"));

            RuleFor(x => x.CapitalMin)
                .GreaterThanOrEqualTo(0)
                .When(x => x.CapitalMin.HasValue)
                .WithMessage(localization.GetLocalizedString("CapitalMinNonNegative"));

            RuleFor(x => x.CapitalMax)
                .GreaterThanOrEqualTo(0)
                .When(x => x.CapitalMax.HasValue)
                .WithMessage(localization.GetLocalizedString("CapitalMaxNonNegative"));

            RuleFor(x => x)
                .Must(x => !x.CapitalMin.HasValue || !x.CapitalMax.HasValue || x.CapitalMin <= x.CapitalMax)
                .WithMessage(localization.GetLocalizedString("CapitalMinLessThanMax"));

            RuleFor(x => x.InterestRate)
                .GreaterThan(0)
                .When(x => x.IsInterest.HasValue && x.IsInterest.Value)
                .WithMessage(localization.GetLocalizedString("InterestRateRequired"));

            RuleFor(x => x.MarketingCost)
                .GreaterThanOrEqualTo(0).When(x => x.MarketingCost.HasValue)
                .WithMessage(localization.GetLocalizedString("MarketingCostMustBePositive"));

            RuleFor(x => x.RentCost)
                .GreaterThanOrEqualTo(0).When(x => x.RentCost.HasValue)
                .WithMessage(localization.GetLocalizedString("RentCostMustBePositive"));

            RuleFor(x => x.DecorationCost)
                .GreaterThanOrEqualTo(0).When(x => x.DecorationCost.HasValue)
                .WithMessage(localization.GetLocalizedString("DecorationCostMustBePositive"));

            RuleFor(x => x.EquipmentCost)
                .GreaterThanOrEqualTo(0).When(x => x.EquipmentCost.HasValue)
                .WithMessage(localization.GetLocalizedString("EquipmentCostMustBePositive"));

            RuleFor(x => x.GovFees)
                .GreaterThanOrEqualTo(0).When(x => x.GovFees.HasValue)
                .WithMessage(localization.GetLocalizedString("GovFeesMustBePositive"));

            RuleFor(x => x.InventoryCost)
                .GreaterThanOrEqualTo(0).When(x => x.InventoryCost.HasValue)
                .WithMessage(localization.GetLocalizedString("InventoryCostMustBePositive"));
        }
    }
}
