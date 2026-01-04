using EntreLaunch.DTOs.SimulationDtos;

namespace EntreLaunch.Validations.SimulationValidators
{
    public class MarketingCreateDtoValidator : AbstractValidator<MarketingCreateDto>
    {
        public MarketingCreateDtoValidator(ILocalizationManager localization)
        {
            RuleFor(x => x.ProductName)
                .MustNotBeDefault();

            RuleFor(x => x.ProductName)
                .MustHaveLengthInRange(2, 100)
                .WithMessage(localization.GetLocalizedString("MarketingProductNameRequired"));

            RuleFor(x => x.Quantity)
                .NotNull();

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage(localization.GetLocalizedString("MarketingQuantityPositive"));

            RuleFor(x => x.UnitPrice)
                .NotNull();

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0)
                .WithMessage(localization.GetLocalizedString("MarketingUnitPricePositive"));

            RuleForEach(x => x.Ads).ChildRules(ad =>
            {
                ad.RuleFor(a => a.AdUrl)
                    .MustBeValidAttachment()
                    .WithMessage(localization.GetLocalizedString("MarketingAdUrlInvalid"));

                ad.RuleFor(a => a.AdType)
                    .MustNotBeDefault();

                ad.RuleFor(a => a.AdType)
                    .MustHaveLengthInRange(2, 50)
                    .WithMessage(localization.GetLocalizedString("MarketingAdTypeRequired"));
            });
        }
    }
}
